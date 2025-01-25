namespace SunamoWinStd;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class PHWin
{
    public static void CodeInsider(ILogger logger, string defFile, bool throwExWhenError = false, int? openOnLine = null)
    {
        PH.RunVsCode(logger, CodeInsiderExe, defFile, throwExWhenError, openOnLine);
    }

    public static void Codium(ILogger logger, string defFile, bool throwExWhenError = false, int? openOnLine = null)
    {
        if (string.IsNullOrWhiteSpace(defFile)) ThrowEx.InvalidParameter(defFile, "defFile");

        //PH.RunFromPath(logger, CodiumExe, defFile, false, throwExWhenError);
        PH.RunVsCode(logger, CodiumExe, defFile, throwExWhenError, openOnLine);
    }


    public static void Code(ILogger logger, string defFile, bool throwExWhenError = false, int? openOnLine = null)
    {
        if (string.IsNullOrWhiteSpace(defFile)) ThrowEx.InvalidParameter(defFile, "defFile");

        PH.RunVsCode(logger, CodeExe, defFile, throwExWhenError, openOnLine);
    }
}