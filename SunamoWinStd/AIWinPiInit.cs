namespace SunamoWinStd;

/// <summary>
/// Wires the <see cref="IAIWinPi"/> action-injection interface to the concrete
/// Windows-specific implementations in this package.
/// </summary>
public class AIWinPiInit
{
    /// <summary>
    /// Fills <paramref name="target"/>'s delegate properties with the concrete Windows implementations.
    /// </summary>
    /// <param name="target">The instance to initialize.</param>
    public static void Init(IAIWinPi target)
    {
        target.PHWinPiRunAsDesktopUserNoAdmin = PHWinPi.RunAsDesktopUserNoAdmin;
    }
}
