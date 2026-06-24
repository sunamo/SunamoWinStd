namespace RunnerWinStd;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SunamoWinStd;

internal partial class Program
{
    static ILogger logger = NullLogger.Instance;

    static void Main()
    {
        PHWin.Codium(logger, @"D:\Downloads\At ziji duchove.srt");

        Console.ReadLine();
    }
}
