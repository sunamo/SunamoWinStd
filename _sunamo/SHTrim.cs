

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SunamoWinStd._sunamo
{
    internal class SHTrim
    {
        internal static string TrimEnd(string name, string ext)
        {
            while (name.EndsWith(ext)) return name.Substring(0, name.Length - ext.Length);

            return name;
        }
        internal static string TrimStart(string v, string s)
        {
            while (v.StartsWith(s))
            {
                v = v.Substring(s.Length);
            }

            return v;
        }

        internal static bool TrimIfStartsWith(ref string s, string p)
        {
            if (s.StartsWith(p))
            {
                s = s.Substring(p.Length);
                return true;
            }
            return false;
        }
    }
}
