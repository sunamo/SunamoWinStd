namespace SunamoWinStd;


public class ProgressState
{
    public bool isRegistered { get; set; } = false;
    public void Init(Action<int> OverallSongs, Action<int> AnotherSong, Action WriteProgressBarEnd)
    {
        isRegistered = true;
        this.AnotherSong += AnotherSong;
        this.OverallSongs += OverallSongs;
        this.WriteProgressBarEnd += WriteProgressBarEnd;
    }
    public event Action<int> AnotherSong;
    public event Action<int> OverallSongs;
    public event Action WriteProgressBarEnd;
    public int n = 0;
    public void OnAnotherSong()
    {
        n++;
        OnAnotherSong(n);
    }
    public void OnAnotherSong(int n)
    {
        AnotherSong(n);
    }
    public void OnOverallSongs(int n2)
    {
        n = 0;
        OverallSongs(n2);
    }
    public void OnWriteProgressBarEnd()
    {
        WriteProgressBarEnd();
    }
}