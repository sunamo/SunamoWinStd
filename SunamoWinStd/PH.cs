namespace SunamoWinStd;

/// <summary>
/// Represents the PH type.
/// </summary>
public partial class PH
{
    /// <summary>
    /// Run from path.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="exe">The exe.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="isWithOutput">The is with output.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    /// <returns>The run from path result.</returns>
    public static string? RunFromPath(ILogger logger, string exe, string arguments, bool isWithOutput, bool isThrowingOnError = false)
    {
        PHWin.BreakIfTen();
        var environmentPath = GetMergedPath();
        if (string.IsNullOrWhiteSpace(environmentPath))
        {
            logger.LogWarning("PATH is null");
            return null;
        }

        var paths = environmentPath.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var combinedPaths = paths.Select(pathEntry => Path.Combine(pathEntry.Trim().Trim('"'), exe));
        var existingFiles = combinedPaths.Where(filePath => File.Exists(filePath));
        var exePath = existingFiles.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(exePath))
        {
            if (isWithOutput)
                return RunWithOutput(exePath, arguments);
            Process.Start(exePath, arguments);
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
    /// Merges the process PATH with the PATH from the registry (Machine + User), because the process inherits PATH from its parent and it becomes stale after PATH is changed in Windows.
    /// </summary>
    private static string GetMergedPath()
    {
        var parts = new[]
        {
            Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process),
            Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User),
            Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine)
        };
        var merged = string.Join(";", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
        return Environment.ExpandEnvironmentVariables(merged);
    }

    /// <summary>
    /// Exec cmd.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The exec cmd result.</returns>
    public static bool ExecCmd(string command)
    {
        var isSuccess = ExecCmd(command, out _);
        return isSuccess;
    }

    /// <summary>
    /// Exec cmd.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="output">The output.</param>
    /// <param name="isTransferringEnvVars">The is transferring env vars.</param>
    /// <returns>The exec cmd result.</returns>
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
    /// Run with output.
    /// </summary>
    /// <param name="exe">The exe.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The run with output result.</returns>
    public static string RunWithOutput(string exe, string arguments)
    {
        return RunWithOutput(new ProcessStartInfo { FileName = exe, Arguments = arguments, UseShellExecute = false });
    }

    /// <summary>
    /// Run with output.
    /// </summary>
    /// <param name="processStartInfo">The process start info.</param>
    /// <param name="isTransferringEnvVars">The is transferring env vars.</param>
    /// <returns>The run with output result.</returns>
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
    /// Start.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
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
    /// Start.
    /// </summary>
    /// <param name="exe">The exe.</param>
    /// <param name="arguments">The arguments.</param>
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
    /// Start hidden.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <param name="workingDirectory">The working directory.</param>
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
    /// Normalize uri.
    /// </summary>
    /// <param name="uri">The uri.</param>
    /// <returns>The normalize uri result.</returns>
    public static string NormalizeUri(string uri)
    {
        uri = SHReplace.ReplaceAll(uri, "%22", "\"");
        return uri;
    }

    /// <summary>
    /// Kill process.
    /// </summary>
    /// <param name="process">The process.</param>
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
    /// Terminate.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The terminate result.</returns>
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
    /// Run from path 3.
    /// </summary>
    /// <param name="exe">The exe.</param>
    /// <param name="fileWithoutQm">The file without qm.</param>
    public static void RunFromPath3(string exe, string fileWithoutQm)
    {
        var processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = exe;
        processStartInfo.Arguments = SH.WrapWithQm(fileWithoutQm);
        processStartInfo.UseShellExecute = true;
        Process.Start(processStartInfo);
    }
}
