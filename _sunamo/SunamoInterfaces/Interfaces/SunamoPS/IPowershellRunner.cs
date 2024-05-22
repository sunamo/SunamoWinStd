namespace SunamoWinStd;


//internal interface IPowershellRunner
//{
//    ProgressState clpb { get; set; }
//#if ASYNC
//    Task<List<string>> InvokeProcess(string exeFileNameWithoutPath, string arguments);
//    Task<List<List<string>>> Invoke(IList<string> commands);
//    Task<List<List<string>>> Invoke(IList<string> commands, PsInvokeArgs e);
//    Task<List<string>> Invoke(string commands);
//    Task<List<List<string>>> InvokeAsync(IList<string> commands, PsInvokeArgs e = null);
//    Task<string> InvokeLinesFromString(string v, bool writePb);
//    Task<List<string>> InvokeSingle(string command);
//#else
//List<string> InvokeProcess(string exeFileNameWithoutPath, string arguments);
//List<List<string>> Invoke(IList<string> commands);
//    List<List<string>> Invoke(IList<string> commands, PsInvokeArgs e);
//    List<string> Invoke(string commands);
//    Task<List<List<string>>> InvokeAsync(IList<string> commands, PsInvokeArgs e = null);
//    string InvokeLinesFromString(string v, bool writePb);
//     List<string> InvokeSingle(string command);
//#endif
//    Dictionary<string, List<string>> UsedCommandsInFolders { get; set; }
//    //List<string> ProcessPSObjects(ICollection<PSObject> pso);
//}
/// <summary>
/// Invoke - more commands, just run InvokeWorker
/// InvokeLinesFromString - more commands, with progress bar. Simply call InvokeWorker
/// InvokeProcess - spustí proces ze kterého vrátí output
/// InvokeSingle - just run InvokeWorker
/// </summary>
internal interface IPowershellRunner
{
    ProgressState clpb { get; set; }
    Task<List<string>> InvokeInFolder(string folder, string command);
#if ASYNC
    Task<List<List<string>>>
#else
List<List<string>>
#endif
    Invoke(List<string> commands);
#if ASYNC
    Task<List<string>>
#else
List<string>
#endif
    InvokeSingle(string command);
    //List<List<string>> Invoke(IList<string> commands, PsInvokeArgs e);
    //List<string> Invoke(string commands);
    // zakomentoval jsem protože všechny 4 invoke pouze volají InvokeWorker
#if ASYNC
    Task<List<List<string>>>
#else
List<List<string>>
#endif
    Invoke(List<string> commands, PsInvokeArgs e = null);
#if ASYNC
    Task<string>
#else
string
#endif
    InvokeLinesFromString(string v, bool writePb);
#if ASYNC
    Task<List<string>>
#else
List<string>
#endif
    InvokeProcess(string exeFileNameWithoutPath, string arguments, InvokeProcessArgs a = null);
    #region Když to bylo instanční, nechtělo mi to z nějakého důvodu fungovat. Nastavilo se true ale vracelo se furt false
    bool SaveUsedCommandToDictionary { get; set; }
    Dictionary<string, List<string>> UsedCommandsInFolders { get; set; }
    #endregion
    //List<string> ProcessPSObjects(ICollection<PSObject> pso);
}