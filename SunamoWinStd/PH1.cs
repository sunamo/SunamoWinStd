namespace SunamoWinStd;

public partial class PH
{
    public static string RunFromPathBetter(string exe, string arguments)
    {
        var environmentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        var paths = environmentPath.Split(';');
        foreach (var currentPath in paths)
        {
            var currentFile = Path.Combine(currentPath, exe);
            string[] executableExtensions =
            {
                ".exe"
            };
            foreach (var extension in executableExtensions)
            {
                var fullFile = currentFile + extension;
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

        foreach (var currentPath in paths)
        {
            var currentFile = Path.Combine(currentPath, exe);
            try
            {
                if (File.Exists(currentFile))
                {
                    exe = currentFile;
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
        IList<string> processNames = Process.GetProcessesByName(name).Select(process => process.ProcessName).ToList();
        return processNames.Count() > 1;
    }

    public static bool ExistsOnPath(string fileName)
    {
        return GetFullPath(fileName) != null;
    }

    public static string? GetFullPath(string fileName)
    {
        if (File.Exists(fileName))
            return Path.GetFullPath(fileName);
        var environmentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var pathEntry in environmentPath.Split(Path.PathSeparator))
        {
            var fullPath = Path.Combine(pathEntry, fileName);
            if (File.Exists(fullPath))
                return fullPath;
        }

        return null;
    }

    public static List<string> ProcessesWithNameContains(string name)
    {
        var processNames = GetProcessesNames(true);
        var matchingProcesses = processNames.Where(processName => processName.Contains(name.ToLower())).ToList();
        return matchingProcesses;
    }

    public static int TerminateProcessesWithNameContains(string name)
    {
        var matchingProcesses = ProcessesWithNameContains(name);
        var terminatedCount = 0;
        foreach (var item in matchingProcesses)
            terminatedCount += Terminate(item);
        return terminatedCount;
    }

    // Terminates processes with the exact name (without extensions, all lower case).
    public static int TerminateProcessesWithName(string name)
    {
        name = name.ToLower();
        name = Path.GetFileNameWithoutExtension(name);
        var processNames = GetProcessesNames(true);
        var terminatedCount = 0;
        if (processNames.Contains(name))
            terminatedCount += Terminate(name);
        return terminatedCount;
    }

    // Shuts down processes that occupy a file handle. Alternative is FileUtil.WhoIsLocking.
    public static void ShutdownProcessWhichOccupyFileHandleExe(ILogger logger, string fileName)
    {
        var foundProcesses = FindProcessesWhichOccupyFileHandleExe(logger, fileName);
        foreach (var process in foundProcesses)
            KillProcess(process);
    }

    // Finds processes that occupy a file handle using handle64.exe. Alternative is FileUtil.WhoIsLocking.
    public static List<Process> FindProcessesWhichOccupyFileHandleExe(ILogger logger, string fileName, bool isThrowingOnError = true)
    {
        var foundProcesses = new List<Process>();
        var handleProcess = new Process();
        handleProcess.StartInfo.FileName = "handle64.exe";
        handleProcess.StartInfo.Arguments = fileName + " /accepteula";
        handleProcess.StartInfo.UseShellExecute = false;
        handleProcess.StartInfo.RedirectStandardOutput = true;
        handleProcess.StartInfo.WorkingDirectory = @"";
        try
        {
            handleProcess.Start();
        }
        catch (Win32Exception ex)
        {
            logger.LogError(Exceptions.TextOfExceptions(ex));
            if (isThrowingOnError)
            {
                throw;
            }
        }

        handleProcess.WaitForExit();
        string? handleOutput = null;
        try
        {
            handleOutput = handleProcess.StandardOutput.ReadToEnd();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("NoProcessIsAssociatedWithThisObject"))
            {
                ThisApp.Warning("PleaseAddHandle64ExeToPATH");
                return foundProcesses;
            }
        }

        if (handleOutput == null)
        {
            return new List<Process>();
        }

        var pidPattern = @"(?<=\s+piD:\s+)\b(\d+)\b(?=\s+)";
        var matches = Regex.Matches(handleOutput, pidPattern);
        foreach (Match match in matches)
        {
            var process = Process.GetProcessById(int.Parse(match.Value));
            foundProcesses.Add(process);
        }

        return foundProcesses;
    }

    public static List<string> GetProcessesNames(bool isLowerCase)
    {
        var processNames = Process.GetProcesses().Select(process => process.ProcessName).ToList();
        if (isLowerCase)
            CA.ToLower(processNames);
        return processNames;
    }

    internal static void RunVsCode(ILogger logger, string codeExe, string arguments, bool isThrowingOnError, int? openOnLine)
    {
        arguments = SH.WrapWithChar(arguments.TrimEnd('"').TrimStart('"'), '"');

        if (openOnLine != null)
        {
            arguments = $"-g {arguments}:{openOnLine}";
        }

        PH.RunFromPath(logger, codeExe, arguments, false, isThrowingOnError);
    }
}
