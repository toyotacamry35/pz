using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using SharedCode.Utils.Threads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.Cluster.Cheats;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Environment.Logging;
using Assets.ColonyShared.SharedCode.Shared;
using ColonyShared.SharedCode.Utils;
using Core.Cheats;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Debugging.Log
{
    public class RestServer : IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        private readonly HttpListener _listener = new HttpListener();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public int Port { get; }

        private string _guid = UniquePlayerPrefs.GetString("RestServerGuid", Guid.NewGuid().ToString());

        private readonly SyncedMemoryTarget _target;

        private readonly bool _editorMode;

        private static T Unwrap<T>(NLog.Targets.Target target) where T : NLog.Targets.Target
        {
            var curTarget = target;
            while (curTarget != null)
            {
                var reqTarget = curTarget as T;
                if (reqTarget != null)
                    return reqTarget;

                var wrap = curTarget as NLog.Targets.Wrappers.WrapperTargetBase;
                curTarget = wrap?.WrappedTarget;
            }
            return null;
        }

        public static RestServer Create(bool isEditor)
        {
            var target = Unwrap<SyncedMemoryTarget>(LogManager.Configuration.FindTargetByName("memory"));
            if (target == null)
            {
                Logger.IfWarn()?.Message("Cant find memory target - skipping rest server init").Write();
                return null;
            }
            return new RestServer(target, isEditor);
        }

        private RestServer(SyncedMemoryTarget target, bool isEditor)
        {
            _editorMode = isEditor;
            _target = target;

            int tries = 0;
            while (tries < 100)
            {
                int port = 8080 + tries++;
                try
                {
                    _listener.Prefixes.Clear();
                    _listener.Prefixes.Add($"http://*:{port}/");
                    _listener.Start();
                    Port = port;
                    if (_cancellationTokenSource != null)
                        TaskEx.Run(() => Listen(_cancellationTokenSource.Token)).WrapErrors();
                    Logger.IfInfo()?.Message("Initialized REST server on port {0}", port).Write();
                    return;
                }
                catch (Exception)
                {
                }
            }
            Logger.IfError()?.Message("Failed to initialize REST server").Write();
        }

        public void Dispose()
        {
            if (!_listener.IsListening)
                return;

            _cancellationTokenSource.Cancel();
            _listener.Close();
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try 
                {
                    var context = await _listener.GetContextAsync();
                    ProcessRequest(context);
                }
                catch(Exception)
                {
                    //log
                }
            }

        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            if (context.Request.HttpMethod == "GET" && context.Request.Url.LocalPath == "/Log")
            {
                var minStr = context.Request.QueryString["KnownMin"];
                var maxStr = context.Request.QueryString["KnownMax"];
                int.TryParse(minStr, out var min);
                int.TryParse(maxStr, out var max);
                SendLogs(context.Response, min, max);
                return;
            }
            if (context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath == "/Command")
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    var cmd = await reader.ReadToEndAsync();
                    await IssueCommand(context.Response, cmd);
                }
                return;
            }
            if (context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath == "/AutoComplete")
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    var cmd = await reader.ReadToEndAsync();
                    await Autocomplete(context.Response, cmd);
                }
                return;
            }
            if (context.Request.HttpMethod == "GET" && context.Request.Url.LocalPath == "/Status")
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    await reader.ReadToEndAsync();
                    await Status(context.Response);
                }
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "text/plain";
            using (var writer = new StreamWriter(context.Response.OutputStream))
                writer.Write("Not found");
            context.Response.Close();
        }


        public struct LogEntry
        {
            public int Index { get; set; }

            public DateTime DateTime { get; set; }

            public string Level { get; set; }

            public string Message { get; set; }

            public string StackTrace { get; set; }

            public LogEntry(Core.Environment.Logging.LogEntry entry)
            {
                Index = entry.Index;
                DateTime = entry.DateTime;
                Level = entry.Level;
                Message = entry.Message;
                StackTrace = entry.StackTrace?.ToString() ?? "";
            }
        }


        private static readonly Queue<LogEntry> NullEntries = new Queue<LogEntry>(new[] { new LogEntry() { DateTime = DateTime.Now, Level = "Error", Index = 0, Message = "No memory appender" } });

        private void SendLogs(HttpListenerResponse resp, int min, int max)
        {
            resp.ContentType = "application/json";
            using (var stream = new StreamWriter(resp.OutputStream))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                if(_target == null)
                {
                    var unknown = NullEntries.Where(v => v.Index < min || v.Index >= max).Reverse().Take(1000);
                    _jsonSerializer.Serialize(writer, unknown);
                }
                else
                {
                    lock (_target.Logs)
                    {
                        var unknown = _target.Logs.Where(v => v.Index < min || v.Index >= max).Reverse().Select(v => new LogEntry(v)).Take(1000);
                        _jsonSerializer.Serialize(writer, unknown);

                    }
                }
            }
            resp.Close();
        }

        private async Task IssueCommand(HttpListenerResponse resp, string command)
        {
            resp.ContentType = "application/json";
            using (var stream = new StreamWriter(resp.OutputStream))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                var result = await UnityQueueHelper.RunInUnityThread(() => CheatExecutor.Execute(NodeAccessor.Repository, Guid.Empty, command));
                _jsonSerializer.Serialize(writer, result);
            }

            resp.Close();
        }

        private Task Autocomplete(HttpListenerResponse resp, string command)
        {
            resp.ContentType = "application/json";
            using (var stream = new StreamWriter(resp.OutputStream))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                var allAutoCompletes = CheatsManager.TryAutocompleteCommand(command).Select(v => v.Name).ToArray();
                _jsonSerializer.Serialize(writer, allAutoCompletes);
            }
            resp.Close();
            return Task.CompletedTask;
        }

        public class ProcessStatus
        {
            public string Guid { get; set; }
            public string Info { get; set; }
            public bool IsEditor { get; set; }
            public bool IsPlaying { get; set; }
            public bool IsHeadless { get; set; }
            public int DebugPort { get; set; }
            public string[] LoadedMaps { get; set; }
            public int ProcessID { get; set; }
        }


        private string[] GetMaps()
        {
            var maps = Enumerable.Range(0, SceneManager.sceneCount).Select(v => SceneManager.GetSceneAt(v)).Where(v => v.isLoaded).Select(v => v.name).ToArray();
            return maps;
        }

        private ProcessStatus GetStatusEditor()
        {
            var results = new ProcessStatus()
            {
                Guid = _guid,
                Info = "OK",
                LoadedMaps = Array.Empty<string>(),
                IsEditor = true,
                IsPlaying = false,
                IsHeadless = false,
                DebugPort = Process.GetCurrentProcess().Id % 1000 + 56000,
                ProcessID = Process.GetCurrentProcess().Id
            };
            return results;
        }

        private async Task<ProcessStatus> GetStatusPlayer()
        {
            var portArg = Environment.GetCommandLineArgs().SkipWhile(v => string.Compare(v, "-debugPort", true) != 0).Skip(1).FirstOrDefault();
            var port = string.IsNullOrWhiteSpace(portArg) ? -1 : int.Parse(portArg);

            var results = await UnityQueueHelper.RunInUnityThread(() =>
            {
                return new ProcessStatus()
                {
                    Guid = _guid,
                    Info = "OK",
                    LoadedMaps = GetMaps(),
                    IsEditor = Application.isEditor,
                    IsPlaying = Application.isPlaying,
                    IsHeadless = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null,
                    DebugPort = port,
                    ProcessID = Process.GetCurrentProcess().Id
                };
            });
            return results;

        }

        private async Task Status(HttpListenerResponse resp)
        {
            resp.ContentType = "application/json";
            using (var stream = new StreamWriter(resp.OutputStream))
            using (JsonWriter writer = new JsonTextWriter(stream))
            {
                var results = _editorMode ? GetStatusEditor() : await GetStatusPlayer();
                _jsonSerializer.Serialize(writer, results);
            }
            resp.Close();
        }
    }
}
