using ABI_RC.Core.Savior;
using ABI_RC.Systems.GameEventSystem;
using UnityEngine.SceneManagement;
using MelonLoader;

namespace Bluscream.PropSpawner;

public class PropSpawner : MelonMod {
    public override void OnInitializeMelon() {
        ModConfig.InitializeMelonPrefs();

        CVRGameEventSystem.Instance.OnConnected.AddListener(instance => {
            if (!ModConfig.EnableMod.Value) return;
            Task.Factory.StartNew(() => CommonMethods.SpawnProps());
        });
    }

    internal static class CommonMethods {
        internal static void SpawnProps(string? worldId = null) {
            if (!ModConfig.EnableMod.Value) return;
            worldId ??= MetaPort.Instance.CurrentWorldId;
        }
    }
}
