namespace SunamoWinStd;

/// <summary>
/// Extended process helper with additional search and termination methods.
/// </summary>
public partial class PH
{
    /// <summary>
    /// Runs an executable from the system PATH with better extension detection.
    /// </summary>
    /// <param name="exe">The executable name.</param>
    /// <param name="arguments">Arguments to pass.</param>
    /// <returns>Process output.</returns>
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

    /// <summary>
    /// Checks if a process with the given name is already running (more than one instance).
    /// </summary>
    /// <param name="name">The process name to check.</param>
    /// <returns>True if more than one instance is running.</returns>
    public static bool IsAlreadyRunning(string name)
    {
        IList<string> processNames = Process.GetProcessesByName(name).Select(process => process.ProcessName).ToList();
        return processNames.Count() > 1;
    }

    /// <summary>
    /// Checks if an executable exists on the system PATH.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if the file exists on PATH.</returns>
    public static bool ExistsOnPath(string fileName)
    {
        return GetFullPath(fileName) != null;
    }

    /// <summary>
    /// Gets the full path of a file by searching the system PATH.
    /// </summary>
    /// <param name="fileName">The file name to find.</param>
    /// <returns>Full path if found, null otherwise.</returns>
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

    /// <summary>
    /// Returns names of processes whose name contains the specified string.
    /// </summary>
    /// <param name="name">The substring to search for in process names.</param>
    /// <returns>List of matching process names.</returns>
    public static List<string> ProcessesWithNameContains(string name)
    {
        var processNames = GetProcessesNames(true);
        var matchingProcesses = processNames.Where(processName => processName.Contains(name.ToLower())).ToList();
        return matchingProcesses;
    }

    /// <summary>
    /// Terminates all processes whose name contains the specified string.
    /// </summary>
    /// <param name="name">The substring to search for in process names.</param>
    /// <returns>Number of processes terminated.</returns>
    public static int TerminateProcessesWithNameContains(string name)
    {
        var matchingProcesses = ProcessesWithNameContains(name);
        var terminatedCount = 0;
        foreach (var item in matchingProcesses)
            terminatedCount += Terminate(item);
        return terminatedCount;
    }

    /// <summary>
    ///     Terminates processes with the exact name (without extensions, all lower case).
    /// </summary>
    /// <param name="name">The process name to terminate.</param>
    /// <returns>Number of processes terminated.</returns>
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

    /// <summary>
    ///     Shuts down processes that occupy a file handle. Alternative is FileUtil.WhoIsLocking.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="fileName">The file name to check.</param>
    public static void ShutdownProcessWhichOccupyFileHandleExe(ILogger logger, string fileName)
    {
        var foundProcesses = FindProcessesWhichOccupyFileHandleExe(logger, fileName);
        foreach (var process in foundProcesses)
            KillProcess(process);
    }

    /// <summary>
    ///     Finds processes that occupy a file handle using handle64.exe. Alternative is FileUtil.WhoIsLocking.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="fileName">The file name to check.</param>
    /// <param name="isThrowingOnError">Whether to throw if handle64.exe fails.</param>
    /// <returns>List of processes occupying the file handle.</returns>
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

    /// <summary>
    /// Gets names of all running processes.
    /// </summary>
    /// <param name="isLowerCase">Whether to convert names to lower case.</param>
    /// <returns>List of process names.</returns>
    public static List<string> GetProcessesNames(bool isLowerCase)
    {
        var processNames = Process.GetProcesses().Select(process => process.ProcessName).ToList();
        if (isLowerCase)
            CA.ToLower(processNames);
        return processNames;
    }

    /// <summary>
    /// Runs VS Code (or compatible editor) with a file path, opening at a specific line if specified.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="codeExe">The VS Code executable name.</param>
    /// <param name="arguments">The file path to open.</param>
    /// <param name="isThrowingOnError">Whether to throw on error.</param>
    /// <param name="openOnLine">Line number to open at.</param>
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
