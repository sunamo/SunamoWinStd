namespace SunamoWinStd;

[AttributeUsage(
    AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor |
    AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field |
    AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Module |
    AttributeTargets.Property | AttributeTargets.Struct,
    AllowMultiple = true, Inherited = false)]
internal sealed class SupportedOSPlatformAttribute : Attribute
{
    public SupportedOSPlatformAttribute(string platformName) { PlatformName = platformName; }
    public string PlatformName { get; }
}

[AttributeUsage(
    AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor |
    AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field |
    AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Module |
    AttributeTargets.Property | AttributeTargets.Struct,
    AllowMultiple = true, Inherited = false)]
internal sealed class UnsupportedOSPlatformAttribute : Attribute
{
    public UnsupportedOSPlatformAttribute(string platformName) { PlatformName = platformName; }
    public string PlatformName { get; }
}
