// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;


internal class ProgressState
{
    internal bool isRegistered { get; set; } = false;
    internal event Action<int> AnotherSong;
    internal event Action<int> OverallSongs;
    internal event Action WriteProgressBarEnd;
    internal int n = 0;
}