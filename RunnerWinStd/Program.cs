namespace RunnerWinStd;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SunamoWinStd;

/// <summary>
/// Main entry point for the SunamoWinStd runner application.
/// </summary>
internal partial class Program
{
    static ILogger logger = NullLogger.Instance;

    /// <summary>
    /// Application entry point.
    /// </summary>
    static void Main()
    {
        PHWin.Codium(logger, @"D:\Downloads\At ziji duchove.srt");

        Console.ReadLine();
    }
}
