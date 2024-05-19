using System.Reflection;
using Bluscream.MoreLogging;
using Bluscream.MoreLogging.Properties;
using MelonLoader;


[assembly: AssemblyVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyFileVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyInformationalVersion(AssemblyInfoParams.Version)]
[assembly: AssemblyTitle(AssemblyInfoParams.Name)]
[assembly: AssemblyCompany(AssemblyInfoParams.Author)]
[assembly: AssemblyProduct(AssemblyInfoParams.Name)]

[assembly: MelonInfo(
    typeof(MoreLogging),
    AssemblyInfoParams.Name,
    AssemblyInfoParams.Version,
    AssemblyInfoParams.Author,
    downloadLink: "https://github.com/Bluscream/cvr-mods"
)]
[assembly: MelonGame("Alpha Blend Interactive", "ChilloutVR")]
[assembly: MelonPlatform(MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.MONO)]
[assembly: VerifyLoaderVersion(0, 6, 1, true)]
[assembly: MelonColor(200, 178, 178, 178)]
[assembly: MelonAuthorColor(255, 0, 108, 255)]

namespace Bluscream.MoreLogging.Properties;
internal static class AssemblyInfoParams {
    public const string Name = "More Logging";
    public const string Version = "1.0.1";
    public const string Author = "Bluscream";
}
