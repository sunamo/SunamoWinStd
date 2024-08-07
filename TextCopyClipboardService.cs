namespace SunamoWinStd;

/// <summary>
///     Bude to nejlepší možnost
///     Jinak bych musel tu nugetovat TextCopy a mohli by vznikat konflikty
///     Nebo tu dát kód z TextCopy ale je to rozsáhlé https://github.com/CopyText/TextCopy/tree/main/src/TextCopy
/// </summary>
public class TextCopyClipboardService
{
    public static Func<string> GetText;

    public static Action<string> SetText;
}