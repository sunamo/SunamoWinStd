// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoWinStd;
public partial class PH
{
    private static Type type = typeof(PH);
    /// <summary>
    ///     https://stackoverflow.com/a/12393522
    ///     Return SE or output if everything gone good
    /// </summary>
    public static string? RunFromPath(ILogger logger, string exe, string arguments, bool withOutput, bool throwExWhenError = false)
    {
        PHWin.BreakIfTen();
        var enviromentPath = Environment.GetEnvironmentVariable("PATH");
        if (enviromentPath == null)
        {
            logger.LogWarning("PATH is null");
            return null;
        }

        var paths = enviromentPath.Split(';'); // SHSplit.SplitChar(enviromentPath, ';');
#if DEBUG
        var wc = paths.Where(d => d.Contains("Microsoft VS Code Insiders"));
        paths.Reverse();
#endif
        var paths2 = paths.Select(x => Path.Combine(x, exe));
        var files = paths2.Where(x => File.Exists(x));
        var fi = files.FirstOrDefault();
        var exePath = fi;
        if (!string.IsNullOrWhiteSpace(exePath))
        {
            if (withOutput)
                return RunWithOutput(exe, arguments);
            Process.Start(exe, arguments);
            return string.Empty;
        }

        logger.LogError(exe + " is not in the path!");
        if (throwExWhenError)
        {
            throw new Exception(exe + " is not in the path!");
        }

        return null;
    }

    public static bool ExecCmd(string cmd)
    {
        string output;
        var builder = ExecCmd(cmd, out output);
        return builder;
    }

    /// <summary>
    ///     Executes command
    /// </summary>
    /// <param name = "cmd">command to be executed</param>
    /// <param name = "output">output which application produced</param>
    /// <param name = "transferEnvVars">true - if retain PATH environment variable from executed command</param>
    /// <returns>true if process exited with code 0</returns>
    public static bool ExecCmd(string cmd, out string output, bool transferEnvVars = false)
    {
        ProcessStartInfo processInfo;
        if (transferEnvVars)
            cmd = cmd + " && echo --VARS-- && set";
        processInfo = new ProcessStartInfo("cmd.exe", "/c " + cmd);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;
        output = RunWithOutput(processInfo, transferEnvVars);
        return string.IsNullOrEmpty(output);
    }

    public static string RunWithOutput(string exe, string arguments)
    {
        return RunWithOutput(new ProcessStartInfo { FileName = exe, Arguments = arguments, UseShellExecute = false });
    }

    public static string RunWithOutput(ProcessStartInfo processInfo, bool transferEnvVars = false)
    {
        Process process;
        process = new Process();
        string output = string.Empty;
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        // Executing long lasting operation in batch file will hang the process, as it will wait standard output / error pipes to be processed.
        // We process these pipes here asynchronously.
        var so = new StringBuilder();
        process.OutputDataReceived += (sender, args) =>
        {
            so.AppendLine(args.Data);
        };
        var se = new StringBuilder();
        process.ErrorDataReceived += (sender, args) =>
        {
            se.AppendLine(args.Data);
        };
        process.StartInfo = processInfo;
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        output = so.ToString();
        var error = se.ToString();
        if (transferEnvVars)
        {
            var result = new Regex("--VARS--(.*)", RegexOptions.Singleline);
            var match = result.Match(output);
            if (match.Success)
            {
                output = result.Replace(output, "");
                foreach (Match m2 in new Regex("(.*?)=([^\r]*)", RegexOptions.Multiline).Matches(match.Groups[1].ToString()))
                {
                    var key = m2.Groups[1].Value;
                    var value = m2.Groups[2].Value;
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
        //return exitCode == 0;
        return output;
    }

    /// <summary>
    ///     Exe must be in path
    /// </summary>
    /// <param name = "p"></param>
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

    public static void Start(string exe, string args)
    {
        try
        {
            var arg = "/c " + exe + " " + args;
            Process.Start("cmd.exe", arg);
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }
    }

    public static void StartHidden(string parameter, string k)
    {
        try
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = k;
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

    //public static void Uri(string v)
    //{
    //    v = NormalizeUri(v);
    //    v = v.Trim();
    //    //Must UrlDecode for https://mapy.cz/?q=Antala+Sta%c5%a1ka+1087%2f3%2c+Hav%c3%ad%c5%99ov&sourceid=Searchmodule_1
    //    // to fulfillment RFC 3986 and RFC 3987 https://docs.microsoft.com/en-us/dotnet/api/system.uri.iswellformeduristring?view=netframework-4.8
    //    v = WebUtility.UrlDecode(v);
    //    if (System.Uri.IsWellFormedUriString(v, UriKind.RelativeOrAbsolute))
    //        PHWin.OpenInBrowser(v);
    //}
    public static string NormalizeUri(string v)
    {
        // Without this cant search for google apps
        v = SHReplace.ReplaceAll(v, "%22", "\"");
        return v;
    }

    public static void KillProcess(Process pr)
    {
        try
        {
            pr.Kill();
        }
        catch (Exception ex)
        {
            if (!ex.Message.Contains("Access is denied"))
                ThrowEx.CustomWithStackTrace(ex);
        }
    }

    public static int Terminate(string name)
    {
        var deleted = 0;
        foreach (var process in Process.GetProcessesByName(name))
        {
            KillProcess(process);
            deleted++;
        }

        return deleted;
    }

    //public static string RunFromPath2(string exe, string arguments)
    //{
    //    // Commented due to 'StringDictionary' does not contain a definition for 'Replace'
    //    ProcessStartInfo psi = new ProcessStartInfo(exe);
    //    psi.Arguments = arguments;
    //    var dictionary = psi.EnvironmentVariables;
    //    // Manipulate dictionary...\
    //    psi.EnvironmentVariables["PATH"] = dictionary.Replace(@"\\", @"\");
    //    return RunWithOutput(exe, arguments);
    //}
    /// <summary>
    ///     Exe must be in path
    /// </summary>
    /// <param name = "exe"></param>
    /// <param name = "fileWithoutQm"></param>
    public static void RunFromPath3(string exe, string fileWithoutQm)
    {
        var pi = new ProcessStartInfo();
        pi.FileName = exe;
        pi.Arguments = SH.WrapWithQm(fileWithoutQm);
        // To use env variables
        pi.UseShellExecute = true;
        Process.Start(pi);
    //var cmd = exe + "" + SH.WrapWithQm(fileWithoutQm);
    //Process.Start(@"C:\Windows\System32\cmd.exe", "/c " + cmd);
    //PH.ExecCmd(cmd);
    }
}