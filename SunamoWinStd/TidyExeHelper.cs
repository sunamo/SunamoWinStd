namespace SunamoWinStd;

public class TidyExeHelper
{
    public static Tuple<FileInfo, string> GenerateMapInfo(string mapDirectory, string fileExtension)
    {
        var uniqueMapName = Guid.NewGuid().ToString();
        var fileName = Path.Combine(mapDirectory, Path.ChangeExtension(uniqueMapName, fileExtension));
        return Tuple.Create(new FileInfo(fileName), uniqueMapName);
    }

    public static void WriteToFile(Tuple<FileInfo, string> mapInfo, string text)
    {
        var maxOffset = int.MaxValue / 2;
        var encodedBytes = Encoding.UTF8.GetBytes(text);
        long capacity = encodedBytes.Length + maxOffset;

        using var memoryMappedFile = MemoryMappedFile.CreateFromFile(mapInfo.Item1.FullName, FileMode.Create, mapInfo.Item2,
                   capacity);
        WriteToFile(text, maxOffset, encodedBytes, memoryMappedFile);
    }

    private static void WriteToFile(string text, int maxOffset, byte[] encodedBytes, MemoryMappedFile memoryMappedFile)
    {
        using var accessor = memoryMappedFile.CreateViewAccessor();
        var lengthBytes = BitConverter.GetBytes(text.Length);
        accessor.WriteArray(0, lengthBytes, 0, lengthBytes.Length);
        accessor.WriteArray(maxOffset, encodedBytes, 0, encodedBytes.Length);
    }

    public static
        async Task<string>
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
            await
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
