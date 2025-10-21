// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
﻿namespace SunamoWinStd;

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

    static bool? existsCursor = null;

    public static void Code(ILogger logger, string defFile, bool throwExWhenError = false, int? openOnLine = null)
    {
        if (string.IsNullOrWhiteSpace(defFile)) ThrowEx.InvalidParameter(defFile, "defFile");

        // Kontrola existence Cursor.exe v běžné instalační cestě
        var userName = Environment.UserName;
        var cursorPath = Path.Combine($@"C:\Users\{userName}\AppData\Local\Programs\cursor", "Cursor.exe");

        if (existsCursor == null)
        {
            existsCursor = File.Exists(cursorPath);
        }

        if (existsCursor.Value)
        {
            // Použití Cursor s parametry podobnými VS Code
            var arguments = defFile;
            if (openOnLine != null)
            {
                arguments = $"-g {defFile}:{openOnLine}";
            }

            try
            {
                Process.Start(new ProcessStartInfo(cursorPath, arguments));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to start Cursor: {ex.Message}");
                if (throwExWhenError)
                {
                    throw;
                }
                // Fall back to VS Code if Cursor fails
                PH.RunVsCode(logger, CodeExe, defFile, throwExWhenError, openOnLine);
            }
        }
        else
        {
            // Fall back to VS Code if Cursor doesn't exist
            PH.RunVsCode(logger, CodeExe, defFile, throwExWhenError, openOnLine);
        }
    }
}