public interface IClipboardHelperBase<String, ListString, Bool>
{
    String GetText();
    ListString GetLines();
    Bool ContainsText();

    void SetText(string s);
    void SetText2(string s);

    /// <summary>
    ///     For working with thread
    /// </summary>
    /// <param name="s"></param>
    void SetText3(string s);

    void SetList(List<string> d);
    void SetLines(List<string> lines);
    void CutFiles(params string[] selected);

    // Can't be because TextBuilder is in sunamo and have many references
    //void SetText(TextBuilder stringBuilder);
    void SetText(StringBuilder stringBuilder);
}