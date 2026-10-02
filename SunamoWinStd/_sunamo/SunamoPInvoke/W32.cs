using System.Runtime.InteropServices;
using System.Text;

namespace SunamoWinStd._sunamo.SunamoPInvoke;

/// <summary>
/// Minimal set of Windows API declarations used by this package (process tokens, clipboard, MIME sniffing).
/// </summary>
internal static class W32
{
    /// <summary>Determines the MIME type from the data provided.</summary>
    [DllImport("urlmon.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    internal static extern uint FindMimeFromData(uint pBC, [MarshalAs(UnmanagedType.LPStr)] string pwzUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer, uint cbSize, [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed, uint dwMimeFlags, out uint ppwzMimeOut, uint dwReserverd);

    /// <summary>Retrieves a pseudo handle for the current process.</summary>
    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern IntPtr GetCurrentProcess();

    /// <summary>Opens the access token associated with a process.</summary>
    [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

    /// <summary>Retrieves the LUID used to represent the specified privilege name.</summary>
    [DllImport("advapi32.dll", SetLastError = true)]
    internal static extern bool LookupPrivilegeValue(string host, string name, ref LUID pluid);

    /// <summary>Enables or disables privileges in the specified access token.</summary>
    [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
    internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TOKEN_PRIVILEGES newst, int len, IntPtr prev, IntPtr relen);

    /// <summary>Retrieves a handle to the Shell's desktop window.</summary>
    [DllImport("user32.dll")]
    internal static extern IntPtr GetShellWindow();

    /// <summary>Retrieves the identifier of the thread (and process) that created the specified window.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    /// <summary>Opens an existing local process object using typed access flags.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

    /// <summary>Opens an existing local process object using a raw access mask.</summary>
    [DllImport("kernel32.dll")]
    internal static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

    /// <summary>Creates a new access token that duplicates an existing token.</summary>
    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, SECURITY_IMPERSONATION_LEVEL impersonationLevel, TOKEN_TYPE tokenType, out IntPtr phNewToken);

    /// <summary>Creates a new process running in the security context of the specified token.</summary>
    [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool CreateProcessWithTokenW(IntPtr hToken, int dwLogonFlags, string lpApplicationName, string lpCommandLine, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    /// <summary>Retrieves the full path of the module loaded in the given process.</summary>
    [DllImport("psapi.dll")]
    internal static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

    /// <summary>Closes an open object handle.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CloseHandle(IntPtr hObject);

    /// <summary>Locks a global memory object and returns a pointer to its first byte.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr GlobalLock(IntPtr hMem);

    /// <summary>Decrements the lock count of a global memory object.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool GlobalUnlock(IntPtr hMem);

    /// <summary>Retrieves the size of a global memory object in bytes.</summary>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern UIntPtr GlobalSize(IntPtr hMem);

    /// <summary>Closes the clipboard.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool CloseClipboard();

    /// <summary>Places data on the clipboard in the specified format.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

    /// <summary>Retrieves data from the clipboard in the specified format.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr GetClipboardData(uint uFormat);

    /// <summary>Determines whether the clipboard contains data in the specified format.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool IsClipboardFormatAvailable(uint format);

    /// <summary>Opens the clipboard for examination.</summary>
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool OpenClipboard(IntPtr hWndNewOwner);
}
