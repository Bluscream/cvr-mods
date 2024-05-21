using System.Diagnostics;
using ABI_RC.Systems.VRModeSwitch;
using MelonLoader;

namespace Bluscream.MoreChatNotifications.Modules;

internal class VirtualDesktopModule : ProcessMonitorModule {
    protected new ModuleConfig Config;

    public VirtualDesktopModule() : base("VirtualDesktop.Server") { }

    internal override void Initialize() {
        Config = new ModuleConfig();
        Config.Initialize(this);
        ProcessStarted += VirtualDesktopModule_ProcessStarted;
        ProcessExited += VirtualDesktopModule_ProcessExited;
    }

    private void VirtualDesktopModule_ProcessStarted(Process newProcess) {
        if (!ModConfig.EnableMod.Value || !Config.Enabled.Value || string.IsNullOrWhiteSpace(Config.ConnectedTemplate.Value)) return;
        Mod.Logger.Msg($"VirtualDesktopProcess has connected with pid {newProcess.Id}");
        if (!Config.Exclusive.Value || !VRModeSwitchManager.Instance.IsInXR())
            Mod.SendChatNotification(Config.ConnectedTemplate.Value);
    }

    private void VirtualDesktopModule_ProcessExited(Process oldProcess) {
        if (!ModConfig.EnableMod.Value || !Config.Enabled.Value || string.IsNullOrWhiteSpace(Config.DisconnectedTemplate.Value)) return;
        Mod.Logger.Msg($"VirtualDesktopProcess has disconnected with pid {oldProcess.Id}");
        if (!Config.Exclusive.Value || VRModeSwitchManager.Instance.IsInXR())
            Mod.SendChatNotification(Config.DisconnectedTemplate.Value);
    }

    public class ModuleConfig : ProcessMonitorModuleConfig {
        internal MelonPreferences_Entry<string> ConnectedTemplate;
        internal MelonPreferences_Entry<string> DisconnectedTemplate;
        internal MelonPreferences_Entry<bool> Sound;
        internal MelonPreferences_Entry<bool> Exclusive;
        internal new void Initialize(ProcessMonitorModule moduleBase, bool enabled = true) {
            base.Initialize(moduleBase, enabled);
            ConnectedTemplate = Category.CreateEntry("VirtualDesktop connected template", "VR Connected",
                description: "Template for VirtualDesktop connected notifications");
            DisconnectedTemplate = Category.CreateEntry("VirtualDesktop disconnected template", "VR Disconnected",
                description: "Template for VirtualDesktop disconnected notifications");
            Sound = Category.CreateEntry("VirtualDesktop notification sound", false,
                description: "Will play a sound to other users when the notification is sent");
            Exclusive = Category.CreateEntry("VirtualDesktop exclusive checking", true,
                description: "Will only show VirtualDesktop notifications when in vr/not in vr mode respectively");
        }
    }
}
