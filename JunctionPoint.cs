namespace SunamoWinStd;

/// <summary>
///     Provides access to NTFS junction points in .Net.
/// </summary>
public class JunctionPoint : JunctionPointExists
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

        var command = "cmd /c mklink /H " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);

        return command;
    }

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

        var command = "cmd /c mklink /J " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);


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

        var command = "cmd /c mklink /D " + SH.WrapWithQm(source) + " " + SH.WrapWithQm(target);

        return command;
    }





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
                throw new Exception("UnableToDeleteJunctionPoint " + Exceptions.TextOfExceptions(ex));
            }
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
    public static string? GetTarget(string path)
    {
        var p = new ReparsePoint(path);
        return p.Target;
        //using (SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericRead))
        //{
        //    string target = InternalGetTarget(handle);
        //    if (target == null)
        //        throw new Exception(Translate.FromKey(XlfKeys.PathIsNotAJunctionPoint)+".");

        //    return target;
        //}
    }






}