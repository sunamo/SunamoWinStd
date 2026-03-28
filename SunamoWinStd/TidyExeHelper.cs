namespace SunamoWinStd;

/// <summary>
/// Helper for formatting HTML using the tidy executable via memory-mapped files.
/// </summary>
public class TidyExeHelper
{
    /// <summary>
    /// Generates a unique map info tuple with file info and map name.
    /// </summary>
    /// <param name="mapDirectory">Directory for the map file.</param>
    /// <param name="fileExtension">File extension for the map file.</param>
    /// <returns>Tuple of FileInfo and unique map name.</returns>
    public static Tuple<FileInfo, string> GenerateMapInfo(string mapDirectory, string fileExtension)
    {
        var uniqueMapName = Guid.NewGuid().ToString();
        var fileName = Path.Combine(mapDirectory, Path.ChangeExtension(uniqueMapName, fileExtension));
        return Tuple.Create(new FileInfo(fileName), uniqueMapName);
    }

    /// <summary>
    /// Writes input text to a memory-mapped file.
    /// </summary>
    /// <param name="mapInfo">Map info tuple containing file info and map name.</param>
    /// <param name="text">Text to write.</param>
    public static void WriteToFile(Tuple<FileInfo, string> mapInfo, string text)
    {
        var maxOffset = int.MaxValue / 2;
        var encodedBytes = Encoding.UTF8.GetBytes(text);
        long capacity = encodedBytes.Length + maxOffset;

        using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(mapInfo.Item1.FullName, FileMode.Create, mapInfo.Item2,
                   capacity))
        {
            WriteToFile(text, maxOffset, encodedBytes, memoryMappedFile);
        }
    }

    private static void WriteToFile(string text, int maxOffset, byte[] encodedBytes, MemoryMappedFile memoryMappedFile)
    {
        using (var accessor = memoryMappedFile.CreateViewAccessor())
        {
            var lengthBytes = BitConverter.GetBytes(text.Length);
            accessor.WriteArray(0, lengthBytes, 0, lengthBytes.Length);
            accessor.WriteArray(maxOffset, encodedBytes, 0, encodedBytes.Length);
        }
    }

    /// <summary>
    /// Formats HTML content using the tidy executable via PowerShell.
    /// </summary>
    /// <param name="text">HTML content to format.</param>
    /// <param name="tidyConfigPath">Path to the tidy configuration file.</param>
    /// <param name="powershellRunnerInvoke">Function to invoke PowerShell commands.</param>
    /// <returns>Formatted HTML content.</returns>
    public static
#if ASYNC
        async Task<string>
#else
    string
#endif
        FormatHtml(string text, string tidyConfigPath,
            Func<List<string>, Task<List<List<string>>>> powershellRunnerInvoke)
    {
        var mapInfo = GenerateMapInfo(Path.GetTempPath(), ".txt");

        int maxOffset, capacity;
        maxOffset = capacity = 1024 * 1024 * 2;

        var mapName = mapInfo.Item2;

        var encodedBytes = Encoding.UTF8.GetBytes(text);
        capacity = encodedBytes.Length + maxOffset;

        var memoryMappedFile = MemoryMappedFile.CreateFromFile(mapInfo.Item1.FullName, FileMode.Create, mapInfo.Item2, capacity);

        WriteToFile(text, maxOffset, encodedBytes, memoryMappedFile);

        var command = "tidy -config " + SH.WrapWithQm(tidyConfigPath) + " -output " + SH.WrapWithQm(mapName) + " " +
                      SH.WrapWithQm(mapName);
        var commandResult =
#if ASYNC
            await
#endif
                powershellRunnerInvoke(new List<string>([command]));


        string? output = null;

        using (var viewStream = memoryMappedFile.CreateViewStream())
        {
            var reader = new StreamReader(viewStream);
            var readText = reader.ReadToEnd();
            output = readText;
        }

        memoryMappedFile.Dispose();

        return output;
    }
}
