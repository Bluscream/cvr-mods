using System.Text;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using Aardwolf;

namespace Bluscream.HTTPServer;

internal static class Server {
    internal static HttpListener Listener;
    private static object monitorRoutine = null;

    public static void Toggle(bool? enable = null) {
        enable ??= monitorRoutine is null;
        if (enable.Value) Start();
        else Stop();
    }
    internal static bool Start() {
        try {
            Initialize();
            LoadPrefixes();
            StartMonitoring();
            return true;
        } catch (Exception ex) {
            Utils.Logger.Error(ex);
            return false;
        }
    }
    internal static bool Stop() {
        try {
            StopMonitoring();
            Listener = null;
            return true;
        } catch (Exception ex) {
            Utils.Logger.Error(ex);
            return false;
        }
    }
    internal static void Initialize() {
        Listener = new HttpListener();
    }
    internal static void LoadPrefixes() {
        Listener.Prefixes.Clear();
        foreach (var prefix in ModConfig.Prefixes.Value) {
            try {
                Listener.Prefixes.Add(prefix);
            } catch (Exception ex) {
                Utils.Logger.Error($"Failed to add prefix \"{prefix}\": {ex.Message}");
            }
        }
    }
    public static void ToggleMonitoring(bool? enable = null) {
        enable ??= monitorRoutine is null;
        if (enable.Value) StartMonitoring();
        else StopMonitoring();
    }
    private static void StartMonitoring(bool force = false) {
        if (monitorRoutine != null || force) StopMonitoring();
        if (monitorRoutine is null || force) {
            Utils.Logger.Warning("Starting HTTP Server Routine");
            monitorRoutine = MelonLoader.MelonCoroutines.Start(MonitorServer());
        } else {
            Utils.Logger.Error("Tried to start server when it was already running");
        }
    }
    private static void StopMonitoring(bool force = false) {
        if (monitorRoutine != null || force) {
            Utils.Logger.Warning("Stopping HTTP Server Routine");
            Listener.Stop();
            monitorRoutine = null;
        } else {
            Utils.Logger.Error("Tried to stop server when it was already off");
        }
    }

    public static IEnumerator MonitorServer(int delayS = 1) {
        yield return new UnityEngine.WaitForSeconds(delayS);
        Listener.Start();
        Utils.Logger.Msg($"HTTP Server listening on {Extensions.ToString(Listener.Prefixes)}");
        while (ModConfig.Enabled.Value) {
            if (monitorRoutine is null) break;
            var context = Listener.GetContextAsync();
            yield return new UnityEngine.WaitUntil(() => context.IsCompleted);
            HttpListenerRequest request = context.Result.Request;
            var splitPath = request.Url.AbsolutePath.Trim().Split('/').Select(t => t.ToLowerInvariant()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            //Console.WriteLine(splitPath.ToJson());
            var client = $"{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}";
            var fullUrl = Listener.Prefixes.FirstOrDefault() + request.RawUrl.Substring(1);
            var ret = new Dictionary<string, object>();
            ret["request"] = new Dictionary<string, string>() { { "url", fullUrl }, { "ip", client } };
            ret["response"] = new Dictionary<string, string>() { { "status", "success" } };
            Utils.Log($"[{DateTime.Now}] {request.HttpMethod} request from {client} to {fullUrl}");


            var requestBody = ReadRequestBody(request);
            var payload = DeserializePayload(requestBody);

            if (ModConfig.Respond.Value) Respond(context.Result.Response, ret);
        }
        Listener.Stop();
        Utils.Warn($"Stopped HTTP Server loop");
    }

    static void Respond(HttpListenerResponse response, Dictionary<string, object> responseDict) => Respond(response, responseDict.ToJson());
    static void Respond(HttpListenerResponse response, string responseString) {
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
    private static string ReadRequestBody(HttpListenerRequest request) {
        StringBuilder sb = new StringBuilder();
        using (StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8)) {
            char[] buffer = new char[1024];
            int bytesRead;
            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0) {
                sb.Append(buffer, 0, bytesRead);
            }
        }
        return sb.ToString();
    }
    private static Dictionary<string, object> DeserializePayload(string requestBody) {
        try {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(requestBody);
        } catch (Exception ex) {
            Utils.Error($"Error deserializing payload: {ex.Message}");
            return null; // Return null or handle error appropriately
        }
    }
}
