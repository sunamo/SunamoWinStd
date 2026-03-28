namespace SunamoWinStd;

/// <summary>
///     Provides access to NTFS junction points in .Net.
/// </summary>
public partial class JunctionPoint
{
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
    ///     Command to delete the reparse point data base.
    /// </summary>
    private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;
    /// <summary>
    ///     Creates a hard link (/H) using mklink. Only works for files.
    ///     /H always creates a file link (recognizable in file system).
    ///     If the target exists, it will be overwritten.
    /// </summary>
    /// <param name="source">Source path for the hard link.</param>
    /// <param name="target">Target file path that the link points to.</param>
    /// <returns>The mklink command string.</returns>
    public static
#if ASYNC
        string
#else
    List<string>
#endif
        MklinkH(string source, string target)
    {
        if (!File.Exists(target)) ThrowEx.DirectoryExists(target);
        var command = "cmd /c mklink /H " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);
        return command;
    }
    /// <summary>
    /// Gets paths and targets of all junction points in the specified folder.
    /// </summary>
    /// <param name="folderFrom">Folder to scan for junction points.</param>
    /// <returns>Dictionary mapping junction point paths to their targets.</returns>
    public static Dictionary<string, string> PathsAndTargetsOfAll(string folderFrom)
    {
        var dict = new Dictionary<string, string>();
        var folders = Directory.GetDirectories(folderFrom, "*");
        foreach (var item in folders)
        {
            var target = GetTarget(item);
            if (target == null) target = "(null)";
            if (target != "(null)") dict.Add(item, target);
        }
        return dict;
    }
    /// <summary>
    ///     Creates a junction (/J) using mklink. Only works for directories.
    /// </summary>
    /// <param name="source">Source path for the junction.</param>
    /// <param name="target">Target directory path that the junction points to.</param>
    /// <returns>The mklink command string.</returns>
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
            ThrowEx.DirectoryExists(target);
        }
        var command = "cmd /c mklink /J " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);
        return command;
    }
    /// <summary>
    ///     Creates a directory symbolic link (/D) using mklink. Only works for directories.
    /// </summary>
    /// <param name="source">Source path for the symbolic link.</param>
    /// <param name="target">Target directory path that the link points to.</param>
    /// <returns>The mklink command string.</returns>
    public static
#if ASYNC
        string
#else
    List<string>
#endif
        MklinkD(string source, string target)
    {
        if (!Directory.Exists(target)) ThrowEx.DirectoryExists(target);
        var command = "cmd /c mklink /D " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);
        return command;
    }
    /// <summary>
    ///     For files use mklink, this can be use only for directory.
    ///     Creates a junction point from the specified directory to the specified target directory.
    /// </summary>
    /// <remarks>
    ///     Only works on NTFS.
    /// </remarks>
    /// <param name="logger">Logger instance.</param>
    /// <param name="junctionPoint">The junction point path.</param>
    /// <param name="targetDir">The target directory.</param>
    /// <param name="isOverwriting">If true overwrites an existing reparse point or empty directory.</param>
    /// <exception cref="IOException">
    ///     Thrown when the junction point could not be created or when
    ///     an existing directory was found and <paramref name="isOverwriting" /> is false.
    /// </exception>
    public static void Create(ILogger logger, string junctionPoint, string targetDir, bool isOverwriting)
    {
        targetDir = Path.GetFullPath(targetDir);
        if (!Directory.Exists(targetDir))
            throw new Exception("TargetPathDoesNotExistOrIsNotADirectory");
        if (Directory.Exists(junctionPoint))
        {
            if (!isOverwriting)
                throw new Exception("DirectoryAlreadyExistsAndOverwriteParameterIsFalse");
        }
        else
        {
            Directory.CreateDirectory(junctionPoint);
        }
        using (var handle = OpenReparsePoint(logger, junctionPoint, EFileAccess.GenericWrite))
        {
            if (handle == null)
            {
                return;
            }
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
                var createResult = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT,
                    inBuffer, targetDirBytes.Length + 20, nint.Zero, 0, out bytesReturned, nint.Zero);
                if (!createResult)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    ThrowLastWin32Error(logger, errorCode, "UnableToCreateJunctionPoint");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(inBuffer);
            }
        }
    }
    /// <summary>
    /// Checks if the specified path is a reparse point (junction, symlink, etc.).
    /// </summary>
    /// <param name="path">Path to check.</param>
    /// <returns>True if the path is a reparse point.</returns>
    public static bool IsReparsePoint(string path)
    {
        var reparsePoint = new ReparsePoint(path);
        return !string.IsNullOrEmpty(reparsePoint.Target);
    }
    /// <summary>
    ///     Deletes a junction point at the specified source directory along with the directory itself.
    ///     Does nothing if the junction point does not exist.
    /// </summary>
    /// <remarks>
    ///     Only works on NTFS.
    /// </remarks>
    /// <param name="logger">Logger instance.</param>
    /// <param name="junctionPoint">The junction point path.</param>
    public static void Delete(ILogger logger, string junctionPoint)
    {
        if (!Directory.Exists(junctionPoint))
        {
            if (File.Exists(junctionPoint))
                throw new Exception("PathIsNotAJunctionPoint");
            return;
        }
        using (var handle = OpenReparsePoint(logger, junctionPoint, EFileAccess.GenericWrite))
        {
            if (handle == null)
            {
                return;
            }
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
                var deleteResult = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
                    inBuffer, 8, nint.Zero, 0, out bytesReturned, nint.Zero);
                if (!deleteResult)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                    ThrowLastWin32Error(logger, errorCode, "UnableToDeleteJunctionPoint");
                }
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
                throw new Exception("UnableToDeleteJunctionPoint " + Exceptions.TextOfExceptions(ex));
            }
        }
    }
    /// <summary>
    ///     Gets the target of the specified junction point.
    ///     Works for /j and /d (folders).
    ///     Does not work for /h (file) - see GetTargetTest.
    ///     If the path is not /j, /d, or /h, returns null.
    /// </summary>
    /// <remarks>
    ///     Only works on NTFS.
    /// </remarks>
    /// <param name="path">The junction point or reparse point path.</param>
    /// <returns>The target of the junction point, or null.</returns>
    /// <exception cref="IOException">
    ///     Thrown when the specified path does not
    ///     exist, is invalid, is not a junction point, or some other error occurs.
    /// </exception>
    public static string? GetTarget(string path)
    {
        var reparsePoint = new ReparsePoint(path);
        return reparsePoint.Target;
    }
}
