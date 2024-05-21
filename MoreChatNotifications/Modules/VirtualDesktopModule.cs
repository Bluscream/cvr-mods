using System.Collections;
using System.Diagnostics;
using UnityEngine;
using ABI_RC.Systems.VRModeSwitch;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream.MoreChatNotifications.Modules;

public class VirtualDesktopModule : MonoBehaviour {
    private protected const string VDServerProcessName = "VirtualDesktop.Server";
    private static Process virtualDesktopProcess;
    private static object monitorRoutine = null;
    private static bool isInitialized = false;
    private static bool Enabled = false;

    private void Awake() {
        Mod.Logger.Msg($"Awake");
        if (!isInitialized) {
            Initialize();
            isInitialized = true;
            Mod.Logger.Msg($"isInitialized");
        }
    }

    public static void Initialize() {
        ModuleConfig.InitializeMelonPrefs();
    }

    public static void ToggleMonitor() {
        if (monitorRoutine != null) {
            Mod.Logger.Msg($"old monitorRoutine already running, stopping");
            monitorRoutine = null;
        } else {
            monitorRoutine = MelonCoroutines.Start(MonitorVirtualDesktop());
        }
    }

    private static IEnumerator MonitorVirtualDesktop() {
        Enabled = true;
        Mod.Logger.Msg($"Started {VDServerProcessName} Monitor with interval of {ModuleConfig.VirtualDesktopInterval.Value}s");
        while (Enabled) {
            CheckForVirtualDesktop();
            yield return new WaitForSeconds(ModuleConfig.VirtualDesktopInterval.Value); // Wait for 2 seconds before checking again
        }
        Mod.Logger.Msg($"Stopped {VDServerProcessName} Monitor");
    }

    private static void CheckForVirtualDesktop() {
        var runningProcesses = Process.GetProcessesByName(VDServerProcessName);
        if (runningProcesses.Length > 1) {
            Mod.Logger.Warning($"{VDServerProcessName} found {runningProcesses.Length} times, imploding!");
            return;
        } else if (runningProcesses.Length == 1) {
            var runningProcess = runningProcesses[0];
            if (virtualDesktopProcess == null) {
                virtualDesktopProcess = runningProcess;
                OnVirtualDesktopConnected(runningProcess);
            } else if (runningProcess.Id == virtualDesktopProcess.Id) {
                return;
            } else {
                UnityEngine.Debug.Log($"{VDServerProcessName} found {runningProcesses.Length} times, imploding!");
            }
        } else if (runningProcesses.Length == 0) {
            if (virtualDesktopProcess != null) {
                var oldProcess = virtualDesktopProcess;
                virtualDesktopProcess = null;
                OnVirtualDesktopDisconnected(oldProcess);
            }
        }
    }

    private static void OnVirtualDesktopConnected(Process newProcess) {
        if (!ModConfig.EnableMod.Value || !ModuleConfig.VirtualDesktop.Value || string.IsNullOrWhiteSpace(ModuleConfig.VirtualDesktopConnectedTemplate.Value)) return;
        Mod.Logger.Msg($"VirtualDesktopProcess has connected with pid {newProcess.Id}");
        if (!ModuleConfig.VirtualDesktopExclusive.Value || !VRModeSwitchManager.Instance.IsInXR())
            Mod.SendChatNotification(ModuleConfig.VirtualDesktopConnectedTemplate.Value);
    }

    private static void OnVirtualDesktopDisconnected(Process oldProcess) {
        if (!ModConfig.EnableMod.Value || !ModuleConfig.VirtualDesktop.Value || string.IsNullOrWhiteSpace(ModuleConfig.VirtualDesktopDisconnectedTemplate.Value)) return;
        Mod.Logger.Msg($"VirtualDesktopProcess has disconnected with pid {oldProcess.Id}");
        if (!ModuleConfig.VirtualDesktopExclusive.Value || VRModeSwitchManager.Instance.IsInXR())
            Mod.SendChatNotification(ModuleConfig.VirtualDesktopDisconnectedTemplate.Value);
    }

    public static class ModuleConfig {
        private static MelonPreferences_Category _melonCategory;
        internal static MelonPreferences_Entry<bool> VirtualDesktop;
        internal static MelonPreferences_Entry<float> VirtualDesktopInterval;
        internal static MelonPreferences_Entry<string> VirtualDesktopConnectedTemplate;
        internal static MelonPreferences_Entry<string> VirtualDesktopDisconnectedTemplate;
        internal static MelonPreferences_Entry<bool> VirtualDesktopSound;
        internal static MelonPreferences_Entry<bool> VirtualDesktopExclusive;
        public static void InitializeMelonPrefs() {
            _melonCategory = MelonPreferences.GetCategory(AssemblyInfoParams.Name);
            VirtualDesktop = _melonCategory.CreateEntry("VirtualDesktop notifications", true,
                description: "Will automatically send ChatBox notifications when your VR Headset disconnects from VirtualDesktop while you're in VR mode (VirtualDesktop.Server.exe quits)");
            VirtualDesktop.OnEntryValueChanged.Subscribe((oldValue, newValue) => {
                if (oldValue != false && newValue == false) Enabled = false;
                else if (oldValue != true && newValue == true) Enabled = true;
            });
            VirtualDesktopInterval = _melonCategory.CreateEntry("VirtualDesktop check interval (s)", 1f,
                description: $"How many seconds between checks wether {VDServerProcessName}.exe is (still) running");
            VirtualDesktopConnectedTemplate = _melonCategory.CreateEntry("VirtualDesktop connected template", "VR Connected",
                description: "Template for VirtualDesktop connected notifications");
            VirtualDesktopDisconnectedTemplate = _melonCategory.CreateEntry("VirtualDesktop disconnected template", "VR Disconnected",
                description: "Template for VirtualDesktop disconnected notifications");
            VirtualDesktopSound = _melonCategory.CreateEntry("VirtualDesktop notification sound", false,
                description: "Will play a sound to other users when the notification is sent");
            VirtualDesktopExclusive = _melonCategory.CreateEntry("VirtualDesktop exclusive checking", true,
                description: "Will only show VirtualDesktop notifications when in vr/not in vr mode respectively");
        }
    }
}
