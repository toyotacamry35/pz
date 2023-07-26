using Assets.Src.Lib.Cheats;
using Core.Cheats;
using Newtonsoft.Json;
using SharedCode.Utils.Threads;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Uins;
using UnityEngine;

namespace Assets.Src.Client.Impl
{
    public static class LoginInterop
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const string LoopbackCallbackPath = "/";

        /// <summary>The call back format. Expects one port parameter.</summary>
        private static readonly string LoopbackCallback = $"http://localhost:{{0}}{LoopbackCallbackPath}";

        private static IntPtr windowHandle;


        /// <summary>Returns a random, unused port.</summary>
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return ((IPEndPoint) listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        private static string redirectUri;

        private static string RedirectUri
        {
            get
            {
                if (!string.IsNullOrEmpty(redirectUri))
                {
                    return redirectUri;
                }

                return redirectUri = string.Format(LoopbackCallback, GetRandomUnusedPort());
            }
        }

        [Cheat]
        public static void TestGetToken(string authServerAddress)
        {
             Logger.IfInfo()?.Message("Requesting token").Write();;
            TaskEx.Run(async () =>
            {
                var token = await GetLongTermToken(authServerAddress, CancellationToken.None);
                 Logger.IfInfo()?.Message("Got token {0}",  token).Write();
            });
        }

        public static async Task<string> GetLongTermToken(string authServerAddress, CancellationToken cancellationToken, string ltStream = null)
        {
            var redirUriEncoded = WWW.EscapeURL(RedirectUri);
            if (ltStream == null)
                ltStream = "";
            var authorizationUrl = $"{authServerAddress}?redirect_url={redirUriEncoded}&stream={ltStream}";

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(RedirectUri);
                try
                {
                    listener.Start();

                    Console.WriteLine("Listening at {0}", RedirectUri);

                    Console.WriteLine("Open a browser with \"{0}\" URL", authorizationUrl);
                    var urlToStart = authorizationUrl.Replace("&", "^&");
                    using (var proc = OpenBrowser(urlToStart))
                    {
                        // Wait to get the authorization code response.
                        var contextTask = listener.GetContextAsync();

                        await Task.WhenAny(contextTask, Task.Delay(-1, cancellationToken));

                        if (!contextTask.IsCompleted)
                            throw new TaskCanceledException("Cancellation token is fired");

                        var context = await contextTask;

                        var token = Regex.Match(context.Request.Url.Query, @"token=(\S+)").Groups[1].Value;

                        string closePageResponse =
                            $@"<html>
  <head><meta http-equiv='Refresh' content='1; url={
                                    authServerAddress
                                }/success'><title>OAuth 2.0 Authentication Token Received</title></head>
  <body>
    Received verification code. You may now close this window.
    <script type='text/javascript'>
      // This doesn't work on every browser.
      window.setTimeout(function() {{
          window.location.href = '{authServerAddress}/success'; 
        }}, 1000);
    </script>
  </body>
</html>";
                        // Write a "close" response.
                        var buffer = System.Text.Encoding.UTF8.GetBytes(closePageResponse);
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        context.Response.Close();

                        Focus();

                        return token;
                    }
                }
                finally
                {
                    listener.Close();
                }
            }
        }

        public class VerifyUserLoginTokenResponse
        {
            public string status { get; set; }

            public Data data { get; set; }

            public string message { get; set; }

            public override string ToString()
            {
                return $"{nameof(VerifyUserLoginTokenResponse)}: {nameof(status)}='{status}' {nameof(data)}=[{data}]";
            }
            public class ServerData
            {
                public string name { get; set; }
                public string ip { get; set; }
                public string port { get; set; }
            }

            public class FriendServerData
            {
                public string name { get; set; }
                public ServerData server { get; set; }
            }
            public class Data
            {
                public string server_ip { get; set; }
                public int server_port { get; set; }
                public string username { get; set; }
                public string user_token { get; set; }
                public string auth_token { get; set; }
                public string auth_expires { get; set; }
                public ServerData[] previous_servers { get; set; }
                public FriendServerData[] friend_servers { get; set; }
                public override string ToString()
                {
                    return $"{nameof(server_ip)}='{server_ip}', {nameof(server_port)}='{server_port}', {nameof(username)}='{username}'," +
                           $"{nameof(user_token)}='{user_token}', {nameof(auth_token)}='{auth_token}', {nameof(auth_expires)}='{auth_expires}'";
                }
            }
        }

        public static async Task<VerifyUserLoginTokenResponse> VerifyToken(string verifyServerAddress, string longTermToken,
            CancellationToken ct)
        {
            using (HttpClient client = new HttpClient())
            {
                ct.ThrowIfCancellationRequested();

                var resp = await client.GetAsync($"{verifyServerAddress}?token={longTermToken}", ct);
                var data = await resp.Content.ReadAsStringAsync();

                try
                {
                    var result = JsonConvert.DeserializeObject<VerifyUserLoginTokenResponse>(data);
                    return result;
                }
                catch (Exception e)
                {
                    UI.Logger.IfError()?.Message($"Exception: {e}").Write();
                    return new VerifyUserLoginTokenResponse()
                    {
                        message = $"{data}\n{e.Message}"
                    };
                }
            }
        }

        private static Process OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Process.Start(new ProcessStartInfo($"{url}") {CreateNoWindow = true, UseShellExecute = true});
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Process.Start(new ProcessStartInfo("xdg-open", url) {CreateNoWindow = true});
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Process.Start(new ProcessStartInfo("open", url) {CreateNoWindow = true});
            }
            else
            {
                throw new InvalidOperationException("OS is unknown to us");
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private static void Focus()
        {
            SetForegroundWindow(windowHandle);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            windowHandle = GetActiveWindow();
        }
    }
}