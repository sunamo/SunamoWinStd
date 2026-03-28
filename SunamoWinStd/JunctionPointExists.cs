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
    ///     For normal folder and /H returns false.
    ///     For junction returns true.
    ///     Determines whether the specified path exists and refers to a junction point.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="path">The junction point path.</param>
    /// <returns>True if the specified path represents a junction point.</returns>
    /// <exception cref="IOException">
    ///     Thrown if the specified path is invalid
    ///     or some other error occurs.
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
    ///     Opens a reparse point for reading or writing. Cannot be used for /H (hard links).
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="reparsePointPath">Path to the reparse point.</param>
    /// <param name="accessMode">File access mode.</param>
    /// <returns>Safe file handle to the reparse point, or null on error.</returns>
    protected static SafeFileHandle? OpenReparsePoint(ILogger logger, string reparsePointPath, EFileAccess accessMode)
    {
        var reparsePointHandle = new SafeFileHandle(CreateFile(reparsePointPath, accessMode,
            EFileShare.Read | EFileShare.Write | EFileShare.Delete,
            nint.Zero, ECreationDisposition.OpenExisting,
            EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, nint.Zero), true);
        var errorCode = Marshal.GetLastWin32Error();
        if (errorCode != 0)
            if (ThrowLastWin32Error(logger, errorCode, "UnableToOpenReparsePoint"))
            {
                return null;
            }
        return reparsePointHandle;
    }
    /// <summary>
    /// Win32 CreateFile interop for opening reparse points.
    /// </summary>
    /// <param name="lpFileName">File name or path.</param>
    /// <param name="dwDesiredAccess">Desired access flags.</param>
    /// <param name="dwShareMode">Share mode flags.</param>
    /// <param name="lpSecurityAttributes">Security attributes pointer.</param>
    /// <param name="dwCreationDisposition">Creation disposition.</param>
    /// <param name="dwFlagsAndAttributes">Flags and attributes.</param>
    /// <param name="hTemplateFile">Template file handle.</param>
    /// <returns>Handle to the opened file.</returns>
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
    /// Win32 DeviceIoControl interop for reparse point operations.
    /// </summary>
    /// <param name="hDevice">Device handle.</param>
    /// <param name="dwIoControlCode">I/O control code.</param>
    /// <param name="InBuffer">Input buffer pointer.</param>
    /// <param name="nInBufferSize">Input buffer size.</param>
    /// <param name="OutBuffer">Output buffer pointer.</param>
    /// <param name="nOutBufferSize">Output buffer size.</param>
    /// <param name="pBytesReturned">Number of bytes returned.</param>
    /// <param name="lpOverlapped">Overlapped structure pointer.</param>
    /// <returns>True if the operation succeeded.</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    protected static extern bool DeviceIoControl(nint hDevice, uint dwIoControlCode,
        nint InBuffer, int nInBufferSize,
        nint OutBuffer, int nOutBufferSize,
        out int pBytesReturned, nint lpOverlapped);
    /// <summary>
    ///     Reparse point tag used to identify mount points and junction points.
    /// </summary>
    protected const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;
    /// <summary>
    /// Gets the target path from a reparse point handle.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="handle">Safe file handle to the reparse point.</param>
    /// <returns>The target path, or null if not a reparse point.</returns>
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
                nint.Zero, 0, outBuffer, outBufferSize, out bytesReturned, nint.Zero);
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
    /// <summary>
    ///     \??\
    ///     This prefix indicates to NTFS that the path is to be treated as a non-interpreted
    ///     path in the virtual file system.
    /// </summary>
    protected const string NonInterpretedPathPrefix = @"\??\";
    /// <summary>
    /// Handles a Win32 error. Returns true for access denied (error 5), otherwise logs the error.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="errorCode">Win32 error code.</param>
    /// <param name="message">Context message for logging.</param>
    /// <returns>True if the error was access denied (silently handled).</returns>
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
    /// Win32 file access flags.
    /// </summary>
    [Flags]
    protected enum EFileAccess : uint
    {
        /// <summary>Generic read access.</summary>
        GenericRead = 0x80000000,
        /// <summary>Generic write access.</summary>
        GenericWrite = 0x40000000,
        /// <summary>Generic execute access.</summary>
        GenericExecute = 0x20000000,
        /// <summary>Generic all access.</summary>
        GenericAll = 0x10000000
    }
    /// <summary>
    /// Win32 file share mode flags.
    /// </summary>
    [Flags]
    protected enum EFileShare : uint
    {
        /// <summary>No sharing.</summary>
        None = 0x00000000,
        /// <summary>Allow read sharing.</summary>
        Read = 0x00000001,
        /// <summary>Allow write sharing.</summary>
        Write = 0x00000002,
        /// <summary>Allow delete sharing.</summary>
        Delete = 0x00000004
    }
    /// <summary>
    /// Win32 file creation disposition.
    /// </summary>
    protected enum ECreationDisposition : uint
    {
        /// <summary>Create new file, fail if exists.</summary>
        New = 1,
        /// <summary>Create always, overwrite if exists.</summary>
        CreateAlways = 2,
        /// <summary>Open existing file, fail if not exists.</summary>
        OpenExisting = 3,
        /// <summary>Open always, create if not exists.</summary>
        OpenAlways = 4,
        /// <summary>Truncate existing file.</summary>
        TruncateExisting = 5
    }
    /// <summary>
    /// Win32 file attribute flags.
    /// </summary>
    [Flags]
    protected enum EFileAttributes : uint
    {
        /// <summary>Read-only file.</summary>
        Readonly = 0x00000001,
        /// <summary>Hidden file.</summary>
        Hidden = 0x00000002,
        /// <summary>System file.</summary>
        System = 0x00000004,
        /// <summary>Directory.</summary>
        Directory = 0x00000010,
        /// <summary>Archive file.</summary>
        Archive = 0x00000020,
        /// <summary>Device.</summary>
        Device = 0x00000040,
        /// <summary>Normal file.</summary>
        Normal = 0x00000080,
        /// <summary>Temporary file.</summary>
        Temporary = 0x00000100,
        /// <summary>Sparse file.</summary>
        SparseFile = 0x00000200,
        /// <summary>Reparse point.</summary>
        ReparsePoint = 0x00000400,
        /// <summary>Compressed file.</summary>
        Compressed = 0x00000800,
        /// <summary>Offline file.</summary>
        Offline = 0x00001000,
        /// <summary>Not content indexed.</summary>
        NotContentIndexed = 0x00002000,
        /// <summary>Encrypted file.</summary>
        Encrypted = 0x00004000,
        /// <summary>Write-through mode.</summary>
        Write_Through = 0x80000000,
        /// <summary>Overlapped I/O.</summary>
        Overlapped = 0x40000000,
        /// <summary>No buffering.</summary>
        NoBuffering = 0x20000000,
        /// <summary>Random access optimization.</summary>
        RandomAccess = 0x10000000,
        /// <summary>Sequential scan optimization.</summary>
        SequentialScan = 0x08000000,
        /// <summary>Delete on close.</summary>
        DeleteOnClose = 0x04000000,
        /// <summary>Backup semantics (required for directories).</summary>
        BackupSemantics = 0x02000000,
        /// <summary>POSIX semantics.</summary>
        PosixSemantics = 0x01000000,
        /// <summary>Open reparse point.</summary>
        OpenReparsePoint = 0x00200000,
        /// <summary>Open no recall.</summary>
        OpenNoRecall = 0x00100000,
        /// <summary>First pipe instance.</summary>
        FirstPipeInstance = 0x00080000
    }
    /// <summary>
    /// Reparse data buffer structure for NTFS reparse points.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    protected struct REPARSE_DATA_BUFFER
    {
        /// <summary>
        ///     Reparse point tag. Must be a Microsoft reparse point tag.
        /// </summary>
        public uint ReparseTag;
        /// <summary>
        ///     Size, in bytes, of the data after the Reserved member.
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
        ///     Length, in bytes, of the substitute name string.
        /// </summary>
        public ushort SubstituteNameLength;
        /// <summary>
        ///     Offset, in bytes, of the print name string in the PathBuffer array.
        /// </summary>
        public ushort PrintNameOffset;
        /// <summary>
        ///     Length, in bytes, of the print name string.
        /// </summary>
        public ushort PrintNameLength;
        /// <summary>
        ///     A buffer containing the unicode-encoded path string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
        public byte[] PathBuffer;
    }
}
