using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public static class HttpHelperUtility
    {
        private static readonly Logger Logger = LogManager.GetLogger("Telemetry-PlatformApiRequests");
        private static readonly Logger UiLogger = LogManager.GetLogger("UI");

        public static JsonSerializer Serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };

        class HttpResponceWrapper<T>
        {
            /// <summary>
            /// Данные приходят в этом свойстве, если ERROR
            /// </summary>
            public HttpResponceWrapper<T> error { get; set; }

            /// <summary>
            /// Данные всегда приходят в этом свойстве. 1) OK: в 'data' корневого класса, 2) ERROR: в 'error.data'
            /// </summary>
            public T data { get; set; }

            /// <summary>
            /// Внутри error свойства: description ошибки
            /// </summary>
            public string description { get; set; }

            /// <summary>
            /// Внутри error свойства: code ошибки
            /// </summary>
            public string code { get; set; }
        }

        static HttpClient client = new HttpClient(new HttpClientHandler() {AllowAutoRedirect = false});

        public static async Task<R> OkOnlyHttpSend<R>(
            HttpMethod method,
            string apiHostname,
            string urlHost,
            string apiEndpoint,
            string authToken,
            object message = null)
        {
            var httpResponseData = await HttpSend<R>(true, method, apiHostname, urlHost, apiEndpoint, authToken, message);
            return httpResponseData.IsSuccessStatusCode ? httpResponseData.Data : default;
        }

        public static async Task<HttpResponseData<R>> HttpSend<R>(
            bool okOnlyDataReturn,
            HttpMethod method,
            string apiHostname,
            string urlHost,
            string apiEndpoint,
            string authToken,
            object message = null)
        {
            return await AsyncUtils.RunAsyncTask(
                async () =>
                {
                    var httpResponseData = new HttpResponseData<R>(Guid.NewGuid())
                    {
                        QueryUrl = $"{urlHost}{apiEndpoint}",
                        Method = method,
                        ApiHostName = apiHostname,
                        AuthToken = authToken
                    };

                    if (!IsMethodSupported(method))
                    {
                        Logger.IfError()
                            ?.Message("The method {platform_api_request_method} isn't supported yet {http_response_data}", method, httpResponseData)
                            .Write();
                        return httpResponseData;
                    }

                    var requestMessage = new HttpRequestMessage(method, httpResponseData.QueryUrl);
                    var str = new StringWriter();
                    if (method == HttpMethod.Post || method == HttpMethod.Put)
                    {
                        if (message != null)
                            Serializer.Serialize(str, message);

                        httpResponseData.RequestMessageContent = str.ToString();
                        requestMessage.Content = new StringContent(httpResponseData.RequestMessageContent, Encoding.UTF8, "application/json");
                    }

                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    requestMessage.Headers.Add("X-Request-Id", httpResponseData.RequestGuid.ToString());

                    if (!string.IsNullOrEmpty(apiHostname))
                        requestMessage.Headers.Host = apiHostname;
                    HttpResponceWrapper<R> httpResponseWrapper = null;
                    try
                    {
                        UiLogger.IfInfo()?.Message($"Before send: {httpResponseData}").Write();
                        Logger.Log(LogLevel.Info)
                            .Message(
                                "Pre Request id {platform_api_request_id}: host {platform_api_request_host}, " +
                                "url {platform_api_request_url}, token {platform_api_request_token} ",
                                httpResponseData.RequestGuid,
                                apiHostname,
                                httpResponseData.QueryUrl,
                                authToken)
                            .Property("platform_api_request_message", str.ToString())
                            .Write();
                        var responseMessage = await HttpRequest(client, requestMessage);
                        httpResponseData.IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
                        httpResponseData.StatusCode = responseMessage.StatusCode;
                        httpResponseData.ResponseMessageContent = await responseMessage.Content.ReadAsStringAsync();
                        UiLogger.IfInfo()?.Message($"Before response s11n: {httpResponseData}").Write();

                        if (!(okOnlyDataReturn && !responseMessage.IsSuccessStatusCode) &&
                            !string.IsNullOrWhiteSpace(httpResponseData.ResponseMessageContent))
                        {
                            httpResponseWrapper = JsonConvert.DeserializeObject<HttpResponceWrapper<R>>(httpResponseData.ResponseMessageContent);
                            if (!httpResponseWrapper.Equals(default(HttpResponceWrapper<R>)))
                            {
                                if (responseMessage.IsSuccessStatusCode)
                                {
                                    httpResponseData.Data = httpResponseWrapper.data;
                                }
                                else
                                {
                                    var error = httpResponseWrapper.error;
                                    if (!error.Equals(default(HttpResponceWrapper<R>)))
                                    {
                                        httpResponseData.Data = error.data;
                                        httpResponseData.ErrorCode = error.code;
                                        httpResponseData.ErrorDescription = error.description;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        UiLogger.IfError()?.Message(e.ToString()).Write();
                    }
                    finally
                    {
                        UiLogger.IfInfo()?.Message($"Result: {httpResponseData}").Write();
                        Logger.Log(LogLevel.Info)
                            .Message(
                                "Request id {platform_api_request_id}: " +
                                "host {platform_api_request_host}, " +
                                "url {platform_api_request_url}, " +
                                "token {platform_api_request_token}, " +
                                "is sucess {platform_api_request_is_sucess}, ",
                                httpResponseData.RequestGuid,
                                apiHostname,
                                httpResponseData.QueryUrl,
                                authToken,
                                httpResponseData.IsSuccessStatusCode)
                            .Property("platform_api_request_message", str.ToString())
                            .Property("platform_api_request_response", httpResponseWrapper)
                            .Write();
                    }

                    return httpResponseData;
                });
        }

        private static async Task<HttpResponseMessage> HttpRequest(HttpClient httpClient, HttpRequestMessage request)
        {
            var response = await httpClient.SendAsync(request);
            var statusCode = (int) response.StatusCode;
            // We want to handle redirects ourselves so that we can determine the final redirect Location (via header)
            if (statusCode >= 300 && statusCode <= 399)
            {
                var redirectUri = response.Headers.Location;
                if (!redirectUri.IsAbsoluteUri)
                {
                    redirectUri = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority) + redirectUri);
                }

                var reRequest = new HttpRequestMessage(request.Method, redirectUri);
                reRequest.Content = request.Content;
                foreach (var header in request.Headers)
                {
                    reRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                if (response.Headers.Contains("set-cookie"))
                {
                    request.Headers.Add("Cookie", response.Headers.GetValues("set-cookie"));
                }

                return await HttpRequest(httpClient, reRequest);
            }

            return response;
        }

        /// <summary>
        /// Поддерживается ли method функционалом HttpSend(). Если не поддерживается, добавляем-проверяем, дописываем метод сюда
        /// </summary>
        private static bool IsMethodSupported(HttpMethod method)
        {
            return method != null && (method.Equals(HttpMethod.Post) || method.Equals(HttpMethod.Get) || method.Equals(HttpMethod.Put));
        }
    }
}