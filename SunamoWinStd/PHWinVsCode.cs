namespace SunamoWinStd;

partial class PHWin
{
    /// <summary>
    /// Code insider.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    /// <param name="openOnLine">The open on line.</param>
    public static void CodeInsider(ILogger logger, string filePath, bool isThrowingOnError = false, int? openOnLine = null)
    {
        PH.RunVsCode(logger, CodeInsiderExe, filePath, isThrowingOnError, openOnLine);
    }

    /// <summary>
    /// Codium.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    /// <param name="openOnLine">The open on line.</param>
    public static void Codium(ILogger logger, string filePath, bool isThrowingOnError = false, int? openOnLine = null)
    {
        if (string.IsNullOrWhiteSpace(filePath)) ThrowEx.InvalidParameter(filePath, "filePath");
        PH.RunVsCode(logger, CodiumExe, filePath, isThrowingOnError, openOnLine);
    }

    private static bool? isCursorAvailable = null;

    /// <summary>
    /// Code.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="isThrowingOnError">The is throwing on error.</param>
    /// <param name="openOnLine">The open on line.</param>
    public static void Code(ILogger logger, string filePath, bool isThrowingOnError = false, int? openOnLine = null)
    {
        if (string.IsNullOrWhiteSpace(filePath)) ThrowEx.InvalidParameter(filePath, "filePath");

        var userName = Environment.UserName;
        var cursorPath = Path.Combine($@"C:\Users\{userName}\AppData\Local\Programs\cursor", "Cursor.exe");

        isCursorAvailable ??= File.Exists(cursorPath);

        if (isCursorAvailable.Value)
        {
            var arguments = filePath;
            if (openOnLine != null)
            {
                arguments = $"-g {filePath}:{openOnLine}";
            }

            try
            {
                Process.Start(new ProcessStartInfo(cursorPath, arguments));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to start Cursor: {ex.Message}");
                if (isThrowingOnError)
                {
                    throw;
                }
                PH.RunVsCode(logger, CodeExe, filePath, isThrowingOnError, openOnLine);
            }
        }
        else
        {
            PH.RunVsCode(logger, CodeExe, filePath, isThrowingOnError, openOnLine);
        }
    }
}
