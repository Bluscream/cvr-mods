using ABI_RC.Systems.IK;
using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;
using System.Text;
using Valve.VR;

namespace Bluscream.MoreChatNotifications.Modules {
    public static class TrackerBatteryModule {
        internal static Dictionary<string, float> Cache = new();
        public static object monitorRoutine = null;

        public static void Initialize() {
            ModuleConfig.InitializeMelonPrefs();
        }

        public static string GetHmdName() {
            var sb = new StringBuilder();
            var modules = IKSystem.Instance.TrackingSystem._trackingModules;
            foreach (var module in modules) {
                var _trackers = module.TrackingPoints;
                foreach (var tracker in _trackers) {
                    sb.AppendLine($"{module.GetType().Name} Tracker: {tracker.name} {tracker.identifier} {tracker.assignedRole}/{tracker.suggestedRole} {tracker.deviceName} {tracker.batteryPercentage}");
                }
            }
            var trackers = IKSystem.Instance.TrackingSystem.AllTrackingPoints;
            foreach (var tracker in trackers) {
                sb.AppendLine($"Tracker: {tracker.name} {tracker.identifier} {tracker.assignedRole}/{tracker.suggestedRole} {tracker.deviceName} {tracker.batteryPercentage}");
            }
            return sb.ToString();
        }

        public static float GetDeviceBattery() {
            ETrackedPropertyError etrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
            return OpenVR.System.GetFloatTrackedDeviceProperty(1, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref etrackedPropertyError);
        }

        public static class ModuleConfig {
            private static MelonPreferences_Category Category;
            public static void InitializeMelonPrefs() {
                Category = MelonPreferences.GetCategory(AssemblyInfoParams.Name);
            }
        }
    }
}
