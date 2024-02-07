
namespace SunamoWinStd._sunamo;

using SunamoArgs;
using System;
using System.Collections.Generic;


internal class CAChangeContent
{
    internal static List<string> ChangeContent0(ChangeContentArgs a, List<string> files_in, Func<string, string> func)
    {
        for (int i = 0; i < files_in.Count; i++)
        {
            files_in[i] = func.Invoke(files_in[i]);
        }

        RemoveNullOrEmpty(a, files_in);

        return files_in;
    }

    internal static List<string> ChangeContent1(ChangeContentArgs a, List<string> files_in, Func<string, string, string> func, string a1)
    {
        var result = ChangeContent<string>(a, files_in, func, a1);
        return result;
    }

    /// <summary>
    /// Direct edit input collection
    ///
    /// Dříve to bylo List<string> files_in, Func<string,
    /// </summary>
    /// <typeparam name="Arg1"></typeparam>
    /// <param name="files_in"></param>
    /// <param name="func"></param>
    /// <param name="arg"></param>
    internal static List<string> ChangeContent<Arg1>(ChangeContentArgs a, List<string> files_in, Func<string, Arg1, string> func, Arg1 arg, Func<Arg1, string, string> funcSwitch12 = null)
    {
        if (a == null)
        {
            a = new ChangeContentArgs();
        }

        if (a.switchFirstAndSecondArg)
        {
            files_in = ChangeContentSwitch12<Arg1>(files_in, funcSwitch12, arg);
        }
        else
        {
            for (int i = 0; i < files_in.Count; i++)
            {
                files_in[i] = func.Invoke(files_in[i], arg);
            }
        }


        RemoveNullOrEmpty(a, files_in);

        return files_in;
    }

    private static void RemoveNullOrEmpty(ChangeContentArgs a, List<string> files_in)
    {
        if (a != null)
        {
            if (a.removeNull)
            {
                files_in.Remove(null);
            }

            if (a.removeEmpty)
            {
                for (int i = files_in.Count - 1; i >= 0; i--)
                {
                    if (files_in[i].Trim() == string.Empty)
                    {
                        files_in.RemoveAt(i);
                    }
                }
            }
        }
    }

    internal static List<string> ChangeContentSwitch12<Arg1>(List<string> files_in, Func<Arg1, string, string> func, Arg1 arg)
    {
        for (int i = 0; i < files_in.Count; i++)
        {
            files_in[i] = func.Invoke(arg, files_in[i]);
        }
        return files_in;
    }
}

