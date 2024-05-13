
namespace SunamoWinStd;


using System.Net;
using System.Text.RegularExpressions;


public partial class PH
{
    static Type type = typeof(PH);



    /// <summary>
    /// https://stackoverflow.com/a/12393522
    /// Return SE or output if everything gone good
    /// </summary>
    /// <param name="exe"></param>
    /// <param name="arguments"></param>
    public static string RunFromPath(string exe, string arguments, bool withOutput)
    {
        var enviromentPath = Environment.GetEnvironmentVariable("PATH");

        var paths = enviromentPath.Split(';'); // SHSplit.SplitChar(enviromentPath, ';');

#if DEBUG
        var wc = paths.Where(d => d.Contains("Code"));
        paths.Reverse();
#endif
        var paths2 = paths.Select(x => Path.Combine(x, exe));
        var files = paths2.Where(x => File.Exists(x));
        var fi = files.FirstOrDefault();

        var exePath = fi;

        if (!string.IsNullOrWhiteSpace(exePath))
        {
            if (withOutput)
            {
                return RunWithOutput(exe, arguments);
            }
            Process.Start(exe, arguments);
            return string.Empty;
        }
        throw new Exception(exe + "is not in the path!");
        return null;
    }

    public static bool ExecCmd(string cmd)
    {
        string output;
        var b = ExecCmd(cmd, out output);
        return b;
    }

    /// <summary>
    /// Executes command
    /// </summary>
    /// <param name="cmd">command to be executed</param>
    /// <param name="output">output which application produced</param>
    /// <param name="transferEnvVars">true - if retain PATH environment variable from executed command</param>
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
        string output = null;

        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;


        // Executing long lasting operation in batch file will hang the process, as it will wait standard output / error pipes to be processed.
        // We process these pipes here asynchronously.
        StringBuilder so = new StringBuilder();
        process.OutputDataReceived += (sender, args) => { so.AppendLine(args.Data); };
        StringBuilder se = new StringBuilder();
        process.ErrorDataReceived += (sender, args) => { se.AppendLine(args.Data); };

        process.StartInfo = processInfo;
        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();



        process.WaitForExit();

        output = so.ToString();
        string error = se.ToString();

        if (transferEnvVars)
        {
            Regex r = new Regex("--VARS--(.*)", RegexOptions.Singleline);
            var m = r.Match(output);
            if (m.Success)
            {
                output = r.Replace(output, "");

                foreach (Match m2 in new Regex("(.*?)=([^\r]*)", RegexOptions.Multiline).Matches(m.Groups[1].ToString()))
                {
                    string key = m2.Groups[1].Value;
                    string value = m2.Groups[2].Value;
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        if (error.Length != 0)
            output += error;
        int exitCode = process.ExitCode;

        if (exitCode != 0)
            Console.WriteLine("Error: " + output + Consts.rn + error);

        process.Close();
        //return exitCode == 0;

        return output;
    }

    /// <summary>
    /// Exe must be in path
    /// </summary>
    /// <param name="p"></param>
    public static void Start(string p)
    {
        try
        {
            Process.Start("cmd.exe", "/c " + p);
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
            var arg = "/c " + exe + AllStrings.space + args;
            Process.Start("cmd.exe", arg);
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }
    }




    public static void StartHidden(string p, string k)
    {
        try
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = k;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + p;
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }
    }



    public static void Uri(string v)
    {
        v = NormalizeUri(v);
        v = v.Trim();
        //Must UrlDecode for https://mapy.cz/?q=Antala+Sta%c5%a1ka+1087%2f3%2c+Hav%c3%ad%c5%99ov&sourceid=Searchmodule_1
        // to fulfillment RFC 3986 and RFC 3987 https://docs.microsoft.com/en-us/dotnet/api/system.uri.iswellformeduristring?view=netframework-4.8
        v = WebUtility.UrlDecode(v);
        if (System.Uri.IsWellFormedUriString(v, UriKind.RelativeOrAbsolute))
        {
            Process.Start(v);
        }
        else
        {
            //////////DebugLogger.Instance.WriteLine("Wasnt in right format: " + v);
        }
    }

    public static string NormalizeUri(string v)
    {
        // Without this cant search for google apps
        v = SHReplace.ReplaceAll(v, "%22", AllStrings.qm);
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
            {
                ThrowEx.CustomWithStackTrace(ex);
            }
        }
    }

    public static int Terminate(string name)
    {
        int deleted = 0;

        foreach (var process in Process.GetProcessesByName(name))
        {
            KillProcess(process);
            deleted++;
        }

        return deleted;
    }
}
