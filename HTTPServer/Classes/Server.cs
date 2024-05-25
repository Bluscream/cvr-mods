using Aardwolf;
using ABI_RC.Core.Base;
using ABI_RC.Core.Savior;
using MelonLoader;
using SteamAudio;
using UnityEngine.Playables;

namespace Bluscream.HTTPServer;

internal class Server : IHttpAsyncHandler {
    /// <summary>
    /// Main logic.
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public async Task<IHttpResponseAction> Execute(IHttpRequestContext _ctx) {
        var ctx = new API.HTTPRequestContext(_ctx);
        Utils.Log($"[{DateTime.Now}] {ctx.Request.HttpMethod} request from {ctx.ClientAddress} to {ctx.Url}");

        //if (req.HttpMethod != "GET")
        //    return new JsonRootResponse(statusCode: 405, statusDescription: "HTTP Method Not Allowed", message: "HTTP Method Not Allowed");

        var rsp = new API.HTTPResponseAction();
        foreach (var listener in API.Listeners) {
            try {
                rsp = listener.Invoke(ctx);
            } catch (Exception ex) {
                MelonLogger.Error("An mod's interceptor errored :(");
                MelonLogger.Error(ex);
            }
        }
        if (rsp == null) return null;

        // Not a JSON response? Return it:
        var root = rsp as JsonRootResponse;
        if (root == null) return rsp;

        // Check request header X-Exclude:
        string xexclude = req.Headers["X-Exclude"];
        if (xexclude == null) return rsp;

        // Comma-delimited list of response items to exclude:
        string[] excluded = xexclude.Split(',', ' ');
        bool includeLinks = true, includeMeta = true;
        if (excluded.Contains("links", StringComparer.OrdinalIgnoreCase))
            includeLinks = false;
        if (excluded.Contains("meta", StringComparer.OrdinalIgnoreCase))
            includeMeta = false;

        // If nothing to exclude, return the original response:
        if (includeLinks & includeMeta) return rsp;

        // Filter out 'links' and/or 'meta':
        return new JsonRootResponse(
            statusCode: root.statusCode,
            statusDescription: root.statusDescription,
            message: root.message,
            links: includeLinks ? root.links : null,
            meta: includeMeta ? root.meta : null,
            errors: root.errors,
            results: root.results
        );
    }

    async Task<IHttpResponseAction> ProcessRequest(IHttpRequestContext context) {
        var req = context.Request;

        // Capture the current service configuration values only once per connection in case they update during:
        var main = this.services;
        var services = main.Value.Services.Services;

        // Not getting any further than this with severe errors:
        if (main.Value.Services.Errors.Count > 0) {
            return new JsonRootResponse(
                statusCode: 500,
                message: "Severe errors encountered",
                errors: main.Value.Services.Errors.ToArray()
            );
        }

        // Split the path into component parts:


        if (path.Length == 0) {
            // Our default response when no action is given:
            return new JsonRootResponse(
                links: new RestfulLink[]
                {
                        RestfulLink.Create("config", "/config"),
                        RestfulLink.Create("meta", "/meta"),
                        RestfulLink.Create("errors", "/errors"),
                        RestfulLink.Create("debug", "/debug")
                },
                meta: new {
                    configHash = main.HashHexString
                }
            );
        }

        string actionName = path[0];

        // Requests are always of one of these forms:
        //  * /{action}
        //  * /{action}/{service}
        //  * /{action}/{service}/{method}

        try {
            if (path.Length == 1) {
                if (actionName == "data") {
                    return new RedirectResponse("/meta");
                } else if (actionName == "meta") {
                    return metaAll(main);
                } else if (actionName == "debug") {
                    return debugAll(main);
                } else if (actionName == "config") {
                    return configAll(main);
                } else if (actionName == "errors") {
                    return errorsAll(main);
                } else {
                    return new JsonRootResponse(
                        statusCode: 400,
                        statusDescription: "Unknown action",
                        message: "Unknown action '{0}'".F(actionName),
                        meta: new {
                            configHash = main.HashHexString,
                        },
                        errors: new[]
                        {
                                new { actionName }
                        }
                    );
                }
            }

            // Look up the service name:
            string serviceName = path[1];

            Service service;
            if (!main.Value.Services.Services.TryGetValue(serviceName, out service))
                return new JsonRootResponse(
                    statusCode: 400,
                    statusDescription: "Unknown service name",
                    message: "Unknown service name '{0}'".F(serviceName),
                    meta: new {
                        configHash = main.HashHexString
                    },
                    errors: new[]
                    {
                            new { serviceName }
                    }
                );

            if (path.Length == 2) {
                if (actionName == "data") {
                    return new RedirectResponse("/meta/{0}".F(serviceName));
                } else if (actionName == "meta") {
                    return metaService(main, service);
                } else if (actionName == "debug") {
                    return debugService(main, service);
                } else if (actionName == "errors") {
                    // Report errors encountered while building a specific service descriptor.
                    return errorsService(main, service);
                } else {
                    return new JsonRootResponse(
                        statusCode: 400,
                        statusDescription: "Unknown request type",
                        message: "Unknown request type '{0}'".F(actionName),
                        meta: new {
                            configHash = main.HashHexString
                        },
                        errors: new[]
                        {
                                new { actionName }
                        }
                    );
                }
            }

            if (path.Length > 3) {
                return new JsonRootResponse(
                    statusCode: 400,
                    statusDescription: "Too many path components supplied",
                    message: "Too many path components supplied",
                    meta: new {
                        configHash = main.HashHexString
                    }
                );
            }

            // Find method:
            string methodName = path[2];
            Method method;
            if (!service.Methods.TryGetValue(methodName, out method))
                return new JsonRootResponse(
                    statusCode: 400,
                    statusDescription: "Unknown method name",
                    message: "Unknown method name '{0}'".F(methodName),
                    meta: new {
                        configHash = main.HashHexString
                    },
                    errors: new[]
                    {
                            new { methodName }
                    }
                );

            if (actionName == "data") {
                // Await execution of the method and return the results:
                var result = await dataMethod(main, method, req.QueryString);
                return result;
            } else if (actionName == "meta") {
                return metaMethod(main, service, method);
            } else if (actionName == "debug") {
                return debugMethod(main, service, method);
            } else if (actionName == "errors") {
                // Report errors encountered while building a specific method:
                return errorsMethod(main, service, method);
            } else {
                return new JsonRootResponse(
                    statusCode: 400,
                    statusDescription: "Unknown request type",
                    message: "Unknown request type '{0}'".F(actionName),
                    meta: new {
                        configHash = main.HashHexString
                    },
                    errors: new[]
                    {
                            new { actionName }
                    }
                );
            }
        } catch (Exception ex) {
            return getErrorResponse(ex);
        }
    }

}
