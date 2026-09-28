namespace SunamoWinStd;

/// <summary>
/// Process-related Windows helpers (running a process as the interactive desktop user,
/// enumerating running processes, checking whether a file is locked).
/// </summary>
public class PHWinPi
{
    /// <summary>
    /// Starts <paramref name="fileName"/> as the interactive desktop user without elevation,
    /// even when the calling process itself is elevated. Uses the desktop shell's process token
    /// (SeIncreaseQuotaPrivilege + CreateProcessWithTokenW), so it requires the current process
    /// to already be running elevated.
    /// </summary>
    /// <param name="fileName">Full path of the executable to start.</param>
    public static void RunAsDesktopUserNoAdmin(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fileName));

        // To start a process as the shell user:
        // 1. Enable SeIncreaseQuotaPrivilege in the current token.
        // 2. Get an HWND representing the desktop shell (GetShellWindow).
        // 3. Get the process id of the process owning that window (GetWindowThreadProcessId).
        // 4. Open that process (OpenProcess).
        // 5. Get its access token (OpenProcessToken).
        // 6. Duplicate it into a primary token (DuplicateTokenEx).
        // 7. Start the new process with that primary token (CreateProcessWithTokenW).

        var processTokenHandle = IntPtr.Zero;
        try
        {
            var currentProcess = W32.GetCurrentProcess();
            if (!W32.OpenProcessToken(currentProcess, 0x0020, ref processTokenHandle)) return;

            var tokenPrivileges = new TOKEN_PRIVILEGES
            {
                PrivilegeCount = 1,
                Privileges = new LUID_AND_ATTRIBUTES[1]
            };

            if (!W32.LookupPrivilegeValue(null!, "SeIncreaseQuotaPrivilege", ref tokenPrivileges.Privileges[0].Luid)) return;

            tokenPrivileges.Privileges[0].Attributes = 0x00000002;

            if (!W32.AdjustTokenPrivileges(processTokenHandle, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero)) return;
        }
        finally
        {
            W32.CloseHandle(processTokenHandle);
        }

        // Desktop shell window. Fails if the shell is not running or has been replaced.
        var shellWindowHandle = W32.GetShellWindow();
        if (shellWindowHandle == IntPtr.Zero) return;

        var shellProcessHandle = IntPtr.Zero;
        var shellProcessTokenHandle = IntPtr.Zero;
        var primaryTokenHandle = IntPtr.Zero;
        try
        {
            if (W32.GetWindowThreadProcessId(shellWindowHandle, out var shellProcessId) == 0) return;

            shellProcessHandle = W32.OpenProcess(ProcessAccessFlags.QueryInformation, false, shellProcessId);
            if (shellProcessHandle == IntPtr.Zero) return;

            if (!W32.OpenProcessToken(shellProcessHandle, 0x0002, ref shellProcessTokenHandle)) return;

            // Minimal set of rights required by CreateProcessWithTokenW (found experimentally;
            // contrary to the current documentation).
            const uint tokenRights = 395U;
            if (!W32.DuplicateTokenEx(shellProcessTokenHandle, tokenRights, IntPtr.Zero,
                    SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out primaryTokenHandle)) return;

            var startupInfo = new STARTUPINFO();
            W32.CreateProcessWithTokenW(primaryTokenHandle, 0, fileName, "", 0, IntPtr.Zero,
                Path.GetDirectoryName(fileName), ref startupInfo, out _);
        }
        finally
        {
            W32.CloseHandle(shellProcessTokenHandle);
            W32.CloseHandle(primaryTokenHandle);
            W32.CloseHandle(shellProcessHandle);
        }
    }

    /// <summary>
    /// Determines whether the given file is currently locked by another process.
    /// </summary>
    /// <param name="fullPath">Full path of the file to check.</param>
    /// <returns>True if at least one process is locking the file.</returns>
    public static bool IsUsed(string fullPath)
    {
        return FileUtil.WhoIsLocking(fullPath).Count > 0;
    }

    /// <summary>
    /// Gets the full path of the executable running the given process id.
    /// </summary>
    /// <param name="processId">The process id.</param>
    /// <returns>The full path of the process' main module, or null if it could not be determined.</returns>
    public static string? GetProcessName(int processId)
    {
        var processHandle = W32.OpenProcess(0x0400 | 0x0010, false, processId);
        if (processHandle == IntPtr.Zero) return null;

        const int bufferLength = 4000;
        var moduleNameBuilder = new StringBuilder(bufferLength);
        string? result = null;

        if (W32.GetModuleFileNameEx(processHandle, IntPtr.Zero, moduleNameBuilder, bufferLength) > 0)
            result = moduleNameBuilder.ToString();

        W32.CloseHandle(processHandle);
        return result;
    }

    /// <summary>
    /// Gets the full executable path of every currently running process.
    /// </summary>
    /// <param name="toLowerCase">Whether to lowercase every returned path.</param>
    /// <param name="includeAlsoNull">Whether to include an entry for processes whose path could not be determined.</param>
    /// <returns>The list of full executable paths.</returns>
    public static List<string?> GetProcessesFullPaths(bool toLowerCase, bool includeAlsoNull = false)
    {
        var processes = Process.GetProcesses();
        var result = new List<string?>(processes.Length);

        foreach (var process in processes)
        {
            var path = GetProcessName(process.Id);
            if (path is null && !includeAlsoNull) continue;
            result.Add(path);
        }

        if (toLowerCase)
        {
            for (var index = 0; index < result.Count; index++) result[index] = result[index]?.ToLowerInvariant();
        }

        return result;
    }
}
