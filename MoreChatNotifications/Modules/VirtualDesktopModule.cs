using System.Collections;
using System.Diagnostics;
using UnityEngine;
using ABI_RC.Systems.VRModeSwitch;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream.MoreChatNotifications.Modules {
    public static class VirtualDesktopModule {
        internal const string ProcessName = "VirtualDesktop.Server";
        internal static Process Process;
        public static object monitorRoutine = null;

        public static void Initialize() {
            ModuleConfig.InitializeMelonPrefs();
        }

        public static void ToggleMonitor() {
            if (monitorRoutine != null) {
                MelonLogger.Warning($"old monitorRoutine already running, stopping");
                monitorRoutine = null;
            } else {
                monitorRoutine = MelonCoroutines.Start(MonitorProcess());
            }
        }

        public static IEnumerator MonitorProcess() {
            MelonLogger.Msg($"Started {ProcessName} Monitor with interval of {ModuleConfig.Interval.Value}s");
            while (ModuleConfig.Enabled.Value) {
                CheckForProcess();
                yield return new WaitForSeconds(ModuleConfig.Interval.Value);
            }
            MelonLogger.Msg($"Stopped {ProcessName} Monitor");
        }

        internal static void CheckForProcess() {
            //MelonLogger.Warning($"CheckForProcess");
            var runningProcesses = Process.GetProcessesByName(ProcessName);
            //MelonLogger.Warning($"{ProcessName}={runningProcesses.Length}");
            if (runningProcesses.Length > 1) {
                MelonLogger.Warning($"{ProcessName} found {runningProcesses.Length} times, imploding!");
                return;
            } else if (runningProcesses.Length == 1) {
                var runningProcess = runningProcesses[0];
                if (Process == null) {
                    Process = runningProcess;
                    _OnVirtualDesktopConnected(runningProcess);
                } else if (runningProcess.Id == Process.Id) {
                    //MelonLogger.Warning($"pid same");
                } else {
                    MelonLogger.Warning($"pid change? ({Process.Id}=>{runningProcess.Id})");
                }
            } else if (runningProcesses.Length == 0) {
                if (Process != null) {
                    var oldProcess = Process;
                    Process = null;
                    _OnVirtualDesktopDisconnected(oldProcess);
                }
            }
        }

        public delegate void VirtualDesktopEventHandler(Process process);
        public static event VirtualDesktopEventHandler OnVirtualDesktopConnected;
        public static event VirtualDesktopEventHandler OnVirtualDesktopDisconnected;

        private static void _OnVirtualDesktopConnected(Process newProcess) {
            MelonLogger.Msg($"VirtualDesktopProcess has connected with pid {newProcess.Id}");
            if (!ModConfig.EnableMod.Value || !ModuleConfig.Enabled.Value || string.IsNullOrWhiteSpace(ModuleConfig.ConnectedTemplate.Value)) return;
            OnVirtualDesktopConnected?.Invoke(newProcess);
            if (!ModuleConfig.Exclusive.Value || !VRModeSwitchManager.Instance.IsInXR())
                Utils.SendChatNotification(ModuleConfig.ConnectedTemplate.Value);
        }

        private static void _OnVirtualDesktopDisconnected(Process oldProcess) {
            MelonLogger.Msg($"VirtualDesktopProcess has disconnected with pid {oldProcess.Id}");
            if (!ModConfig.EnableMod.Value || !ModuleConfig.Enabled.Value || string.IsNullOrWhiteSpace(ModuleConfig.DisconnectedTemplate.Value)) return;
            OnVirtualDesktopDisconnected?.Invoke(oldProcess);
            if (!ModuleConfig.Exclusive.Value || VRModeSwitchManager.Instance.IsInXR())
                Utils.SendChatNotification(ModuleConfig.DisconnectedTemplate.Value);
        }

        public static class ModuleConfig {
            private static MelonPreferences_Category Category;
            internal static MelonPreferences_Entry<bool> Enabled;
            internal static MelonPreferences_Entry<float> Interval;
            internal static MelonPreferences_Entry<string> ConnectedTemplate;
            internal static MelonPreferences_Entry<string> DisconnectedTemplate;
            internal static MelonPreferences_Entry<bool> Sound;
            internal static MelonPreferences_Entry<bool> Exclusive;
            public static void InitializeMelonPrefs() {
                Category = MelonPreferences.GetCategory(AssemblyInfoParams.Name);
                Enabled = Category.CreateEntry("VirtualDesktop notifications (Requires admin privileges for ChilloutVR)", true,
                    description: "Will automatically send ChatBox notifications when your VR Headset disconnects from VirtualDesktop while you're in VR mode (VirtualDesktop.Server.exe quits)");
                Enabled.OnEntryValueChanged.Subscribe((_, newValue) => { ToggleMonitor(); });
                Interval = Category.CreateEntry("VirtualDesktop check interval (s)", 1f,
                    description: $"How many seconds between checks whether {ProcessName}.exe is (still) running");
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
}
