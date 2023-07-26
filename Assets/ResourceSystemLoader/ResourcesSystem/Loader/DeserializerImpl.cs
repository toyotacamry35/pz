using System;
using System.IO;
using System.Runtime.Serialization;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;

namespace ResourcesSystem.Loader
{
    public delegate void LoadingResourceErrorEventHandler(Exception e);

    public interface IResourcesRepository
    {
        T LoadResource<T>(in ResourceIDFull id) where T : IResource;
    }

    public sealed class DeserializerImpl : IResourcesRepository
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static bool SuppressLoadingTrace; 

        private const string TraceJdb = null; //"AttackSimple";

        public ILoader Loader { get; }
        public LoadedResourcesHolder LoadedResources { get; }
        public LoadingContext Context { get; private set; } = new LoadingContext();
        public JsonSerializer Serializer { get; }

        private readonly object _loadLock = new object();

        public event LoadingResourceErrorEventHandler OnLoadingResourceError;

        public T LoadResource<T>(string relativePath) where T : IResource
        {
            return (T) LoadResource(ResourceIDFull.Parse(relativePath), typeof(T));
        }

        public IResource LoadResource(string relativePath, Type type)
        {
            return LoadResource(ResourceIDFull.Parse(relativePath), type);
        }

        T IResourcesRepository.LoadResource<T>(in ResourceIDFull id)
        {
            return (T)LoadResource(id, typeof(T), false, true, false, true);
        }

        public T LoadResource<T>(in ResourceIDFull id) where T : IResource
        {
            return (T) LoadResource(id, typeof(T));
        }

        internal bool IsResourceExists(string relativePath)
        {
            return Loader.IsExists(relativePath);
        }

        public DeserializerImpl(ILoader loader)
        {
            Loader = loader;
            LoadedResources = new LoadedResourcesHolder();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse, HeuristicUnreachableCode
            if (!string.IsNullOrEmpty(TraceJdb))
                Context.Trace = false;

            Serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.Indented,
                ContractResolver = ContractResolver.Instance,
                SerializationBinder = KnownTypesBinder.Instance,
                Context = new StreamingContext(StreamingContextStates.Other, Context)
            };
        }

        private IResource LoadResource(ResourceIDFull id, Type type, bool asProto = false, bool pushProto = false,
            bool resetContextOnError = true, bool directLink = false)
        {
            IResource resource = default;
            lock (_loadLock)
            {
                var relativePath = id.Root;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                var enableTrace = !string.IsNullOrEmpty(TraceJdb) && relativePath.EndsWith(TraceJdb);

                try
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse, HeuristicUnreachableCode
                    if (enableTrace)
                        Context.Trace = true;
                    Context.Depth++;
                    if (Context.Trace)
                        Logger.Trace(
                            "#{1}{2}#> LoadResource({0})", $"{id}, {type}, {asProto}", Context.Depth, Context.Indent
                        );

                    if (!asProto)
                        if (LoadedResources.GetExisting(id, type, out resource))
                        {
                            if (Context.Trace)
                                 Logger.IfTrace()?.Message("#{1}{2}<# Exist: {0}",  resource, Context.Depth, Context.Indent).Write();
                            Context.Depth--;
                            return resource;
                        }

                    if (pushProto && !asProto)
                        Context.PushProto(false);
                    try
                    {
                        if (string.IsNullOrEmpty(relativePath))
                            GameResources.ThrowError<ArgumentException>($"Relative Path is empty");

                        if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith("/"))
                            GameResources.ThrowError<ArgumentException>(
                                $"Relative Path does not starts with forward slash / \"{relativePath}\"");

                        //ProfileSampleBegin("LoadRes", id.Root);
                        resource = LoadFromDisk(relativePath, type);
                        //ProfileSampleEnd();
                    }
                    catch (Exception)
                    {
                        if (resetContextOnError)
                        {
                            Context = new LoadingContext();
                            Serializer.Context = new StreamingContext(StreamingContextStates.Other, Context);
                        }

                        throw;
                    }

                    if (Context.Trace)
                         Logger.IfTrace()?.Message("#{1}{2}<# Loaded: {0}",  resource, Context.Depth, Context.Indent).Write();
                    if (pushProto && !asProto)
                        Context.PopProto();

                    if (directLink)
                    {
                        if (resource == null)
                            GameResources.ThrowError<NullReferenceException>(
                                "Resource is null on path " + relativePath
                            );
                        else if (!type.IsInstanceOfType(resource))
                            GameResources.ThrowError<InvalidCastException>(
                                $"Resource {resource} is not type of {type.FullNiceName()}"
                            );
                    }
                    else
                    {
                        if (LoadedResources.GetExisting(id, type, out resource))
                            return resource;

                        GameResources.ThrowError<NullReferenceException>(
                            "Resource not found on path " + relativePath
                        );
                    }
                }
                catch (Exception e)
                {
                    OnLoadingResourceError?.Invoke(e);
                }
                finally
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (enableTrace)
                        Context.Trace = false;
                    Context.Depth--;
                }
            }

            return resource;
        }

        public IResource LoadResource(in ResourceIDFull id, Type type, bool asProto)
        {
            return LoadResource(id, type, asProto, true, false, true);
        }

        private BaseResource LoadFromDisk(string relativePath, Type type)
        {
            // Loader Checks File Existence Internally Adding .jbd Extension, So Skip Check For Binary
            if (Loader.IsExists(relativePath))
                try
                {
                    if (Logger.IsTraceEnabled && !SuppressLoadingTrace)  Logger.IfTrace()?.Message("{LoadResource}",  new { Path = relativePath, Type = type.NiceName()}).Write();
                    using (var file = Loader.OpenRead(relativePath))
                    {
                        Context.PushLoading(relativePath);
                        var isBinary = typeof(IBinarySerializable).IsAssignableFrom(type);
                        if (isBinary)
                        {
                            var instance = (IBinarySerializable) Activator.CreateInstance(type);
                            instance.ReadFromStream(file);
                            var baseResource = (BaseResource) instance;
                            LoadedResources.RegisterObject(new ResourceIDFull(relativePath), baseResource, Context);
                            return baseResource;
                        }
                        else
                        {
                            using (var streamReader = new StreamReader(file))
                            using (var jsonTextReader = new JsonTextReader(streamReader))
                            {
                                var res = (BaseResource) Serializer.Deserialize<IResource>(jsonTextReader);
                                return res;
                            }
                        }
                    }
                }
                finally
                {
                    Context.PopLoading();
                }

            //GameResources.ThrowError<IOException>("File does not exist: " + relativePath + " on pathes: " + string.Join(", ", Loaders.Select(v => FolderLoader.ToFullPath(v.GetRoot(), relativePath))));
            GameResources.ThrowError<FileNotFoundException>("File does not exist: " + relativePath);
            return default;
        }
    }
}