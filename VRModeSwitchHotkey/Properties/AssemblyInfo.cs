using System.Reflection;
using Bluscream.VRModeSwitchHotkey;
using Bluscream.VRModeSwitchHotkey.Properties;
using MelonLoader;


[assembly: AssemblyVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyFileVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyInformationalVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyTitle(AssemblyInfoParams.Name)]
[assembly: AssemblyCompany(AssemblyInfoParams.Author)]
[assembly: AssemblyProduct(AssemblyInfoParams.Name)]

[assembly: MelonInfo(
    typeof(VRModeSwitchHotkey),
    AssemblyInfoParams.Name,
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/Bluscream/cvr-mods"
)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: VerifyLoaderVersion(0, 6, 1, true)]
[assembly: MelonColor(200, 255, 0, 0)]
[assembly: MelonAuthorColor(255, 0, 108, 255)]

namespace Bluscream.VRModeSwitchHotkey.Properties;
internal static class AssemblyInfoParams {
    public const string Name = "VR Mode Switch Hotkey";
    public const string Version = "1.0.0";
    public const string Author = "Bluscream";
}
