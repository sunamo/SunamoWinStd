namespace SunamoWinStd;

public partial class JunctionPoint
{
    /// <summary>
    /// The FSCTL GET REPARSE POINT constant.
    /// </summary>
    protected const int FSCTL_GET_REPARSE_POINT = 0x000900A8;
    /// <summary>
    /// The ERROR NOT a REPARSE POINT constant.
    /// </summary>
    protected const int ERROR_NOT_A_REPARSE_POINT = 4390;
    // For normal folder and /H returns false. For junction returns true.
    // Only works on NTFS.
    /// <summary>
    /// Is junction point.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="path">The path.</param>
    /// <returns>The is junction point result.</returns>
    public static bool IsJunctionPoint(ILogger logger, string path)
    {
        if (!Directory.Exists(path))
            return false;
        using (var handle = OpenReparsePoint(logger, path, EFileAccess.GenericRead))
        {
            var target = InternalGetTarget(logger, handle);
            return target != null;
        }
    }
    /// <summary>
    /// Initializes a new instance of the open reparse point class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    protected static SafeFileHandle? OpenReparsePoint(ILogger logger, string reparsePointPath, EFileAccess accessMode)
    {
        var reparsePointHandle = new SafeFileHandle(CreateFile(reparsePointPath, accessMode,
            EFileShare.Read | EFileShare.Write | EFileShare.Delete,
            0, ECreationDisposition.OpenExisting,
            EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, 0), true);
        var errorCode = Marshal.GetLastWin32Error();
        if (errorCode != 0)
            if (ThrowLastWin32Error(logger, errorCode, "UnableToOpenReparsePoint"))
            {
                return null;
            }
        return reparsePointHandle;
    }
    /// <summary>
    /// Create file.
    /// </summary>
    /// <param name=""kernel32.dll"">The kernel 32 dll.</param>
    /// <param name="SetLastError">The set last error.</param>
    /// <returns>The create file result.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern nint CreateFile(
        string lpFileName,
        EFileAccess dwDesiredAccess,
        EFileShare dwShareMode,
        nint lpSecurityAttributes,
        ECreationDisposition dwCreationDisposition,
        EFileAttributes dwFlagsAndAttributes,
        nint hTemplateFile);
    /// <summary>
    /// Device io control.
    /// </summary>
    /// <param name=""kernel32.dll"">The kernel 32 dll.</param>
    /// <param name="CharSet">The char set.</param>
    /// <param name="SetLastError">The set last error.</param>
    /// <returns>The device io control result.</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    protected static extern bool DeviceIoControl(nint hDevice, uint dwIoControlCode,
        nint InBuffer, int nInBufferSize,
        nint OutBuffer, int nOutBufferSize,
        out int pBytesReturned, nint lpOverlapped);
    /// <summary>
    /// The IO REPARSE TAG MOUNT POINT constant.
    /// </summary>
    protected const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;
    /// <summary>
    /// Initializes a new instance of the internal get target class.
    /// </summary>
    /// <param name=""kernel32.dll"">The kernel 32 dll.</param>
    /// <param name="CharSet">The char set.</param>
    /// <param name="SetLastError">The set last error.</param>
    protected static string? InternalGetTarget(ILogger logger, SafeFileHandle? handle)
    {
        if (handle == null)
        {
            return null;
        }
        var outBufferSize = Marshal.SizeOf(typeof(REPARSE_DATA_BUFFER));
        var outBuffer = Marshal.AllocHGlobal(outBufferSize);
        try
        {
            int bytesReturned;
            var ioResult = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT,
                0, 0, outBuffer, outBufferSize, out bytesReturned, 0);
            if (!ioResult)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode == ERROR_NOT_A_REPARSE_POINT)
                    return null;
                if (ThrowLastWin32Error(logger, errorCode, "UnableToGetInformationAboutJunctionPoint"))
                {
                    return null;
                }
            }
            var reparseDataBuffer = (REPARSE_DATA_BUFFER)
                Marshal.PtrToStructure<REPARSE_DATA_BUFFER>(outBuffer);
            if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT)
                return null;
            var targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);
            if (targetDir.StartsWith(NonInterpretedPathPrefix))
                targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);
            return targetDir;
        }
        finally
        {
            Marshal.FreeHGlobal(outBuffer);
        }
    }
    // \??\
    // This prefix indicates to NTFS that the path is to be treated as a non-interpreted
    // path in the virtual file system.
    /// <summary>
    /// The non interpreted path prefix constant.
    /// </summary>
    protected const string NonInterpretedPathPrefix = @"\??\";
    // Handles a Win32 error. Returns true for access denied (error 5), otherwise logs the error.
    /// <summary>
    /// Throw last win 32 error.
    /// </summary>
    /// <param name="outBuffer">The out buffer.</param>
    /// <returns>The throw last win 32 error result.</returns>
    protected static bool ThrowLastWin32Error(ILogger logger, int errorCode, string message)
    {
        if (errorCode == 5)
        {
            return true;
        }
        logger.LogError(message + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        return false;
    }
    /// <summary>
    /// Represents the e file access type.
    /// </summary>
    [Flags]
    protected enum EFileAccess : uint
    {
        /// <summary>
        /// The generic read value.
        /// </summary>
        GenericRead = 0x80000000,
        /// <summary>
        /// The generic write value.
        /// </summary>
        GenericWrite = 0x40000000,
        /// <summary>
        /// The generic execute value.
        /// </summary>
        GenericExecute = 0x20000000,
        /// <summary>
        /// The generic all value.
        /// </summary>
        GenericAll = 0x10000000
    }
    /// <summary>
    /// Represents the e file share type.
    /// </summary>
    [Flags]
    protected enum EFileShare : uint
    {
        /// <summary>
        /// The none value.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        /// The read value.
        /// </summary>
        Read = 0x00000001,
        /// <summary>
        /// The write value.
        /// </summary>
        Write = 0x00000002,
        /// <summary>
        /// The delete value.
        /// </summary>
        Delete = 0x00000004
    }
    /// <summary>
    /// Represents the e creation disposition type.
    /// </summary>
    protected enum ECreationDisposition : uint
    {
        /// <summary>
        /// The new value.
        /// </summary>
        New = 1,
        /// <summary>
        /// The create always value.
        /// </summary>
        CreateAlways = 2,
        /// <summary>
        /// The open existing value.
        /// </summary>
        OpenExisting = 3,
        /// <summary>
        /// The open always value.
        /// </summary>
        OpenAlways = 4,
        /// <summary>
        /// The truncate existing value.
        /// </summary>
        TruncateExisting = 5
    }
    /// <summary>
    /// Represents the e file attributes type.
    /// </summary>
    [Flags]
    protected enum EFileAttributes : uint
    {
        /// <summary>
        /// The readonly value.
        /// </summary>
        Readonly = 0x00000001,
        /// <summary>
        /// The hidden value.
        /// </summary>
        Hidden = 0x00000002,
        /// <summary>
        /// The system value.
        /// </summary>
        System = 0x00000004,
        /// <summary>
        /// The directory value.
        /// </summary>
        Directory = 0x00000010,
        /// <summary>
        /// The archive value.
        /// </summary>
        Archive = 0x00000020,
        /// <summary>
        /// The device value.
        /// </summary>
        Device = 0x00000040,
        /// <summary>
        /// The normal value.
        /// </summary>
        Normal = 0x00000080,
        /// <summary>
        /// The temporary value.
        /// </summary>
        Temporary = 0x00000100,
        /// <summary>
        /// The sparse file value.
        /// </summary>
        SparseFile = 0x00000200,
        /// <summary>
        /// The reparse point value.
        /// </summary>
        ReparsePoint = 0x00000400,
        /// <summary>
        /// The compressed value.
        /// </summary>
        Compressed = 0x00000800,
        /// <summary>
        /// The offline value.
        /// </summary>
        Offline = 0x00001000,
        /// <summary>
        /// The not content indexed value.
        /// </summary>
        NotContentIndexed = 0x00002000,
        /// <summary>
        /// The encrypted value.
        /// </summary>
        Encrypted = 0x00004000,
        /// <summary>
        /// The write through value.
        /// </summary>
        Write_Through = 0x80000000,
        /// <summary>
        /// The overlapped value.
        /// </summary>
        Overlapped = 0x40000000,
        /// <summary>
        /// The no buffering value.
        /// </summary>
        NoBuffering = 0x20000000,
        /// <summary>
        /// The random access value.
        /// </summary>
        RandomAccess = 0x10000000,
        /// <summary>
        /// The sequential scan value.
        /// </summary>
        SequentialScan = 0x08000000,
        /// <summary>
        /// The delete on close value.
        /// </summary>
        DeleteOnClose = 0x04000000,
        /// <summary>
        /// The backup semantics value.
        /// </summary>
        BackupSemantics = 0x02000000,
        /// <summary>
        /// The posix semantics value.
        /// </summary>
        PosixSemantics = 0x01000000,
        /// <summary>
        /// The open reparse point value.
        /// </summary>
        OpenReparsePoint = 0x00200000,
        /// <summary>
        /// The open no recall value.
        /// </summary>
        OpenNoRecall = 0x00100000,
        /// <summary>
        /// The first pipe instance value.
        /// </summary>
        FirstPipeInstance = 0x00080000
    }
    /// <summary>
    /// Represents the REPARSE DATA BUFFER type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    protected struct REPARSE_DATA_BUFFER
    {
        /// <summary>
        /// The reparse tag.
        /// </summary>
        public uint ReparseTag;
        /// <summary>
        /// The reparse data length.
        /// </summary>
        public ushort ReparseDataLength;
        /// <summary>
        /// The reserved.
        /// </summary>
        public ushort Reserved;
        /// <summary>
        /// Gets the substitute name offset.
        /// </summary>
        public ushort SubstituteNameOffset;
        /// <summary>
        /// The substitute name length.
        /// </summary>
        public ushort SubstituteNameLength;
        /// <summary>
        /// Gets the print name offset.
        /// </summary>
        public ushort PrintNameOffset;
        /// <summary>
        /// The print name length.
        /// </summary>
        public ushort PrintNameLength;
        /// <summary>
        /// The path buffer.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
        public byte[] PathBuffer;
    }
}
