using System.Diagnostics;
using System.Security.Principal;
using Aardwolf;
using cohtml.Net;
using MelonLoader;

namespace Bluscream.HTTPServer;

public static class API {

    internal static readonly HashSet<Func<HTTPRequestContext, HTTPResponseAction>> Listeners = new();

    public static void AddListener(Func<HTTPRequestContext, HTTPResponseAction> listener) => Listeners.Add(listener);
    public static void RemoveListener(Func<HTTPRequestContext, HTTPResponseAction> listener) => Listeners.Remove(listener);

    public class HTTPResponseAction : IHttpResponseAction {
        public readonly string ModName;
        public HTTPResponseAction() {
            ModName = GetModName();
        }
        public Task Execute(IHttpRequestResponseContext context) {

        }
    }
    public class HTTPRequestContext : IHttpRequestContext {
        public IHttpAsyncHostHandlerContext HostContext { get; }
        public System.Net.HttpListenerRequest Request { get; }
        public IPrincipal User { get; }
        public string[] Path { get; } = [];
        public string ClientAddress { get; }
        public Uri Url { get; }
        internal HTTPRequestContext(IHttpRequestContext ctx) {
            HostContext = ctx.HostContext; Request = ctx.Request; User = ctx.User;
            if (ctx.Request.Url.AbsolutePath != "/")
                Path = ctx.Request.Url.AbsolutePath.Substring(1).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries); // = req.Url.AbsolutePath.Trim().Split('/').Select(t => t.ToLowerInvariant()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            ClientAddress = $"{ctx.Request.RemoteEndPoint.Address}:{ctx.Request.RemoteEndPoint.Port}";
            Url = new Uri(PluginConfig.Prefixes.Value.FirstOrDefault() + ctx.Request.RawUrl.Substring(1));
        }
    }

    private static string GetModName() {
        try {
            var callingFrame = new StackTrace().GetFrame(2);
            var callingAssembly = callingFrame.GetMethod().Module.Assembly;
            var callingMelonAttr = callingAssembly.CustomAttributes.FirstOrDefault(
                attr => attr.AttributeType == typeof(MelonInfoAttribute));
            return (string)callingMelonAttr!.ConstructorArguments[1].Value;
        } catch (Exception ex) {
            MelonLogger.Error("[GetModName] Attempted to get a mod's name...");
            MelonLogger.Error(ex);
        }
        return "N/A";
    }
}
