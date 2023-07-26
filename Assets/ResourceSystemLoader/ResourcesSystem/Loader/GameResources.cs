using Assets.ResourceSystemLoader.ResourcesSystem.Utils;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json.Converters;

namespace ResourcesSystem.Loader
{
    public class GameResources : IGameResourcesRandomExtension
    {
        public static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public bool LogErrors { get; set; } = true;
        public static readonly bool ThrowExceptions = true;
        public int UniqId { get; }
        public static int _idCounter = 0;
        public event LoadingResourceErrorEventHandler OnLoadingResourceError;

        public static void ThrowError<T>(string message, string file=null, string path=null, int lineNo=0, int linePos=0) where T : Exception
        {
            message = FormatMessage(message, file, path, lineNo, linePos);

            if (ThrowExceptions)
            {
                T ex = (T) Activator.CreateInstance(typeof(T), message);
                throw ex;
            }
            else
            {
                Logger.IfError()?.Message(message).Write();
            }
        }

        public static void ThrowError(Exception e, string file, string path, int lineNo, int linePos)
        {
            if (ThrowExceptions)
            {
                Exception newException = e;
                try
                {
                    newException = new ResourceLoadingException(e, file, path, lineNo, linePos);
                } catch (Exception) {}
                throw newException;
            }
            else
            {
                Logger.IfError()?.Message(FormatMessage(e.Message, file, path, lineNo, linePos)).Write();
            }
        }

        public static void ThrowError(Exception ex)
        {
            if (ThrowExceptions)
            {
                throw ex;
            }
            else
            {
                Logger.IfError()?.Message(ex.MessageWithInnerExceptions()).Write();
            }
        }

        public static void ThrowError(string message, string file=null, string path=null, int lineNo=0, int linePos=0)
        {
            ThrowError<Exception>(message, file, path, lineNo, linePos);
        }

        public static string FormatMessage(string message, string file, string path, int lineNo, int linePos)
        {
            if (!string.IsNullOrEmpty(file))
            {
                var newMessage = $"{message} (at {file}";
                if (lineNo > 0)
                {
                    newMessage += $":{lineNo}";
                    if (linePos > 0)
                        newMessage += $":{linePos}";
                }
                if (!string.IsNullOrEmpty(path))
                    newMessage += $" element {path}";
                newMessage += ")";
                return newMessage;
            }
            return message;
        }

        public LoadingContext Context => Deserializer.Context;
        public JsonSerializer Serializer => Deserializer.Serializer;
        public LoadedResourcesHolder LoadedResources => Deserializer.LoadedResources;

        private NetIDHolder _netIDs;
        private string _name;

        public SaveableIdStorage SaveableStorage { get; private set; }

        public NetIDHolder NetIDs
        {
            get
            {
                if (_netIDs == null)
                {
                    if (ThrowExceptions)
                        ThrowError<InvalidOperationException>("Using netIDs whithout first creating them");
                }

                return _netIDs;
            }
        }

        private readonly ILoader _loader;
        public DeserializerImpl Deserializer { get; }
        public JsonConverterCollection Converters => Serializer.Converters;

        public GameResources(ILoader loader, [CallerFilePath] string name = null)
        {
            UniqId = _idCounter++;
            _name = System.IO.Path.GetFileNameWithoutExtension(name);
            _loader = loader;
            Deserializer = new DeserializerImpl(loader);
            Deserializer.OnLoadingResourceError += (e) =>
            {
                OnLoadingResourceError?.Invoke(e);
                if (LogErrors)
                    Logger.IfError()?.Exception(e).Write();
            };

            // Converters.Add(new BinaryResourceConverter(loader, false));
            Converters.Add(new DefConverter(LoadedResources, Deserializer));
            Converters.Add(TemplateVariableConverter.Instance);
            Converters.Add(TypeConverter.Instance);
            Converters.Add(new DefDictionaryConverter());
            Converters.Add(new ResourceArrayConverter());
            Converters.Add(NonResourcesWithVariables.Instance);

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(GameResources)}() #{UniqId} {_name}").Write();
        }

        public Action<string> Reloaded;

        public void Reload(string filter)
        {
            var resourceToRemove =
                (filter == "*"
                    ? LoadedResources.AllLoaded
                    : LoadedResources.AllLoaded.Where(v => v.Root != null && v.Root.Contains(filter))
                ).ToArray();
            foreach (var resToRemove in resourceToRemove)
                LoadedResources.Unregister(resToRemove);
            Reloaded?.Invoke(filter);
        }

        public ResourceIDFull GetID(IResource res) => LoadedResources.GetID(res).ResId;

        public RidWithNetId GetIDWithCrc(IResource res) => LoadedResources.GetID(res);

        public T TryLoadResourceLogError<T>(string relativePath) where T : IResource
        {
            if (!string.IsNullOrEmpty(relativePath))
                try
                {
                    return Deserializer.LoadResource<T>(relativePath);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                    return default(T);
                }
            else
            {
                Logger.IfError()?.Message($"Resource {relativePath} does not exist.").Write();
                return default(T);
            }
        }

        public T LoadResource<T>(string relativePath) where T : IResource
        {
            if (!string.IsNullOrEmpty(relativePath))
                return Deserializer.LoadResource<T>(relativePath);
            else
            {
                ThrowError<Exception>($"Resource {relativePath} does not exist.");
                return default(T);
            }
        }
        
        public IResource LoadResource(string relativePath, Type type)
        {
            if (!string.IsNullOrEmpty(relativePath))
                return Deserializer.LoadResource(relativePath, type);
            
            ThrowError<Exception>($"Resource {relativePath} does not exist.");
            return type.IsValueType ? (IResource) Activator.CreateInstance(type) : null;
        }

        public T TryLoadResource<T>(string relativePath) where T : IResource
        {
            try
            {
                if (!string.IsNullOrEmpty(relativePath))
                    return Deserializer.LoadResource<T>(relativePath);
            } catch (FileNotFoundException) {}
            return default(T);
        }

        public T LoadResource<T>(ResourceIDFull id) where T : IResource => Deserializer.LoadResource<T>(id);

        public bool IsResourceExists(string relativePath) => Deserializer.IsResourceExists(relativePath);

        public void CreateNetIDs()
        {
            _netIDs = new NetIDHolder(_loader);

            SaveableStorage = new SaveableIdStorage(Deserializer, LoadedResources);
        }

        public static void SimpleSave(
            string dir,
            string refPath,
            IResource data,
            out string path,
            params JsonConverter[] additionalConverters)
        {
            var folderLoader = new FolderLoader(null);
            var gr = new GameResources(folderLoader);

            var extension = data is IBinarySerializable ? FolderLoader.BinaryExtension : FolderLoader.JdbExtension;
            const char separator = '/';
            refPath = refPath.TrimStart(separator);
            dir = string.IsNullOrEmpty(dir) ? "" : dir.TrimEnd(separator);
            path = $"{dir}/{refPath}{extension}";

            if (data == null)
            {
                if (File.Exists(path))
                    File.Delete(path);
                return;
            }

            data.Address = new ResourceIDFull($"/{refPath}");

            dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (data is IBinarySerializable binarySerializable)
                SerializeBinary(binarySerializable, path);
            else
                SerializeJson(data, path, additionalConverters, gr);
        }

        private static void SerializeBinary(IBinarySerializable binarySerializable, string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                binarySerializable.WriteToStream(fileStream);
            }
        }

        private static void SerializeJson(IResource data, string path, JsonConverter[] additionalConverters, GameResources gr)
        {
            var str = new StringWriter();
            var serializer = gr.Serializer;
            serializer.Converters.Clear();
            if (additionalConverters != null)
                foreach (var conv in additionalConverters)
                    serializer.Converters.Add(conv);
            serializer.Converters.Add(new DefReferenceConverter(gr.Deserializer, false));
            serializer.Converters.Add(new StringEnumConverter());
            //serializer.Converters.Add(new DefConverter(LoadedResources));
            //serializer.Converters.Add(NonResourcesWithVariables.Instance);
            //serializer.Converters.Add(TemplateVariableConverter.Instance);
            //serializer.Converters.Add(TypeConverter.Instance);
            serializer.Converters.Add(new DefDictionaryConverter());
            serializer.Converters.Add(new ResourceArrayConverter());
            serializer.TypeNameHandling = TypeNameHandling.Objects;
            serializer.Serialize(str, data);
            File.WriteAllText(path, str.ToString());
        }

        public void LoadAllResources()
        {
            var allRoots = _loader.AllPossibleRoots;
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Load all roots:\n{string.Join("\n", allRoots)}").Write();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var suppressLoadingTrace = DeserializerImpl.SuppressLoadingTrace;
            DeserializerImpl.SuppressLoadingTrace = true;
            try
            {
                foreach (var root in allRoots)
                    if (!_loader.IsBinary(root))
                        LoadResource<IResource>(root);
				AfterAllResourcesAreLoaded();
            }
            finally
            {
                DeserializerImpl.SuppressLoadingTrace = suppressLoadingTrace;
                stopwatch.Stop();
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Loading of all roots takes {stopwatch.Elapsed.TotalSeconds} sec").Write();
            }
        }

        T IGameResourcesRandomExtension.LoadRandomResourceByType<T>(Random random)
        {
            var resourcesOfType = LoadedResources.AllLoadedResources.Where(x => x.Value is T);
            if (!resourcesOfType.Any())
                return default(T);

            var index = random.Next(resourcesOfType.Count());
            var resource = resourcesOfType.Skip(index - 1).FirstOrDefault().Value;
            return (T) resource;
        }

        public override string ToString()
        {
            return $"#{UniqId} {_name} {_loader?.ToString() ?? "<null>"}";
        }

        public static event Action OnAllResourcesAreLoaded;
        public static void AfterAllResourcesAreLoaded()
        {
            Logger.IfWarn()?.Message("#Dbg: After All Resources Are Loaded.").Write();
            OnAllResourcesAreLoaded?.Invoke();
        }

    }

    public interface IGameResourcesRandomExtension
    {
        void LoadAllResources();

        T LoadRandomResourceByType<T>(Random random) where T : IResource;
    }
}