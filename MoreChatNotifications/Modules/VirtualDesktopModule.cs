using System.Collections;
using System.Diagnostics;
using UnityEngine;
using ABI_RC.Systems.VRModeSwitch;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;

namespace Bluscream.MoreChatNotifications.Modules {
    public class VirtualDesktopModule {
        private const string ProcessName = "VirtualDesktop.Server";
        private Process Process;
        public object monitorRoutine = null;

        public void Initialize() {
            ModuleConfig.InitializeMelonPrefs();
        }

        //public void ToggleMonitor() {
        //    if (monitorRoutine != null) {
        //        Mod.Logger.Msg($"old monitorRoutine already running, stopping");
        //        monitorRoutine = null;
        //    } else {
        //        monitorRoutine = MelonCoroutines.Start(MonitorProcess());
        //    }
        //}

        public static IEnumerator MonitorProcess() {
            Mod.Logger.Msg($"Started {ProcessName} Monitor with interval of {ModuleConfig.Interval.Value}s");
            while (ModuleConfig.Enabled.Value) {
                CheckForProcess();
                yield return new WaitForSeconds(ModuleConfig.Interval.Value);
            }
            Mod.Logger.Msg($"Stopped {ProcessName} Monitor");
        }

        private static void CheckForProcess() {
            var runningProcesses = Process.GetProcessesByName(ProcessName);
            if (runningProcesses.Length > 1) {
                Mod.Logger.Warning($"{ProcessName} found {runningProcesses.Length} times, imploding!");
                return;
            } else if (runningProcesses.Length == 1) {
                var runningProcess = runningProcesses[0];
                if (Process == null) {
                    Process = runningProcess;
                    _OnVirtualDesktopConnected(runningProcess);
                } else if (runningProcess.Id == Process.Id) {
                    return;
                } else {
                    UnityEngine.Debug.Log($"{ProcessName} found {runningProcesses.Length} times, imploding!");
                }
            } else if (runningProcesses.Length == 0) {
                if (Process != null) {
                    var oldProcess = Process;
                    Process = null;
                    _OnVirtualDesktopDisconnected(oldProcess);
                }
            }
        }

        public delegate void VirtualDesktopEventHandler(object sender, EventArgs e);
        public event VirtualDesktopEventHandler OnVirtualDesktopConnected;
        public event VirtualDesktopEventHandler OnVirtualDesktopDisconnected;

        private void _OnVirtualDesktopConnected(Process newProcess) {
            if (!ModConfig.EnableMod.Value || !ModuleConfig.Enabled.Value || string.IsNullOrWhiteSpace(ModuleConfig.ConnectedTemplate.Value)) return;
            Mod.Logger.Msg($"VirtualDesktopProcess has connected with pid {newProcess.Id}");
            OnVirtualDesktopConnected?.Invoke(this, EventArgs.Empty);
            if (!ModuleConfig.Exclusive.Value || !VRModeSwitchManager.Instance.IsInXR())
                Mod.SendChatNotification(ModuleConfig.ConnectedTemplate.Value);

        }

        private void _OnVirtualDesktopDisconnected(Process oldProcess) {
            if (!ModConfig.EnableMod.Value || !ModuleConfig.Enabled.Value || string.IsNullOrWhiteSpace(ModuleConfig.DisconnectedTemplate.Value)) return;
            Mod.Logger.Msg($"VirtualDesktopProcess has disconnected with pid {oldProcess.Id}");
            OnVirtualDesktopDisconnected?.Invoke(this, EventArgs.Empty);
            if (!ModuleConfig.Exclusive.Value || VRModeSwitchManager.Instance.IsInXR())
                Mod.SendChatNotification(ModuleConfig.DisconnectedTemplate.Value);

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
                Enabled = Category.CreateEntry("VirtualDesktop notifications", true,
                    description: "Will automatically send ChatBox notifications when your VR Headset disconnects from VirtualDesktop while you're in VR mode (VirtualDesktop.Server.exe quits)");
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
