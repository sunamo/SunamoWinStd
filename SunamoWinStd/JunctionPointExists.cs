namespace SunamoWinStd;

public partial class JunctionPoint
{
    /// <summary>
    ///     Command to get the reparse point data block.
    /// </summary>
    protected const int FSCTL_GET_REPARSE_POINT = 0x000900A8;
    /// <summary>
    ///     The file or directory is not a reparse point.
    /// </summary>
    protected const int ERROR_NOT_A_REPARSE_POINT = 4390;
    /// <summary>
    ///     For normal folder and /H return false
    ///     For junction true
    ///     Determines whether the specified path exists and refers to a junction point.
    /// </summary>
    /// <param name="path">The junction point path</param>
    /// <returns>True if the specified path represents a junction point</returns>
    /// <exception cref="IOException">
    ///     Thrown if the specified path is invalid
    ///     or some other error occurs
    /// </exception>
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
    ///     Cant be use for H
    /// </summary>
    /// <param name="reparsePoint"></param>
    /// <param name="accessMode"></param>
    protected static SafeFileHandle? OpenReparsePoint(ILogger logger, string reparsePoint, EFileAccess accessMode)
    {
        var reparsePointHandle = new SafeFileHandle(CreateFile(reparsePoint, accessMode,
            EFileShare.Read | EFileShare.Write | EFileShare.Delete,
            nint.Zero, ECreationDisposition.OpenExisting,
            EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, nint.Zero), true);
        var err = Marshal.GetLastWin32Error();
        if (err != 0)
            if (ThrowLastWin32Error(logger, err, "UnableToOpenReparsePoint"))
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
    /// <summary>
    ///     Reparse point tag used to identify mount points and junction points.
    /// </summary>
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
            var result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT,
                nint.Zero, 0, outBuffer, outBufferSize, out bytesReturned, nint.Zero);
            if (!result)
            {
                var error = Marshal.GetLastWin32Error();
                if (error == ERROR_NOT_A_REPARSE_POINT)
                    return null;
                if (ThrowLastWin32Error(logger, error, "UnableToGetInformationAboutJunctionPoint"))
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
    /// <summary>
    ///     \??\
    ///     This prefix indicates to NTFS that the path is to be treated as a non-interpreted
    ///     path in the virtual file system.
    /// </summary>
    protected const string NonInterpretedPathPrefix = @"\??\";
    protected static bool ThrowLastWin32Error(ILogger logger, int err, string message)
    {
        if (err == 5)
        {
            /*
Jedná se o tuto chybu:
'UnableToOpenReparsePointSystem.UnauthorizedAccessException: Access is denied. (0x80070005 (E_ACCESSDENIED))'
Nepomůže spustit ani VS jako admin
             */
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
        /// <summary>
        ///     Reparse point tag. Must be a Microsoft reparse point tag.
        /// </summary>
        public uint ReparseTag;
        /// <summary>
        ///     Size, in bytes, of the data after the Reserved member. This can be calculated by:
        ///     (4 * sizeof(ushort)) + SubstituteNameLength + PrintNameLength +
        ///     (namesAreNullTerminated ? 2 * sizeof(char) : 0);
        /// </summary>
        public ushort ReparseDataLength;
        /// <summary>
        ///     Reserved; do not use.
        /// </summary>
        public ushort Reserved;
        /// <summary>
        ///     Offset, in bytes, of the substitute name string in the PathBuffer array.
        /// </summary>
        public ushort SubstituteNameOffset;
        /// <summary>
        ///     Length, in bytes, of the substitute name string. If this string is null-terminated,
        ///     SubstituteNameLength does not include space for the null character.
        /// </summary>
        public ushort SubstituteNameLength;
        /// <summary>
        ///     Offset, in bytes, of the print name string in the PathBuffer array.
        /// </summary>
        public ushort PrintNameOffset;
        /// <summary>
        ///     Length, in bytes, of the print name string. If this string is null-terminated,
        ///     PrintNameLength does not include space for the null character.
        /// </summary>
        public ushort PrintNameLength;
        /// <summary>
        ///     A buffer containing the unicode-encoded path string. The path string contains
        ///     the substitute name string and print name string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
        public byte[] PathBuffer;
    }
}