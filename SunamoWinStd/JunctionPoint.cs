namespace SunamoWinStd;

/// <summary>
/// Represents the junction point type.
/// </summary>
public partial class JunctionPoint
{
    private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;
    private const int ERROR_INVALID_REPARSE_DATA = 4392;
    private const int ERROR_REPARSE_TAG_INVALID = 4393;
    private const int ERROR_REPARSE_TAG_MISMATCH = 4394;
    private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;
    private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;
    /// <summary>
    /// Mklink h.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    /// <returns>The mklink h result.</returns>
    public static
        string
        MklinkH(string source, string target)
    {
        if (!File.Exists(target)) ThrowEx.DirectoryExists(target);
        var command = "cmd /c mklink /H " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);
        return command;
    }
    /// <summary>
    /// Paths and targets of all.
    /// </summary>
    /// <param name="folderFrom">The folder from.</param>
    /// <returns>The paths and targets of all result.</returns>
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
    /// Mklink j.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    /// <returns>The mklink j result.</returns>
    public static
        string
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
    /// Mklink d.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="target">The target.</param>
    /// <returns>The mklink d result.</returns>
    public static
        string
        MklinkD(string source, string target)
    {
        if (!Directory.Exists(target)) ThrowEx.DirectoryExists(target);
        var command = "cmd /c mklink /D " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);
        return command;
    }
    // For files use mklink, this can be used only for directory.
    // Creates a junction point from the specified directory to the specified target directory.
    // Only works on NTFS.
    /// <summary>
    /// Create.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="junctionPoint">The junction point.</param>
    /// <param name="targetDir">The target dir.</param>
    /// <param name="isOverwriting">The is overwriting.</param>
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
                    inBuffer, targetDirBytes.Length + 20, 0, 0, out bytesReturned, 0);
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
    /// Is reparse point.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The is reparse point result.</returns>
    public static bool IsReparsePoint(string path)
    {
        if (string.IsNullOrEmpty(path) || path.Length <= 2 || path[1] != ':' || path[2] != '\\')
            return false;
        var reparsePoint = new ReparsePoint(path);
        return !string.IsNullOrEmpty(reparsePoint.Target);
    }
    // Only works on NTFS.
    /// <summary>
    /// Delete.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="junctionPoint">The junction point.</param>
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
                    inBuffer, 8, 0, 0, out bytesReturned, 0);
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
    // Gets the target of the specified junction point.
    // Works for /j and /d (folders). Does not work for /h (file).
    // Only works on NTFS.
    /// <summary>
    /// Get target.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The get target result.</returns>
    public static string? GetTarget(string path)
    {
        var reparsePoint = new ReparsePoint(path);
        return reparsePoint.Target;
    }
}
