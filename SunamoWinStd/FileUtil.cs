namespace SunamoWinStd;

/// <summary>
/// Provides utilities for detecting which processes have locks on files.
/// </summary>
public static class FileUtil
{
    private const int RmRebootReasonNone = 0;
    private const int CCH_RM_MAX_APP_NAME = 255;
    private const int CCH_RM_MAX_SVC_NAME = 63;
    [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
    private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);
    [DllImport("rstrtmgr.dll")]
    private static extern int RmEndSession(uint pSessionHandle);
    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    private static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames,
        uint nApplications, [In] RM_UNIQUE_PROCESS[]? rgApplications, uint nServices, string[]? rgsServiceNames);
    [DllImport("rstrtmgr.dll")]
    private static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo,
        [In][Out] RM_PROCESS_INFO[]? rgAffectedApps, ref uint lpdwRebootReasons);
    /// <summary>
    ///     Find out what process(es) have a lock on the specified file.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <param name="isThrowingOnError">Whether to throw exceptions on errors.</param>
    /// <returns>Processes locking the file</returns>
    /// <remarks>
    ///     See also:
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    ///     http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
    /// </remarks>
    public static List<Process> WhoIsLocking(string path, bool isThrowingOnError = true)
    {
        uint handle;
        var key = Guid.NewGuid().ToString();
        var processes = new List<Process>();
        var result = RmStartSession(out handle, 0, key);
        if (result != 0)
            throw new Exception("CouldNotBeginRestartSessionUnableToDetermineFileLocker");
        try
        {
            const int ERROR_MORE_DATA = 234;
            uint pnProcInfoNeeded = 0, pnProcInfo = 0, lpdwRebootReasons = RmRebootReasonNone;
            string[] resources = { path };
            result = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);
            if (result != 0)
                if (isThrowingOnError)
                    throw new Exception("CouldNotRegisterResource.");
            result = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);
            if (result == ERROR_MORE_DATA)
            {
                var processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                pnProcInfo = pnProcInfoNeeded;
                result = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                if (result == 0)
                {
                    processes = new List<Process>((int)pnProcInfo);
                    for (var i = 0; i < pnProcInfo; i++)
                        try
                        {
                            processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("Process no longer running: " + ex.Message);
                        }
                }
                else
                {
                    if (isThrowingOnError) throw new Exception("CouldNotListProcessesLockingResource");
                }
            }
            else if (result != 0)
            {
                if (isThrowingOnError) throw new Exception("CouldNotListProcessesLockingResourceFailedToGetSizeOfResult");
            }
        }
        finally
        {
            RmEndSession(handle);
        }
        return processes;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public FILETIME ProcessStartTime;
    }
    private enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
        public string strAppName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
        public string strServiceShortName;
        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;
        [MarshalAs(UnmanagedType.Bool)] public bool bRestartable;
    }
}
