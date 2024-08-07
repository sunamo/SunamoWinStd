namespace SunamoWinStd;

public class TidyExeHelper
{
    //public static object FS { get; private set; }


    public static Tuple<FileInfo, string> GenerateMapInfo(string mapDirectory, string fileExtension)
    {
        var uniqueMapName = Guid.NewGuid().ToString();
        var fileName = Path.Combine(mapDirectory, Path.ChangeExtension(uniqueMapName, fileExtension));
        return Tuple.Create(new FileInfo(fileName), uniqueMapName);
    }

    public static void WriteToFile(Tuple<FileInfo, string> mapInfo, string input)
    {
        var max = int.MaxValue / 2;
        var newValue = Encoding.UTF8.GetBytes(input);
        long capacity = newValue.Length + max;

        using (var mmf = MemoryMappedFile.CreateFromFile(mapInfo.Item1.FullName, FileMode.Create, mapInfo.Item2,
                   capacity))
        {
            WriteToFile(input, max, newValue, mmf);
        }
    }

    private static void WriteToFile(string value, int max, byte[] newValue, MemoryMappedFile mmf)
    {
        using (var accesor = mmf.CreateViewAccessor())
        {
            var newValueLength = BitConverter.GetBytes(value.Length);
            accesor.WriteArray(0, newValueLength, 0, newValueLength.Length);
            accesor.WriteArray(max, newValue, 0, newValue.Length);
        }
    }

    public static
#if ASYNC
        async Task<string>
#else
    string
#endif
        FormatHtml(string input, string tidy_config,
            Func<List<string>, Task<List<List<string>>>> powershellRunnerInvoke)
    {
        var mapInfo = GenerateMapInfo(Path.GetTempPath(), ".txt");
        //WriteToFile(mapInfo, "abc");

        //var random = RandomHelper.RandomString(5, false, false, true, false);
        var temp = Path.GetTempFileName();
        MemoryMappedFile m = null;
        int max, capacity;
        max = capacity = 1024 * 1024 * 2;

        var mapName = mapInfo.Item2;

        //var max = int.MaxValue / 2;
        var newValue = Encoding.UTF8.GetBytes(input);
        capacity = newValue.Length + max;

        //mapName  cant be identical like path
        //m = MemoryMappedFile.CreateFromFile(new FileStream( temp, FileMode.OpenOrCreate), temp, , MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, true); 
        //m =  MemoryMappedFile.CreateFromFile(temp, FileMode.OpenOrCreate, mapName , capacity, MemoryMappedFileAccess.ReadWrite);
        //m = MemoryMappedFile.CreateNew(mapName, capacity);
        m = MemoryMappedFile.CreateFromFile(mapInfo.Item1.FullName, FileMode.Create, mapInfo.Item2, capacity);

        WriteToFile(input, max, newValue, m);

        //PowershellRunner ps = new PowershellRunner();
        var comment = "tidy -config " + SH.WrapWithQm(tidy_config) + " -output " + SH.WrapWithQm(mapName) + " " +
                      SH.WrapWithQm(mapName);
        //comment = "tidy -config \"D:\\pa\\tidy\\tidy_config.txt\" -output \"D:\\pa\\tidy\\1_Out.html\" \"D:\\pa\\tidy\\1.html\"";
        var result =
#if ASYNC
            await
#endif
                powershellRunnerInvoke(new List<string>([comment]));

        if (result[0].Count > 0)
        {
        }


        string output = null;

        using (var accesor = m.CreateViewStream())
        {
            var reader = new StreamReader(accesor);
            var text = reader.ReadToEnd();
            output = text;
        }

        m.Dispose();

        return output;
    }
}