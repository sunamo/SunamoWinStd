namespace SunamoWinStd;


public class PsInvokeArgs
{
    public static readonly PsInvokeArgs Def = new PsInvokeArgs();
    public bool writePb = false;
    /// <summary>
    /// earlier false
    /// </summary>
    public bool immediatelyToStatus = false;
    public List<string> addBeforeEveryCommand = null;
    // nemůžu to dát do #if DEBUG protože se mi to nepromítne do nuget package
    // nevím proč furt dělám takové hloupé chyby které mě stojí čas
    //#if DEBUG
    /// <summary>
    /// pokud soubor existuje, provede load a tím urychlí vykonávání
    /// pokud neexistuje tak vykoná příkazy a save
    ///
    /// nepracuje nijak s datem poslední změny
    /// </summary>
    //
    public string pathToSaveLoadPsOutput = null;
    //[Conditional("DEBUG")]
    //public string GetPathToSaveLoadPsOutput()
    //{
    //    return pathToSaveLoadPsOutput;
    //}
    //[Conditional("DEBUG")]
    //public void SetPathToSaveLoadPsOutput(string value)
    //{
    //    pathToSaveLoadPsOutput = value;
    //}
    //#endif
}