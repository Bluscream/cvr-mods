using Aardwolf;
using MelonLoader;

namespace Bluscream.HTTPServer;
public class Plugin : MelonPlugin {
    internal static Server Server;
    internal static HttpAsyncHost Host;

    public override void OnInitializeMelon() {
        Utils.Logger.Msg("OnInitializeMelon");
        PluginConfig.InitializeMelonPrefs();
        if (PluginConfig.Enabled.Value) Start();
    }

    private void Start() {
        Server = new Server();
        Host = new HttpAsyncHost(Server, PluginConfig.ConnectionsPerCore.Value);
        Host.SetConfiguration(PluginConfig.ToConfigurationDictionary());
        Host.Run(PluginConfig.Prefixes.Value);
    }
    private void Stop() {
        Host = null;
        Server = null;
    }
}
