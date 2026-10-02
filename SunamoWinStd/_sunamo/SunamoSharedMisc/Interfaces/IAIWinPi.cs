namespace SunamoWinStd._sunamo.SunamoSharedMisc.Interfaces;

/// <summary>
/// Action-injection contract for Windows-specific helpers; kept public because it is part of <see cref="AIWinPiInit.Init"/>.
/// Own copy in a unique namespace so it never collides with the same-named type from other packages.
/// </summary>
public interface IAIWinPi
{
    /// <summary>
    /// Runs the given command line as the interactive desktop user without admin rights.
    /// </summary>
    Action<string> PHWinPiRunAsDesktopUserNoAdmin { get; set; }
}
