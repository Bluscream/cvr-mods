using System.Collections;
using System.Diagnostics;
using UnityEngine;
using MelonLoader;

namespace Bluscream;

internal abstract class ProcessMonitorModule : ModuleBase {
    protected readonly string ProcessName;
    protected Process Process;
    protected object monitorRoutine = null;
    protected new ProcessMonitorModuleConfig Config;

    public delegate void ProcessStartedHandler(Process newProcess);
    public delegate void ProcessExitedHandler(Process oldProcess);
    public static event ProcessStartedHandler ProcessStarted;
    public static event ProcessExitedHandler ProcessExited;

    protected ProcessMonitorModule(string processName) : base("Process Monitor") {
        ProcessName = processName;
    }

    internal override void Initialize() {
        Config = new ProcessMonitorModuleConfig();
        Config.Initialize(this);
    }

    internal virtual void ToggleMonitor() {
        if (monitorRoutine != null) {
            MelonLogger.Msg($"[{Name}] Old monitorRoutine already running, stopping");
            monitorRoutine = null;
        } else {
            monitorRoutine = MelonCoroutines.Start(MonitorProcess());
        }
    }

    protected virtual IEnumerator MonitorProcess() {
        MelonLogger.Msg($"Started {ProcessName} Monitor with interval of {Config.Interval.Value}s");
        while (monitorRoutine != null) {
            CheckForProcess();
            yield return new WaitForSeconds(Config.Interval.Value);
        }
        MelonLogger.Msg($"Stopped {ProcessName} Monitor");
    }

    protected virtual void CheckForProcess() {
        var runningProcesses = Process.GetProcessesByName(ProcessName);
        if (runningProcesses.Length > 1) {
            MelonLogger.Warning($"{ProcessName} found {runningProcesses.Length} times, imploding!");
            return;
        } else if (runningProcesses.Length == 1) {
            var runningProcess = runningProcesses[0];
            if (Process == null) {
                Process = runningProcess;
                OnProcessStarted(runningProcess);
            } else if (runningProcess.Id == Process.Id) {
                return;
            } else {
                MelonLogger.Msg($"{ProcessName} changed pid from {Process.Id} to {runningProcess.Id}");
            }
        } else if (runningProcesses.Length == 0) {
            if (Process != null) {
                var oldProcess = Process;
                Process = null;
                OnProcessExited(oldProcess);
            }
        }
    }

    protected virtual void OnProcessStarted(Process newProcess) {
        ProcessStarted?.Invoke(newProcess);
    }
    protected virtual void OnProcessExited(Process oldProcess) {
        ProcessExited?.Invoke(oldProcess);
    }

    public class ProcessMonitorModuleConfig : ModuleConfigBase {
        internal MelonPreferences_Entry<float> Interval;
        internal virtual void Initialize(ProcessMonitorModule moduleBase, bool enabled = true) {
            base.Initialize(moduleBase, enabled);
            Interval = Category.CreateEntry("Process check interval (s)", 1f,
                description: $"How many seconds between checks whether {moduleBase.ProcessName}.exe is (still) running");
        }
    }
}
