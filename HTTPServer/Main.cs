using MelonLoader;

namespace Bluscream.HTTPServer;
public class Plugin : MelonPlugin {
    public override void OnInitializeMelon() {
        Utils.Logger.Msg("OnInitializeMelon");
        ModConfig.InitializeMelonPrefs();
        if (ModConfig.Enabled.Value) Server.Start();
    }
}
