
using System.ComponentModel;

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

    public static string RunFromPath2(string exe, string arguments)
    {
        // Commented due to 'StringDictionary' does not contain a definition for 'Replace'

        //ProcessStartInfo psi = new ProcessStartInfo(exe);
        //psi.Arguments = arguments;
        //StringDictionary dictionary = psi.EnvironmentVariables;

        //// Manipulate dictionary...\

        //psi.EnvironmentVariables["PATH"] = dictionary.Replace(@"\\", @"\");
        //return RunWithOutput(exe, arguments);

        return null;
    }

    /// <summary>
    /// Exe must be in path
    /// 
    /// </summary>
    /// <param name="exe"></param>
    /// <param name="fileWithoutQm"></param>
    public static void RunFromPath3(string exe, string fileWithoutQm)
    {
        ProcessStartInfo pi = new ProcessStartInfo();
        pi.FileName = exe;
        pi.Arguments = SH.WrapWithQm(fileWithoutQm);
        // To use env variables
        pi.UseShellExecute = true;
        Process.Start(pi);

        //var cmd = exe + AllStrings.space + SH.WrapWithQm(fileWithoutQm);
        //Process.Start(@"C:\Windows\System32\cmd.exe", "/c " + cmd);
        //PH.ExecCmd(cmd);
    }

    /// <param name="exe"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static string RunFromPathBetter(string exe, string arguments)
    {
        string enviromentPath = Environment.GetEnvironmentVariable("PATH");
        string[] paths = enviromentPath.Split(';');

        foreach (string thisPath in paths)
        {
            string thisFile = Path.Combine(thisPath, exe);
            string[] executableExtensions = new string[] { ".exe" }; // , ".com", ".bat", ".sh", ".vbs", ".vbscript", ".vbe", ".js", ".rb", ".cmd", ".cpl", ".ws", ".wsf", ".msc", ".gadget"

            foreach (string extension in executableExtensions)
            {
                string fullFile = thisFile + extension;

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

        foreach (string thisPath in paths)
        {
            string thisFile = Path.Combine(thisPath, exe);

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
        //var processes = Process.GetProcesses(name).Where(s => s.ProcessName.Contains(name)).Select(d => d.ProcessName);
        return pr.Count() > 1;
    }

    public static bool ExistsOnPath(string fileName)
    {
        return GetFullPath(fileName) != null;
    }

    public static string GetFullPath(string fileName)
    {
        if (File.Exists(fileName))
            return Path.GetFullPath(fileName);

        var values = Environment.GetEnvironmentVariable("PATH");
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
        List<string> processes = PH.GetProcessesNames(true);
        var s = processes.Where(d => d.Contains(name.ToLower())).ToList(); //CA.ReturnWhichContains(processes, name.ToLower());
        return s;
    }

    public static int TerminateProcessesWithNameContains(string name)
    {
        var s = ProcessesWithNameContains(name);

        int ended = 0;
        foreach (var item in s)
        {
            ended += PH.Terminate(item);
        }
        return ended;
    }

    /// <summary>
    /// without extensions and all lower
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static int TerminateProcessesWithName(string name)
    {
        name = name.ToLower();
        name = Path.GetFileNameWithoutExtension(name);
        List<string> processes = PH.GetProcessesNames(true);

        int ended = 0;

        if (processes.Contains(name))
        {
            ended += PH.Terminate(name);
        }


        return ended;
    }

    #region
    /// <summary>
    /// Alternative is FileUtil.WhoIsLocking
    /// </summary>
    /// <param name="fileName"></param>
    public static void ShutdownProcessWhichOccupyFileHandleExe(string fileName)
    {
        var pr2 = FindProcessesWhichOccupyFileHandleExe(fileName);
        foreach (var pr in pr2)
        {
            KillProcess(pr);
        }
    }
    #endregion

    /// <summary>
    /// Alternative is FileUtil.WhoIsLocking
    /// A2 must be even is not used
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static List<Process> FindProcessesWhichOccupyFileHandleExe(string fileName, bool throwEx = true)
    {
        List<Process> pr2 = new List<Process>();

        Process tool = new Process();
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

        }

        tool.WaitForExit();
        string outputTool = null;

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

        string matchPattern = @"(?<=\s+piD:\s+)\b(\d+)\b(?=\s+)";
        var matches = Regex.Matches(outputTool, matchPattern);
        foreach (Match match in matches)
        {
            var pr = Process.GetProcessById(int.Parse(match.Value));
            pr2.Add(pr);
        }

        return pr2;
    }




    public static void StartAllUri(List<string> all)
    {
        foreach (var item in all)
        {
            Uri(UH.AppendHttpIfNotExists(item));
        }
    }

    public static List<string> GetProcessesNames(bool lower)
    {
        var p = Process.GetProcesses().Select(d => d.ProcessName).ToList();
        if (lower)
        {
            CA.ToLower(p);
        }

        return p;
    }

    /// <summary>
    /// For search one term in all uris use UriWebServices.SearchInAll
    /// </summary>
    /// <param name = "carModels"></param>
    /// <param name = "v"></param>
    public static void StartAllUri(List<string> carModels, string v)
    {

        for (int i = 0; i < carModels.Count; i++)
        {
            if (i % 10 == 0 && i != 0)
            {
                //System.Diagnostics.Debugger.Break();
            }
            //PHWin.OpenInBrowser(UH.AppendHttpIfNotExists(UriWebServices.FromChromeReplacement(v, carModels[i])));
        }
    }

    public static void StartAllUri(List<string> carModels, Func<string, string> spritMonitor)
    {
        carModels = CAChangeContent.ChangeContent0(null, carModels, spritMonitor);
        carModels = CAChangeContent.ChangeContent0(null, carModels, NormalizeUri);
        StartAllUri(carModels);
    }





    /// <summary>
    /// Start all uri in clipboard, splitted by whitespace
    /// </summary>
    public static void StartAllUri(string text)
    {
        //var text = ClipboardHelper.GetText();
        var uris = SHSplit.SplitByWhiteSpaces(text);
        StartAllUri(uris);
    }
}
