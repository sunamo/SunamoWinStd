using System.Runtime.InteropServices;

namespace SunamoWinStd._sunamo.SunamoPInvoke;

/// <summary>Locally unique identifier used in Windows security operations.</summary>
internal struct LUID
{
    /// <summary>Low-order part of the LUID.</summary>
    public uint LowPart;
    /// <summary>High-order part of the LUID.</summary>
    public int HighPart;
}

/// <summary>LUID together with its privilege attributes.</summary>
internal struct LUID_AND_ATTRIBUTES
{
    /// <summary>The locally unique identifier.</summary>
    public LUID Luid;
    /// <summary>Privilege state flags.</summary>
    public uint Attributes;
}

/// <summary>Set of privileges for an access token.</summary>
internal struct TOKEN_PRIVILEGES
{
    /// <summary>Number of entries in <see cref="Privileges"/>.</summary>
    public uint PrivilegeCount;
    /// <summary>Privileges and their attributes.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public LUID_AND_ATTRIBUTES[] Privileges;
}

/// <summary>Access rights for opening a process (only the value used here).</summary>
internal enum ProcessAccessFlags : uint
{
    /// <summary>Required to query certain information about a process.</summary>
    QueryInformation = 0x00000400
}

/// <summary>Security impersonation levels for token operations.</summary>
internal enum SECURITY_IMPERSONATION_LEVEL
{
    /// <summary>Server cannot obtain identification information about the client.</summary>
    SecurityAnonymous,
    /// <summary>Server can obtain information about the client but cannot impersonate.</summary>
    SecurityIdentification,
    /// <summary>Server can impersonate the client on its local system.</summary>
    SecurityImpersonation,
    /// <summary>Server can impersonate the client on remote systems.</summary>
    SecurityDelegation
}

/// <summary>Type of a security token.</summary>
internal enum TOKEN_TYPE
{
    /// <summary>Primary token.</summary>
    TokenPrimary = 1,
    /// <summary>Impersonation token.</summary>
    TokenImpersonation
}

/// <summary>Information about a newly created process and its primary thread.</summary>
internal struct PROCESS_INFORMATION
{
    /// <summary>Handle to the new process.</summary>
    public nint hProcess;
    /// <summary>Handle to the primary thread.</summary>
    public nint hThread;
    /// <summary>Process identifier.</summary>
    public int dwProcessId;
    /// <summary>Thread identifier.</summary>
    public int dwThreadId;
}

/// <summary>Window station, desktop, handles and appearance of the main window for a new process.</summary>
internal struct STARTUPINFO
{
    /// <summary>Size of the structure in bytes.</summary>
    public int cb;
    /// <summary>Reserved.</summary>
    public string lpReserved;
    /// <summary>Desktop (and window station) name.</summary>
    public string lpDesktop;
    /// <summary>Console window title.</summary>
    public string lpTitle;
    /// <summary>X offset of the window.</summary>
    public int dwX;
    /// <summary>Y offset of the window.</summary>
    public int dwY;
    /// <summary>Window width.</summary>
    public int dwXSize;
    /// <summary>Window height.</summary>
    public int dwYSize;
    /// <summary>Console buffer width.</summary>
    public int dwXCountChars;
    /// <summary>Console buffer height.</summary>
    public int dwYCountChars;
    /// <summary>Console text and background colors.</summary>
    public int dwFillAttribute;
    /// <summary>Flags determining which members are used.</summary>
    public int dwFlags;
    /// <summary>Window show state.</summary>
    public short wShowWindow;
    /// <summary>Reserved for the C runtime.</summary>
    public short cbReserved2;
    /// <summary>Reserved for the C runtime.</summary>
    public nint lpReserved2;
    /// <summary>Standard input handle.</summary>
    public nint hStdInput;
    /// <summary>Standard output handle.</summary>
    public nint hStdOutput;
    /// <summary>Standard error handle.</summary>
    public nint hStdError;
}
