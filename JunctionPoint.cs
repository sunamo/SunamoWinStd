namespace SunamoWinStd;

/// <summary>
///     Provides access to NTFS junction points in .Net.
/// </summary>
public static class JunctionPoint
{
    /// <summary>
    ///     The file or directory is not a reparse point.
    /// </summary>
    private const int ERROR_NOT_A_REPARSE_POINT = 4390;

    /// <summary>
    ///     The reparse point attribute cannot be set because it conflicts with an existing attribute.
    /// </summary>
    private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

    /// <summary>
    ///     The data present in the reparse point buffer is invalid.
    /// </summary>
    private const int ERROR_INVALID_REPARSE_DATA = 4392;

    /// <summary>
    ///     The tag present in the reparse point buffer is invalid.
    /// </summary>
    private const int ERROR_REPARSE_TAG_INVALID = 4393;

    /// <summary>
    ///     There is a mismatch between the tag specified in the request and the tag present in the reparse point.
    /// </summary>
    private const int ERROR_REPARSE_TAG_MISMATCH = 4394;

    /// <summary>
    ///     Command to set the reparse point data block.
    /// </summary>
    private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

    /// <summary>
    ///     Command to get the reparse point data block.
    /// </summary>
    private const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

    /// <summary>
    ///     Command to delete the reparse point data base.
    /// </summary>
    private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

    /// <summary>
    ///     Reparse point tag used to identify mount points and junction points.
    /// </summary>
    private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

    /// <summary>
    ///     \??\
    ///     This prefix indicates to NTFS that the path is to be treated as a non-interpreted
    ///     path in the virtual file system.
    /// </summary>
    private const string NonInterpretedPathPrefix = @"\??\";

    private static Type type = typeof(JunctionPoint);

    /// <summary>
    ///     /H = Only files
    ///     If exists, will rewrite.
    ///     /J vytváří vždy adresář, jde pak dle toho poznat i ve FS
    ///     /H pracuje adekvátně se soubory
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static
#if ASYNC
        string
#else
    List<string>
#endif
        MklinkH(string source, string target)
    {
        if (!File.Exists(target)) ThrowEx.DirectoryExists(target);

        var command = "cmd /c mklink /H " + SH.WrapWithQm(source) + AllStrings.space + SH.WrapWithQm(target);

        return command;
    }

    public static Dictionary<string, string> PathsAndTargetsOfAll(string folderFrom)
    {
        var dict = new Dictionary<string, string>();

        var folders = Directory.GetDirectories(folderFrom, AllStrings.asterisk);
        foreach (var item in folders)
        {
            var target = GetTarget(item);
            if (target == null) target = Consts.nulled;
            if (target != Consts.nulled) dict.Add(item, target);
        }

        return dict;
    }

    /// <summary>
    ///     Only folders
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static
#if ASYNC
        string
#else
    List<string>
#endif
        MklinkJ(string source, string target)
    {
        if (!Directory.Exists(target))
        {
            var f = File.Exists(Path.Combine(target, "_.txt"));
            ThrowEx.DirectoryExists(target);
        }

        var command = "cmd /c mklink /J " + SH.WrapWithQm(source) + AllStrings.space + SH.WrapWithQm(target);


        return command;
    }

    /// <summary>
    ///     Only folders
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public static
#if ASYNC
        string
#else
    List<string>
#endif
        MklinkD(string source, string target)
    {
        if (!Directory.Exists(target)) ThrowEx.DirectoryExists(target);

        var command = "cmd /c mklink /D " + SH.WrapWithQm(source) + AllStrings.space + SH.WrapWithQm(target);

        return command;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(nint hDevice, uint dwIoControlCode,
        nint InBuffer, int nInBufferSize,
        nint OutBuffer, int nOutBufferSize,
        out int pBytesReturned, nint lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint CreateFile(
        string lpFileName,
        EFileAccess dwDesiredAccess,
        EFileShare dwShareMode,
        nint lpSecurityAttributes,
        ECreationDisposition dwCreationDisposition,
        EFileAttributes dwFlagsAndAttributes,
        nint hTemplateFile);

    /// <summary>
    ///     For files use mklink, this can be use only for directory
    ///     Creates a junction point from the specified directory to the specified target directory.
    /// </summary>
    /// <remarks>
    ///     Only works on NTFS.
    /// </remarks>
    /// <param name="junctionPoint">The junction point path</param>
    /// <param name="targetDir">The target directory</param>
    /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
    /// <exception cref="IOException">
    ///     Thrown when the junction point could not be created or when
    ///     an existing directory was found and <paramref name="overwrite" /> if false
    /// </exception>
    public static void Create(string junctionPoint, string targetDir, bool overwrite)
    {
        targetDir = Path.GetFullPath(targetDir);

        if (!Directory.Exists(targetDir))
            throw new Exception("TargetPathDoesNotExistOrIsNotADirectory");

        if (Directory.Exists(junctionPoint))
        {
            if (!overwrite)
                throw new Exception("DirectoryAlreadyExistsAndOverwriteParameterIsFalse");
        }
        else
        {
            Directory.CreateDirectory(junctionPoint);
        }

        using (var handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite))
        {
            var targetDirBytes = Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + Path.GetFullPath(targetDir));

            var reparseDataBuffer = new REPARSE_DATA_BUFFER();

            reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
            reparseDataBuffer.ReparseDataLength = (ushort)(targetDirBytes.Length + 12);
            reparseDataBuffer.SubstituteNameOffset = 0;
            reparseDataBuffer.SubstituteNameLength = (ushort)targetDirBytes.Length;
            reparseDataBuffer.PrintNameOffset = (ushort)(targetDirBytes.Length + 2);
            reparseDataBuffer.PrintNameLength = 0;
            reparseDataBuffer.PathBuffer = new byte[0x3ff0];
            Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

            var inBufferSize = Marshal.SizeOf(reparseDataBuffer);
            var inBuffer = Marshal.AllocHGlobal(inBufferSize);

            try
            {
                Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                int bytesReturned;
                var result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT,
                    inBuffer, targetDirBytes.Length + 20, nint.Zero, 0, out bytesReturned, nint.Zero);

                if (!result)
                    ThrowLastWin32Error("UnableToCreateJunctionPoint");
            }
            finally
            {
                Marshal.FreeHGlobal(inBuffer);
            }
        }
    }

    public static bool IsReparsePoint(string path)
    {
        var p = new ReparsePoint(path);
        return !string.IsNullOrEmpty(p.Target);
    }

    /// Deletes a junction point at the specified source directory along with the directory itself.
    /// Does nothing if the junction point does not exist.
    /// </summary>
    /// <remarks>
    ///     Only works on NTFS.
    /// </remarks>
    /// <param name="junctionPoint">The junction point path</param>
    public static void Delete(string junctionPoint)
    {
        if (!Directory.Exists(junctionPoint))
        {
            if (File.Exists(junctionPoint))
                throw new Exception("PathIsNotAJunctionPoint");

            return;
        }

        using (var handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite))
        {
            var reparseDataBuffer = new REPARSE_DATA_BUFFER();

            reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
            reparseDataBuffer.ReparseDataLength = 0;
            reparseDataBuffer.PathBuffer = new byte[0x3ff0];

            var inBufferSize = Marshal.SizeOf(reparseDataBuffer);
            var inBuffer = Marshal.AllocHGlobal(inBufferSize);
            try
            {
                Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

                int bytesReturned;
                var result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
                    inBuffer, 8, nint.Zero, 0, out bytesReturned, nint.Zero);

                if (!result)
                    ThrowLastWin32Error("UnableToDeleteJunctionPoint");
            }
            finally
            {
                Marshal.FreeHGlobal(inBuffer);
            }

            try
            {
                Directory.Delete(junctionPoint);
            }
            catch (IOException ex)
            {
                throw new Exception("UnableToDeleteJunctionPoint");
            }
        }
    }

    public static bool IsJunctionPoint(string path)
    {
        return Exists(path);
    }

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
    public static bool Exists(string path)
    {
        if (!Directory.Exists(path))
            return false;

        using (var handle = OpenReparsePoint(path, EFileAccess.GenericRead))
        {
            var target = InternalGetTarget(handle);
            return target != null;
        }
    }

    /// <summary>
    ///     Gets the target of the specified junction point.
    ///     Is working for /j,/d (folders)
    ///     Is not working for /h (file) - see GetTargetTest
    ///     If A1 is not /j,/d,/h, return null
    /// </summary>
    /// <remarks>
    ///     Only works on NTFS.
    /// </remarks>
    /// <param name="junctionPoint">The junction point path</param>
    /// <returns>The target of the junction point</returns>
    /// <exception cref="IOException">
    ///     Thrown when the specified path does not
    ///     exist, is invalid, is not a junction point, or some other error occurs
    /// </exception>
    public static string GetTarget(string path)
    {
        var p = new ReparsePoint(path);
        return p.Target;
        //using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericRead))
        //{
        //    string target = InternalGetTarget(handle);
        //    if (target == null)
        //        throw new Exception(sess.i18n(XlfKeys.PathIsNotAJunctionPoint)+".");

        //    return target;
        //}
    }

    private static string InternalGetTarget(SafeFileHandle handle)
    {
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

                ThrowLastWin32Error("UnableToGetInformationAboutJunctionPoint");
            }

            var reparseDataBuffer = (REPARSE_DATA_BUFFER)
                Marshal.PtrToStructure(outBuffer, typeof(REPARSE_DATA_BUFFER));

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
    ///     Cant be use for H
    /// </summary>
    /// <param name="reparsePoint"></param>
    /// <param name="accessMode"></param>
    private static SafeFileHandle OpenReparsePoint(string reparsePoint, EFileAccess accessMode)
    {
        var reparsePointHandle = new SafeFileHandle(CreateFile(reparsePoint, accessMode,
            EFileShare.Read | EFileShare.Write | EFileShare.Delete,
            nint.Zero, ECreationDisposition.OpenExisting,
            EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, nint.Zero), true);

        if (Marshal.GetLastWin32Error() != 0)
            ThrowLastWin32Error("UnableToOpenReparsePoint");

        return reparsePointHandle;
    }

    private static void ThrowLastWin32Error(string message)
    {
        throw new Exception(message + Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
    }

    [Flags]
    private enum EFileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000
    }

    [Flags]
    private enum EFileShare : uint
    {
        None = 0x00000000,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004
    }

    private enum ECreationDisposition : uint
    {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5
    }

    [Flags]
    private enum EFileAttributes : uint
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
    private struct REPARSE_DATA_BUFFER
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