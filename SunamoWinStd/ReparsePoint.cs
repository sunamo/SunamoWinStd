// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoWinStd;

public class ReparsePoint
{
    public enum TagType
    {
        None = 0,
        MountPoint = 1,
        SymbolicLink = 2,
        JunctionPoint = 3
    }
    // This is based on the code at http://www.flexhex.com/docs/articles/hard-links.phtml

    private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003; // Moiunt point or junction, see winnt.h

    private const uint
        IO_REPARSE_TAG_SYMLINK =
            0xA000000C; // SYMLINK or SYMLINKD (see http://wesnerm.blogs.com/net_undocumented/2006/10/index.html)

    private const uint SE_PRIVILEGE_ENABLED = 0x00000002;
    private const string SE_BACKUP_NAME = "SeBackupPrivilege";
    private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
    private const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
    private const uint FILE_DEVICE_FILE_SYSTEM = 9;
    private const uint FILE_ANY_ACCESS = 0;
    private const uint METHOD_BUFFERED = 0;
    private const int MAXIMUM_REPARSE_DATA_BUFFER_SIZE = 16 * 1024;
    private const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
    private const int FSCTL_GET_REPARSE_POINT = 42;

    private readonly string normalisedTarget;

    /// <summary>
    ///     Takes a full path to a reparse point and finds the target.
    /// </summary>
    /// <param name="path">Full path of the reparse point</param>
    public ReparsePoint(string path)
    {
        Debug.Assert(!string.IsNullOrEmpty(path) && path.Length > 2 && path[1] == ':' && path[2] == '\\');
        normalisedTarget = "";
        Tag = TagType.None;
        bool success;
        int lastError;
        // Apparently we need to have backup privileges
        nint token;
        var tokenPrivileges = new TOKEN_PRIVILEGES();
        tokenPrivileges.Privileges = new LUID_AND_ATTRIBUTES[1];
        success = OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, out token);
        lastError = Marshal.GetLastWin32Error();
        if (success)
        {
            success = LookupPrivilegeValue(null, SE_BACKUP_NAME,
                out tokenPrivileges.Privileges[0].Luid); // null for local system
            lastError = Marshal.GetLastWin32Error();
            if (success)
            {
                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
                success = AdjustTokenPrivileges(token, false, ref tokenPrivileges, Marshal.SizeOf(tokenPrivileges),
                    nint.Zero, nint.Zero);
                lastError = Marshal.GetLastWin32Error();
            }

            CloseHandle(token);
        }

        if (success)
        {
            // Open the file and get its handle
            var handle = CreateFile(path, FileAccess.Read, FileShare.None, 0, FileMode.Open,
                FILE_FLAG_OPEN_REPARSE_POINT | FILE_FLAG_BACKUP_SEMANTICS, nint.Zero);
            lastError = Marshal.GetLastWin32Error();
            if (handle.ToInt32() >= 0)
            {
                var buffer = new REPARSE_DATA_BUFFER();
                // Make up the control code - see CTL_CODE on ntddk.h
                var controlCode = (FILE_DEVICE_FILE_SYSTEM << 16) | (FILE_ANY_ACCESS << 14) |
                                  (FSCTL_GET_REPARSE_POINT << 2) | METHOD_BUFFERED;
                uint bytesReturned;
                success = DeviceIoControl(handle, controlCode, nint.Zero, 0, out buffer,
                    MAXIMUM_REPARSE_DATA_BUFFER_SIZE, out bytesReturned, nint.Zero);
                lastError = Marshal.GetLastWin32Error();
                if (success)
                {
                    var subsString = "";
                    var printString = "";
                    // Note that according to http://wesnerm.blogs.com/net_undocumented/2006/10/symbolic_links_.html
                    // Symbolic links store relative paths, while junctions use absolute paths
                    // however, they can in fact be either, and may or may not have a leading \.
                    Debug.Assert(
                        buffer.ReparseTag == IO_REPARSE_TAG_SYMLINK || buffer.ReparseTag == IO_REPARSE_TAG_MOUNT_POINT,
                        "Unrecognised reparse tag"); // We only recognise these two
                    if (buffer.ReparseTag == IO_REPARSE_TAG_SYMLINK)
                    {
                        // for some reason symlinks seem to have an extra two characters on the front
                        subsString = new string(buffer.ReparseTarget, buffer.SubsNameOffset / 2 + 2,
                            buffer.SubsNameLength / 2);
                        printString = new string(buffer.ReparseTarget, buffer.PrintNameOffset / 2 + 2,
                            buffer.PrintNameLength / 2);
                        Tag = TagType.SymbolicLink;
                    }
                    else if (buffer.ReparseTag == IO_REPARSE_TAG_MOUNT_POINT)
                    {
                        // This could be a junction or a mounted drive - a mounted drive starts with "\\??\\Volume"
                        subsString = new string(buffer.ReparseTarget, buffer.SubsNameOffset / 2,
                            buffer.SubsNameLength / 2);
                        printString = new string(buffer.ReparseTarget, buffer.PrintNameOffset / 2,
                            buffer.PrintNameLength / 2);
                        Tag = subsString.StartsWith(@"\??\Volume") ? TagType.MountPoint : TagType.JunctionPoint;
                    }

                    //Debug.Assert(!(string.IsNullOrEmpty(subsString) && string.IsNullOrEmpty(printString)), Translate.FromKey(XlfKeys.FailedToRetrieveParsePoint));
                    // the printstring should give us what we want
                    if (!string.IsNullOrEmpty(printString))
                    {
                        normalisedTarget = printString;
                    }
                    else
                    {
                        // if not we can use the substring with a bit of tweaking
                        normalisedTarget = subsString;
                        //Debug.Assert(normalisedTarget.Length > 2, Translate.FromKey(XlfKeys.TargetStringTooShort));
                        //Debug.Assert(
                        //    normalisedTarget.StartsWith(@"\??\") && (normalisedTarget[5] == ':' || normalisedTarget.StartsWith(@"\??\Volume")) ||
                        //    !normalisedTarget.StartsWith(@"\??\") && normalisedTarget[1] != ':',
                        //    Translate.FromKey(XlfKeys.MalformedSubsString));
                        //// Junction points must be absolute
                        //Debug.Assert(
                        //        buffer.ReparseTag == IO_REPARSE_TAG_SYMLINK ||
                        //        normalisedTarget.StartsWith(@"\??\Volume") ||
                        //        normalisedTarget[1] == ':',
                        //    Translate.FromKey(XlfKeys.RelativeJunctionPoint));
                        if (normalisedTarget.StartsWith(@"\??\")) normalisedTarget = normalisedTarget.Substring(4);
                    }

                    Target = normalisedTarget;
                    // Symlinks can be relative.
                    if (buffer.ReparseTag == IO_REPARSE_TAG_SYMLINK &&
                        (normalisedTarget.Length < 2 || normalisedTarget[1] != ':'))
                    {
                        // it's relative, we need to tack it onto the path
                        if (normalisedTarget[0] == '\\') normalisedTarget = normalisedTarget.Substring(1);
                        if (path.EndsWith(@"\")) path = path.Substring(0, path.Length - 1);
                        // Need to take the symlink name off the path
                        normalisedTarget = path.Substring(0, path.LastIndexOf('\\')) + @"\" + normalisedTarget;
                        // Note that if the symlink target path contains any ..s these are not normalised but returned as is.
                    }

                    // Remove any final slash for consistency
                    if (normalisedTarget.EndsWith(@"\"))
                        normalisedTarget = normalisedTarget.Substring(0, normalisedTarget.Length - 1);
                }

                CloseHandle(handle);
            }
            else
            {
                success = false;
            }
        }
    }

    /// <summary>
    ///     Gets the actual target string, before normalising
    /// </summary>
    public string? Target { get; } = null;

    /// <summary>
    ///     Gets the tag
    /// </summary>
    public TagType Tag { get; }

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DeviceIoControl(
        nint hDevice,
        uint dwIoControlCode,
        nint lpInBuffer,
        uint nInBufferSize,
        //IntPtr lpOutBuffer, 
        out REPARSE_DATA_BUFFER outBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        nint lpOverlapped);

    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern nint CreateFile(
        string fileName,
        [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
        [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
        int securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        uint flags,
        nint template);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenProcessToken(nint ProcessHandle,
        uint DesiredAccess, out nint TokenHandle);

    [DllImport("kernel32.dll")]
    private static extern nint GetCurrentProcess();

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool LookupPrivilegeValue(string? lpSystemName, string lpName,
        out LUID lpLuid);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AdjustTokenPrivileges(nint TokenHandle,
        [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
        ref TOKEN_PRIVILEGES NewState,
        int BufferLength,
        //ref TOKEN_PRIVILEGES PreviousState,					!! for some reason this won't accept null
        nint PreviousState,
        nint ReturnLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(nint hObject);

    /// <summary>
    ///     This returns the normalised target, ie. if the actual target is relative it has been made absolute
    ///     Note that it is not fully normalised in that .s and ..s may still be included.
    /// </summary>
    /// <returns>The normalised path</returns>
    public override string ToString()
    {
        return normalisedTarget;
    }

    // This is the official version of the data buffer, see http://msdn2.microsoft.com/en-us/library/ms791514.aspx
    // not the one used at http://www.flexhex.com/docs/articles/hard-links.phtml
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct REPARSE_DATA_BUFFER
    {
        public uint ReparseTag;
        public short ReparseDataLength;
        public short Reserved;
        public short SubsNameOffset;
        public short SubsNameLength;
        public short PrintNameOffset;
        public short PrintNameLength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAXIMUM_REPARSE_DATA_BUFFER_SIZE)]
        public char[] ReparseTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LUID
    {
        public uint LowPart;
        public int HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LUID_AND_ATTRIBUTES
    {
        public LUID Luid;
        public uint Attributes;
    }

    private struct TOKEN_PRIVILEGES
    {
        public uint PrivilegeCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)] // !! think we only need one
        public LUID_AND_ATTRIBUTES[] Privileges;
    }
}