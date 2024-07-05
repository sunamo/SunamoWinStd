namespace SunamoWinStd._sunamo.SunamoInterfaces.Interfaces;


internal class ProgressState
{
    internal bool isRegistered { get; set; } = false;
    internal void Init(Action<int> OverallSongs, Action<int> AnotherSong, Action WriteProgressBarEnd)
    {
        isRegistered = true;
        this.AnotherSong += AnotherSong;
        this.OverallSongs += OverallSongs;
        this.WriteProgressBarEnd += WriteProgressBarEnd;
    }
    internal event Action<int> AnotherSong;
    internal event Action<int> OverallSongs;
    internal event Action WriteProgressBarEnd;
    internal int n = 0;
    internal void OnAnotherSong()
    {
        n++;
        OnAnotherSong(n);
    }
    internal void OnAnotherSong(int n)
    {
        AnotherSong(n);
    }
    internal void OnOverallSongs(int n2)
    {
        n = 0;
        OverallSongs(n2);
    }
    internal void OnWriteProgressBarEnd()
    {
        WriteProgressBarEnd();
    }
}