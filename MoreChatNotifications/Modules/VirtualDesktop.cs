using ABI_RC.Systems.VRModeSwitch;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;
using System.Diagnostics;

namespace Bluscream.MoreChatNotifications.Modules;

internal class VirtualDesktop {
    private protected const string VDServerProcessName = "VirtualDesktop.Server.exe";
    private static Process VirtualDesktopProcess;

    public static void Initialize() {
        ModuleConfig.InitializeMelonPrefs();
    }

    internal static void CheckForVirtualDesktop() {
        if (!ModConfig.EnableMod.Value || !ModuleConfig.VirtualDesktopDisconnected.Value) return;
        VirtualDesktopProcess = Process.GetProcessesByName(VDServerProcessName).FirstOrDefault();
        if (VirtualDesktopProcess != null) {
            Mod.Logger.Msg($"VirtualDesktopProcess found: {VirtualDesktopProcess.Id}");
            VirtualDesktopProcess.Exited += VirtualDesktopProcess_Exited;
        }
    }

    private static void VirtualDesktopProcess_Exited(object sender, EventArgs e) {
        if (!ModConfig.EnableMod.Value || !ModuleConfig.VirtualDesktopDisconnected.Value) return;
        Mod.Logger.Msg("VirtualDesktopProcess_Exited");
        if (VRModeSwitchManager.Instance.IsInXR())
            Mod.SendChatNotification("VirtualDesktop Disconnected");
    }

    public static class ModuleConfig {
        private static MelonPreferences_Category _melonCategory;
        internal static MelonPreferences_Entry<bool> VirtualDesktopDisconnected;
        internal static MelonPreferences_Entry<string> VirtualDesktopDisconnectedTemplate;
        internal static MelonPreferences_Entry<bool> VirtualDesktopDisconnectedSound;
        public static void InitializeMelonPrefs() {
            _melonCategory = MelonPreferences.GetCategory(AssemblyInfoParams.Name);
            VirtualDesktopDisconnected = _melonCategory.CreateEntry("VirtualDesktop notifications", true,
                description: "Will automatically send ChatBox notifications when your VR Headset disconnects from VirtualDesktop while you're in VR mode (VirtualDesktop.Server.exe quits)");
            VirtualDesktopDisconnectedTemplate = _melonCategory.CreateEntry("VirtualDesktop template", "VR Disconnected",
                description: "Template for VirtualDesktop notifications");
            VirtualDesktopDisconnectedSound = _melonCategory.CreateEntry("VirtualDesktop notification sound", false,
                description: "Will play a sound to other users when the notification is sent");
        }
    }
}
