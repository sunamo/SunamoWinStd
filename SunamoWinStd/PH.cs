namespace SunamoWinStd;

/// <summary>
/// Process helper class for running and managing system processes.
/// </summary>
public partial class PH
{
    /// <summary>
    ///     Runs an executable found on the system PATH.
    ///     Returns standard error or output if everything went well.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostics.</param>
    /// <param name="exe">Executable name to find on PATH.</param>
    /// <param name="arguments">Arguments to pass to the executable.</param>
    /// <param name="isWithOutput">Whether to capture and return output.</param>
    /// <param name="isThrowingOnError">Whether to throw if the executable is not found.</param>
    /// <returns>Process output or null if not found.</returns>
    public static string? RunFromPath(ILogger logger, string exe, string arguments, bool isWithOutput, bool isThrowingOnError = false)
    {
        PHWin.BreakIfTen();
        var environmentPath = Environment.GetEnvironmentVariable("PATH");
        if (environmentPath == null)
        {
            logger.LogWarning("PATH is null");
            return null;
        }

        var paths = environmentPath.Split(';');
        var combinedPaths = paths.Select(pathEntry => Path.Combine(pathEntry, exe));
        var existingFiles = combinedPaths.Where(filePath => File.Exists(filePath));
        var exePath = existingFiles.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(exePath))
        {
            if (isWithOutput)
                return RunWithOutput(exe, arguments);
            Process.Start(exe, arguments);
            return string.Empty;
        }

        logger.LogError(exe + " is not in the path!");
        if (isThrowingOnError)
        {
            throw new Exception(exe + " is not in the path!");
        }

        return null;
    }

    /// <summary>
    /// Executes a command via cmd.exe.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <returns>True if the process completed without output to stderr.</returns>
    public static bool ExecCmd(string command)
    {
        var isSuccess = ExecCmd(command, out _);
        return isSuccess;
    }

    /// <summary>
    ///     Executes command via cmd.exe and captures output.
    /// </summary>
    /// <param name="command">Command to be executed.</param>
    /// <param name="output">Output which the application produced.</param>
    /// <param name="isTransferringEnvVars">True if retaining PATH environment variable from executed command.</param>
    /// <returns>True if process exited with empty error output.</returns>
    public static bool ExecCmd(string command, out string output, bool isTransferringEnvVars = false)
    {
        ProcessStartInfo processStartInfo;
        if (isTransferringEnvVars)
            command = command + " && echo --VARS-- && set";
        processStartInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardOutput = true;
        output = RunWithOutput(processStartInfo, isTransferringEnvVars);
        return string.IsNullOrEmpty(output);
    }

    /// <summary>
    /// Runs an executable and returns its output.
    /// </summary>
    /// <param name="exe">Executable path or name.</param>
    /// <param name="arguments">Arguments to pass.</param>
    /// <returns>Combined standard output and error.</returns>
    public static string RunWithOutput(string exe, string arguments)
    {
        return RunWithOutput(new ProcessStartInfo { FileName = exe, Arguments = arguments, UseShellExecute = false });
    }

    /// <summary>
    /// Runs a process with the given start info and returns its output.
    /// </summary>
    /// <param name="processStartInfo">Process start information.</param>
    /// <param name="isTransferringEnvVars">Whether to transfer environment variables from the process output.</param>
    /// <returns>Combined standard output and error.</returns>
    public static string RunWithOutput(ProcessStartInfo processStartInfo, bool isTransferringEnvVars = false)
    {
        var process = new Process();
        string output = string.Empty;
        processStartInfo.RedirectStandardError = true;
        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.CreateNoWindow = true;
        processStartInfo.UseShellExecute = false;
        var standardOutput = new StringBuilder();
        process.OutputDataReceived += (sender, eventArgs) =>
        {
            standardOutput.AppendLine(eventArgs.Data);
        };
        var standardError = new StringBuilder();
        process.ErrorDataReceived += (sender, eventArgs) =>
        {
            standardError.AppendLine(eventArgs.Data);
        };
        process.StartInfo = processStartInfo;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        output = standardOutput.ToString();
        var error = standardError.ToString();
        if (isTransferringEnvVars)
        {
            var varsRegex = new Regex("--VARS--(.*)", RegexOptions.Singleline);
            var match = varsRegex.Match(output);
            if (match.Success)
            {
                output = varsRegex.Replace(output, "");
                foreach (Match envMatch in new Regex("(.*?)=([^\r]*)", RegexOptions.Multiline).Matches(match.Groups[1].ToString()))
                {
                    var key = envMatch.Groups[1].Value;
                    var value = envMatch.Groups[2].Value;
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        if (error.Length != 0)
            output += error;
        var exitCode = process.ExitCode;
        if (exitCode != 0)
            Console.WriteLine("Error: " + output + "\r\n" + error);
        process.Close();
        return output;
    }

    /// <summary>
    ///     Starts a command via cmd.exe. The executable must be on PATH.
    /// </summary>
    /// <param name="parameter">The command parameter to execute.</param>
    public static void Start(string parameter)
    {
        try
        {
            Process.Start("cmd.exe", "/c " + parameter);
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }
    }

    /// <summary>
    /// Starts an executable with arguments via cmd.exe.
    /// </summary>
    /// <param name="exe">The executable to run.</param>
    /// <param name="arguments">Arguments for the executable.</param>
    public static void Start(string exe, string arguments)
    {
        try
        {
            var commandArguments = "/c " + exe + " " + arguments;
            Process.Start("cmd.exe", commandArguments);
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }
    }

    /// <summary>
    /// Starts a hidden command process in the specified working directory.
    /// </summary>
    /// <param name="parameter">The command parameter to execute.</param>
    /// <param name="workingDirectory">The working directory for the process.</param>
    public static void StartHidden(string parameter, string workingDirectory)
    {
        try
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + parameter;
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }
    }

    /// <summary>
    /// Normalizes a URI by replacing URL-encoded quotes.
    /// </summary>
    /// <param name="uri">The URI to normalize.</param>
    /// <returns>The normalized URI.</returns>
    public static string NormalizeUri(string uri)
    {
        uri = SHReplace.ReplaceAll(uri, "%22", "\"");
        return uri;
    }

    /// <summary>
    /// Kills the specified process.
    /// </summary>
    /// <param name="process">The process to kill.</param>
    public static void KillProcess(Process process)
    {
        try
        {
            process.Kill();
        }
        catch (Exception ex)
        {
            if (!ex.Message.Contains("Access is denied"))
                ThrowEx.CustomWithStackTrace(ex);
        }
    }

    /// <summary>
    /// Terminates all processes with the specified name.
    /// </summary>
    /// <param name="name">The process name to terminate.</param>
    /// <returns>Number of processes terminated.</returns>
    public static int Terminate(string name)
    {
        var terminatedCount = 0;
        foreach (var process in Process.GetProcessesByName(name))
        {
            KillProcess(process);
            terminatedCount++;
        }

        return terminatedCount;
    }

    /// <summary>
    ///     Runs an executable found on PATH. The executable must be on PATH.
    /// </summary>
    /// <param name="exe">The executable name.</param>
    /// <param name="fileWithoutQm">The file path to pass as argument (will be wrapped in quotes).</param>
    public static void RunFromPath3(string exe, string fileWithoutQm)
    {
        var processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = exe;
        processStartInfo.Arguments = SH.WrapWithQm(fileWithoutQm);
        processStartInfo.UseShellExecute = true;
        Process.Start(processStartInfo);
    }
}
