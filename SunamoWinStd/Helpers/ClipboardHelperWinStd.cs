namespace SunamoWinStd.Helpers;

public class ClipboardHelperWinStd
{
    private static ClipboardHelperWinStd? instance = null;

    // Must be a method because it is passed as Func.
    public static ClipboardHelperWinStd Instance()
    {
        instance ??= new ClipboardHelperWinStd();
        return instance;
    }

    public bool ContainsText()
    {
        return true;
    }

    public List<string> GetLines()
    {
        return SHGetLines.GetLines(GetText());
    }

    public string? GetText()
    {
        return ClipboardService.GetText();
    }

    public void SetLines(List<string> list)
    {
        SetText(string.Join(Environment.NewLine, list));
    }

    public void SetList(List<string> list)
    {
        SetText(string.Join(Environment.NewLine, list));
    }

    public void SetText(string text)
    {
        ClipboardService.SetText(text);
    }

    public void SetText(StringBuilder stringBuilder)
    {
        SetText(stringBuilder.ToString());
    }

    public void SetText2(string text)
    {
        SetText(text);
    }

    public void SetText3(string text)
    {
        SetText(text);
    }
}
