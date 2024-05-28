using Bluscream.MoreChatNotifications.Properties;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;

namespace Bluscream.MoreChatNotifications.Modules {
    public static class HTTPServerModule {
        static HttpListener Server;
        public static object monitorRoutine = null;

        public static void Initialize() {
            ModuleConfig.InitializeMelonPrefs();
            Server = new HttpListener();
            foreach (var prefix in ModuleConfig.Prefixes.Value)
                Server.Prefixes.Add(prefix);
        }

        public static void ToggleMonitor() {
            if (monitorRoutine != null) {
                Server.Stop();
                Utils.Warn($"old monitorRoutine already running, stopping");
                monitorRoutine = null;
            } else {
                monitorRoutine = MelonCoroutines.Start(MonitorServer());
            }
        }

        public static IEnumerator MonitorServer() {
            yield return new WaitForSeconds(1);
            Server.Start();
            Utils.Log($"HTTP Server listening on {Extensions.ToString(Server.Prefixes)}");
            while (ModuleConfig.Enabled.Value) {
                var context = Server.GetContextAsync();
                yield return new WaitUntil(() => context.IsCompleted);
                HttpListenerRequest request = context.Result.Request;
                var splitPath = request.Url.AbsolutePath.Trim().Split('/').Select(t => t.ToLowerInvariant()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                //Console.WriteLine(splitPath.ToJson());
                var client = $"{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}";
                var fullUrl = Server.Prefixes.FirstOrDefault() + request.RawUrl.Substring(1);
                var ret = new Dictionary<string, object>();
                ret["request"] = new Dictionary<string, string>() { { "url", fullUrl }, { "ip", client } };
                ret["response"] = new Dictionary<string, string>() { { "status", "success" } };
                Utils.Log($"[{DateTime.Now}] {request.HttpMethod} request from {client} to {fullUrl}");
                switch (splitPath[0]) {
                    case "api":
                        string requestBody; Dictionary<string, object> payload;
                        switch (splitPath[1]) {
                            case "chat":
                                switch (splitPath[2]) {
                                    case "notify":
                                        requestBody = ReadRequestBody(request);
                                        payload = DeserializePayload(requestBody);
                                        Utils.SendChatNotification(payload.GetValue("message"), payload.GetValue("sound", "true").ToBoolean());
                                        break;
                                }
                                break;
                            case "hud":
                                switch (splitPath[2]) {
                                    case "notify":
                                        requestBody = ReadRequestBody(request);
                                        payload = DeserializePayload(requestBody);
                                        Utils.HUDNotify(payload.GetValue("title"), payload.GetValue("message"), "(Remote) " + client);
                                        break;
                                }
                                break;
                            case "log":
                                requestBody = ReadRequestBody(request);
                                payload = DeserializePayload(requestBody);
                                var logger = new MelonLogger.Instance((string)payload.GetValueOrDefault("title", AssemblyInfoParams.Name), color: System.Drawing.Color.FromName((string)payload.GetValueOrDefault("color", "white")));
                                var level = (string)payload.GetValueOrDefault("level", null) ?? (string)payload.GetValueOrDefault("severity", "log");
                                switch (level.ToLowerInvariant()) {
                                    case "error":
                                        logger.Error(payload.GetValue("message"));
                                        break;
                                    case "warn":
                                    case "warning":
                                        logger.Warning(payload.GetValue("message"));
                                        break;
                                    default:
                                        // Utils.Error($"Unknown log level: {level}");
                                        logger.Msg(payload.GetValue("message"));
                                        break;
                                }
                                break;
                        }
                        break;
                }
                if (ModuleConfig.Respond.Value) Respond(context.Result.Response, ret);
            }
            Server.Stop();
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

        public static class ModuleConfig {
            private static MelonPreferences_Category Category;
            internal static MelonPreferences_Entry<bool> Enabled;
            internal static MelonPreferences_Entry<string[]> Prefixes;
            internal static MelonPreferences_Entry<bool> Respond;
            public static void InitializeMelonPrefs() {
                Category = MelonPreferences.GetCategory(AssemblyInfoParams.Name);
                Enabled = Category.CreateEntry("HTTP Server", false,
                    description: "Enables the HTTP Server");
                Enabled.OnEntryValueChanged.Subscribe((_, _) => { ToggleMonitor(); });
                Prefixes = Category.CreateEntry("HTTP Server Prefixes", new[] { "http://*:5111/" },
                    description: "Will automatically send ChatBox notifications when your VR Headset disconnects from VirtualDesktop while you're in VR mode (VirtualDesktop.Server.exe quits)");
                Respond = Category.CreateEntry("HTTP Server Respond", false,
                    description: "Enables HTTP Responses");
            }
        }
    }
}
