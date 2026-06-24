namespace SunamoWinStd;

public partial class JunctionPoint
{
    protected const int FSCTL_GET_REPARSE_POINT = 0x000900A8;
    protected const int ERROR_NOT_A_REPARSE_POINT = 4390;
    // For normal folder and /H returns false. For junction returns true.
    // Only works on NTFS.
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
    [DllImport("kernel32.dll", SetLastError = true)]
    protected static extern nint CreateFile(
        string lpFileName,
        EFileAccess dwDesiredAccess,
        EFileShare dwShareMode,
        nint lpSecurityAttributes,
        ECreationDisposition dwCreationDisposition,
        EFileAttributes dwFlagsAndAttributes,
        nint hTemplateFile);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    protected static extern bool DeviceIoControl(nint hDevice, uint dwIoControlCode,
        nint InBuffer, int nInBufferSize,
        nint OutBuffer, int nOutBufferSize,
        out int pBytesReturned, nint lpOverlapped);
    protected const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;
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
    protected const string NonInterpretedPathPrefix = @"\??\";
    // Handles a Win32 error. Returns true for access denied (error 5), otherwise logs the error.
    protected static bool ThrowLastWin32Error(ILogger logger, int errorCode, string message)
    {
        if (errorCode == 5)
        {
            return true;
        }
        logger.LogError(message + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
        return false;
    }
    [Flags]
    protected enum EFileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000
    }
    [Flags]
    protected enum EFileShare : uint
    {
        None = 0x00000000,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004
    }
    protected enum ECreationDisposition : uint
    {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5
    }
    [Flags]
    protected enum EFileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }
    [StructLayout(LayoutKind.Sequential)]
    protected struct REPARSE_DATA_BUFFER
    {
        public uint ReparseTag;
        public ushort ReparseDataLength;
        public ushort Reserved;
        public ushort SubstituteNameOffset;
        public ushort SubstituteNameLength;
        public ushort PrintNameOffset;
        public ushort PrintNameLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
        public byte[] PathBuffer;
    }
}
