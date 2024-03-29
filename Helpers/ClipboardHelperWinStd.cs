
namespace SunamoWinStd.Helpers;
using SunamoInterfaces.Interfaces;
using SunamoWinStd._sunamo;

public class ClipboardHelperWinStd : IClipboardHelper
{
    static TextCopy.Clipboard textCopy = new TextCopy.Clipboard();
    /// <summary>
    /// Schválně jsem to dal malým - místo Instance musím používat CreateInstance
    /// </summary>
    public static ClipboardHelperWinStd instance = null;

    /// <summary>
    /// Už to nechat jako property, jako to bylo prvně, než jsem to změnil na metodu
    /// Aha musí to být metoda protože to předávám jako Func< IClipboardHelper></IClipboardHelper>
    /// </summary>
    public static IClipboardHelper Instance()
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

    public string GetText()
    {
        return textCopy.GetText();
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
        textCopy.SetText(s);
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
