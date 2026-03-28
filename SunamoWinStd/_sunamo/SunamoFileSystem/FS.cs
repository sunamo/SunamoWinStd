namespace SunamoWinStd._sunamo.SunamoFileSystem;

/// <summary>
/// Internal file system utility methods.
/// </summary>
internal class FS
{
    /// <summary>
    /// Tries to delete a file. Returns true if successful, false otherwise.
    /// </summary>
    /// <param name="filePath">The path of the file to delete.</param>
    /// <returns>True if the file was deleted successfully.</returns>
    internal static bool TryDeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("File cannot be deleted: " + filePath + " " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Creates parent directories for the specified path if they do not exist.
    /// </summary>
    /// <param name="path">The path whose parent directories should be created.</param>
    internal static void CreateUpfoldersPhysicallyUnlessThere(string path)
    {
        CreateFoldersPhysicallyUnlessThere(Path.GetDirectoryName(path));
    }

    /// <summary>
    /// Creates the specified directory and all parent directories if they do not exist.
    /// </summary>
    /// <param name="path">The directory path to create.</param>
    internal static void CreateFoldersPhysicallyUnlessThere(string? path)
    {
        ThrowEx.IsNullOrEmpty("path", path!);

        if (Directory.Exists(path))
        {
            return;
        }

        List<string> foldersToCreate = new List<string>
        {
            path!
        };

        while (true)
        {
            path = Path.GetDirectoryName(path);

            if (Directory.Exists(path))
            {
                break;
            }

            foldersToCreate.Add(path!);
        }

        foldersToCreate.Reverse();
        foreach (string item in foldersToCreate)
        {
            if (!Directory.Exists(item))
            {
                Directory.CreateDirectory(item);
            }
        }
    }

    /// <summary>
    /// Ensures the path ends with a backslash and has an uppercase first character.
    /// Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="path">The path to ensure ends with a backslash.</param>
    /// <returns>The path with a trailing backslash.</returns>
    internal static string WithEndSlash(ref string path)
    {
        if (path != string.Empty)
        {
            path = path.TrimEnd('\\') + '\\';
        }

        SH.FirstCharUpper(ref path);
        return path;
    }
}
