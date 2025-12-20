// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoWinStd;
public partial class PH
{
    /// <param name = "exe"></param>
    /// <param name = "arguments"></param>
    /// <returns></returns>
    public static string RunFromPathBetter(string exe, string arguments)
    {
        var enviromentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        var paths = enviromentPath.Split(';');
        foreach (var thisPath in paths)
        {
            var thisFile = Path.Combine(thisPath, exe);
            string[] executableExtensions =
            {
                ".exe"
            }; // , ".com", ".bat", ".sh", ".vbs", ".vbscript", ".vbe", ".js", ".rb", ".cmd", ".cpl", ".ws", ".wsf", ".msc", ".gadget"
            foreach (var extension in executableExtensions)
            {
                var fullFile = thisFile + extension;
                try
                {
                    if (File.Exists(fullFile))
                    {
                        exe = fullFile;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ThrowEx.Custom(ex);
                }
            }
        }

        foreach (var thisPath in paths)
        {
            var thisFile = Path.Combine(thisPath, exe);
            try
            {
                if (File.Exists(thisFile))
                {
                    exe = thisFile;
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.TextOfExceptions(ex));
            }
        }

        return RunWithOutput(exe, arguments);
    }

    public static bool IsAlreadyRunning(string name)
    {
        IList<string> pr = Process.GetProcessesByName(name).Select(d => d.ProcessName).ToList();
        //var processes = Process.GetProcesses(name).Where(text => text.ProcessName.Contains(name)).Select(d => d.ProcessName);
        return pr.Count() > 1;
    }

    public static bool ExistsOnPath(string fileName)
    {
        return GetFullPath(fileName) != null;
    }

    public static string? GetFullPath(string fileName)
    {
        if (File.Exists(fileName))
            return Path.GetFullPath(fileName);
        var values = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var path in values.Split(Path.PathSeparator))
        {
            var fullPath = Path.Combine(path, fileName);
            if (File.Exists(fullPath))
                return fullPath;
        }

        return null;
    }

    public static List<string> ProcessesWithNameContains(string name)
    {
        var processes = GetProcessesNames(true);
        var text = processes.Where(d => d.Contains(name.ToLower())).ToList(); //CA.ReturnWhichContains(processes, name.ToLower());
        return text;
    }

    public static int TerminateProcessesWithNameContains(string name)
    {
        var text = ProcessesWithNameContains(name);
        var ended = 0;
        foreach (var item in text)
            ended += Terminate(item);
        return ended;
    }

    /// <summary>
    ///     without extensions and all lower
    /// </summary>
    /// <param name = "name"></param>
    /// <returns></returns>
    public static int TerminateProcessesWithName(string name)
    {
        name = name.ToLower();
        name = Path.GetFileNameWithoutExtension(name);
        var processes = GetProcessesNames(true);
        var ended = 0;
        if (processes.Contains(name))
            ended += Terminate(name);
        return ended;
    }

    /// <summary>
    ///     Alternative is FileUtil.WhoIsLocking
    /// </summary>
    /// <param name = "fileName"></param>
    public static void ShutdownProcessWhichOccupyFileHandleExe(ILogger logger, string fileName)
    {
        var pr2 = FindProcessesWhichOccupyFileHandleExe(logger, fileName);
        foreach (var pr in pr2)
            KillProcess(pr);
    }

    /// <summary>
    ///     Alternative is FileUtil.WhoIsLocking
    ///     A2 must be even is not used
    /// </summary>
    /// <param name = "fileName"></param>
    /// <returns></returns>
    public static List<Process> FindProcessesWhichOccupyFileHandleExe(ILogger logger, string fileName, bool throwEx = true)
    {
        var pr2 = new List<Process>();
        var tool = new Process();
        tool.StartInfo.FileName = "handle64.exe";
        tool.StartInfo.Arguments = fileName + " /accepteula";
        tool.StartInfo.UseShellExecute = false;
        tool.StartInfo.RedirectStandardOutput = true;
        tool.StartInfo.WorkingDirectory = @"";
        try
        {
            tool.Start();
        }
        catch (Win32Exception ex)
        {
            logger.LogError(Exceptions.TextOfExceptions(ex));
            if (throwEx)
            {
                throw ex;
            }
        }

        tool.WaitForExit();
        string? outputTool = null;
        try
        {
            outputTool = tool.StandardOutput.ReadToEnd();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("NoProcessIsAssociatedWithThisObject"))
            {
                ThisApp.Warning("PleaseAddHandle64ExeToPATH");
                return pr2;
            }
        }

        if (outputTool == null)
        {
            return new List<Process>();
        }

        var matchPattern = @"(?<=\s+piD:\s+)\b(\d+)\b(?=\s+)";
        var matches = Regex.Matches(outputTool, matchPattern);
        foreach (Match match in matches)
        {
            var pr = Process.GetProcessById(int.Parse(match.Value));
            pr2.Add(pr);
        }

        return pr2;
    }

    //public static void StartAllUri(List<string> all)
    //{
    //    foreach (var item in all) Uri(UH.AppendHttpIfNotExists(item));
    //}
    public static List<string> GetProcessesNames(bool lower)
    {
        var parameter = Process.GetProcesses().Select(d => d.ProcessName).ToList();
        if (lower)
            CA.ToLower(parameter);
        return parameter;
    }

    ///// <summary>
    /////     For search one term in all uris use UriWebServices.SearchInAll
    ///// </summary>
    ///// <param name="carModels"></param>
    ///// <param name="v"></param>
    //public static void StartAllUri(List<string> carModels)
    //{
    //    PHWin.AddBrowser();
    //    foreach (var item in carModels)
    //    {
    //        PHWin.OpenInBrowser(item);
    //    }
    //}
    //public static void StartAllUri(List<string> carModels, Func<string, string> spritMonitor)
    //{
    //    carModels = CAChangeContent.ChangeContent0(null, carModels, spritMonitor);
    //    carModels = CAChangeContent.ChangeContent0(null, carModels, NormalizeUri);
    //    StartAllUri(carModels);
    //}
    /// <summary>
    ///     Start all uri in clipboard, splitted by whitespace
    /// </summary>
     //public static void StartAllUri(string text)
    //{
    //    //var text = ClipboardHelper.GetText();
    //    var uris = SHSplit.SplitByWhiteSpaces(text);
    //    StartAllUri(uris);
    //}
    internal static void RunVsCode(ILogger logger, string codeExe, string arguments, bool throwExWhenError, int? openOnLine)
    {
        if (openOnLine != null)
        {
            arguments = $"-g {arguments}:{openOnLine}";
        }

        PH.RunFromPath(logger, codeExe, arguments, false, throwExWhenError);
    }
}