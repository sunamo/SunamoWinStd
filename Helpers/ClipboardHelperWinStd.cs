namespace SunamoWinStd.Helpers;
using TextCopy;

public class ClipboardHelperWinStd //: IClipboardHelper
{
    //static TextCopyClipboard textCopy = new TextCopyClipboard();
    /// <summary>
    /// Schválně jsem to dal malým - místo Instance musím používat CreateInstance
    /// </summary>
    private static ClipboardHelperWinStd? instance = null;

    /// <summary>
    /// Už to nechat jako property, jako to bylo prvně, než jsem to změnil na metodu
    /// Aha musí to být metoda protože to předávám jako Func< IClipboardHelper></IClipboardHelper>
    /// </summary>
    public static ClipboardHelperWinStd Instance()
    {
        if (instance == null)
        {
            instance = new ClipboardHelperWinStd();
        }

        return instance;
    }

    public bool ContainsText()
    {
        return true;
    }

    public void CutFiles(params string[] selected)
    {
        ThrowEx.NotImplementedMethod();
    }

    public List<string> GetLines()
    {
        return SHGetLines.GetLines(GetText());
    }

    public string? GetText()
    {
        return ClipboardService.GetText();
    }

    public void SetLines(List<string> d)
    {
        SetText(string.Join(Environment.NewLine, d));
    }

    public void SetList(List<string> d)
    {
        SetText(string.Join(Environment.NewLine, d));
    }

    public void SetText(string s)
    {
        ClipboardService.SetText(s);
    }

    public void SetText(StringBuilder stringBuilder)
    {
        SetText(stringBuilder.ToString());
    }

    public void SetText2(string s)
    {
        SetText(s);
    }

    public void SetText3(string s)
    {
        SetText(s);
    }
}