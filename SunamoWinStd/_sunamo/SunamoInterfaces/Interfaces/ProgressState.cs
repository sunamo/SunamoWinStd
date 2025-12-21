namespace SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;

internal class ProgressState
{
    internal bool isRegistered { get; set; } = false;
    internal event Action<int> AnotherSong;
    internal event Action<int> OverallSongs;
    internal event Action WriteProgressBarEnd;
    internal int n = 0;
}