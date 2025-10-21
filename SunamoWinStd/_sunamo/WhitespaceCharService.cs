// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoWinStd._sunamo;

internal class WhitespaceCharService
{
    internal List<char>? whiteSpaceChars;
    internal void ConvertWhiteSpaceCodesToChars()
    {
        if (whiteSpaceChars != null)
        {
            return;
        }
        whiteSpaceChars = new List<char>(whiteSpacesCodes.Count);
        foreach (var item in whiteSpacesCodes)
        {
            var text = char.ConvertFromUtf32(item);
            var ch = Convert.ToChar(text);
            whiteSpaceChars.Add(ch);
        }
    }
    internal readonly List<int> whiteSpacesCodes = new(new[]
{
9, 10, 11, 12, 13, 32, 133, 160, 5760, 6158, 8192, 8193, 8194, 8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202,
8232, 8233, 8239, 8287, 12288
});
}