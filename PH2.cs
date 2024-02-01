
namespace SunamoWinStd;
using SunamoWinStd._sunamo;


public partial class PH
{


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


}
