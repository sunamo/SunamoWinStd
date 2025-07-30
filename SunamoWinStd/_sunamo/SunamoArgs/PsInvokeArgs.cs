namespace SunamoWinStd._sunamo.SunamoArgs;


internal class PsInvokeArgs
{
    internal static readonly PsInvokeArgs Def = new PsInvokeArgs();
    internal bool writePb = false;
    /// <summary>
    /// earlier false
    /// </summary>
    internal bool immediatelyToStatus = false;
    internal List<string> addBeforeEveryCommand = null;
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
    internal string pathToSaveLoadPsOutput = null;
    //[Conditional("DEBUG")]
    //internal string GetPathToSaveLoadPsOutput()
    //{
    //    return pathToSaveLoadPsOutput;
    //}
    //[Conditional("DEBUG")]
    //internal void SetPathToSaveLoadPsOutput(string value)
    //{
    //    pathToSaveLoadPsOutput = value;
    //}
    //#endif
}