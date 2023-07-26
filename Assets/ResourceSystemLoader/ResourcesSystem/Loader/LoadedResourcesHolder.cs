using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using Core.Reflection;

namespace ResourcesSystem.Loader
{
    public struct RidWithNetId
    {
        public ResourceIDFull ResId;
        public ulong NetCrc64Id;
    }
    public class LoadedResourcesHolder
    {
        private readonly Dictionary<ResourceIDFull, IResource> _pathsToObjects = new Dictionary<ResourceIDFull, IResource>();
        private readonly Dictionary<IResource, RidWithNetId> _objectsToPaths = new Dictionary<IResource, RidWithNetId>();
        private readonly Dictionary<Guid, ISaveableResource> _saveables = new Dictionary<Guid, ISaveableResource>();

        public IEnumerable<ResourceIDFull> AllLoaded => _pathsToObjects.Keys;
        public IEnumerable<IResource> AllResources => _pathsToObjects.Values;
        public IReadOnlyDictionary<ResourceIDFull, IResource> AllLoadedResources => _pathsToObjects;

        public bool HotLoadWasUsed { get; private set; } = false;
        public LoadedResourcesHolder()
        {
           // _pathsToObjects[default(ResourceIDFull)] = null;
        }

        public ISaveableResource GetLoadedSaveable(Guid id)
        {
            if (_saveables.TryGetValue(id, out var res))
                return res;
            GameResources.ThrowError($"GUID {id} was not found, does not exist or is not loaded");
            return default;
        }

        public RidWithNetId GetID(IResource res)
        {
            lock (this)
            {
                if (res == null)
                    return default(RidWithNetId);

                try
                {
                    return _objectsToPaths[res];

                }
                catch (KeyNotFoundException)
                {
                    GameResources.ThrowError($"Key {res} was not found in the dictionary");
                }

                return default(RidWithNetId);
            }
        }

        public bool GetExisting(ResourceIDFull id, Type type, out IResource res)
        {
            lock (this)
            {
                if (!_pathsToObjects.TryGetValue(id, out res))
                    return false;

                if (res == null)
                    return true;

                if (!type.IsInstanceOfType(res))
                    res = null;

                return true;
            }
        }

        private HashSet<Assembly> _allowedAssemblies;
        private static string[] _allowedAssemblyNames = new string[] {"ResourceSystem"};

        bool IsInProperAsm(Type type)
        {
            if (_allowedAssemblies == null)
            {
                _allowedAssemblies = new HashSet<Assembly>();
                foreach (var asm in AppDomain.CurrentDomain.GetAssembliesSafe().Where(x => _allowedAssemblyNames.Any(y => x.FullName.Contains(y))))
                    _allowedAssemblies.Add(asm);
            }

            return _allowedAssemblies.Contains(type.Assembly);
        }

        internal void RegisterSaveable(ISaveableResource saveable)
        {
            /*if (_saveables.TryGetValue(saveable.Id, out var existing))
            {
                GameResources.ThrowError($"Double registration for saveable with guid {saveable.Id}, id {saveable.Address}, previously seen in {existing.Address}");
            }*/
            _saveables[saveable.Id] = saveable;
        }

        internal void RegisterObject(ResourceIDFull id, BaseResource resource, LoadingContext context)
        {
            lock (this)
            {
                if (!IsInProperAsm(resource.GetType()))
                    GameResources.ThrowError($"{resource.____GetDebugShortName()} {resource.GetType().Name} is not in a proper assembly {resource.GetType().Assembly.FullName}");

                if (_pathsToObjects.ContainsKey(id))
                {
                    GameResources.ThrowError($"Double registration for id {id}, file {context.RootAddress}");
                }
                if (_objectsToPaths.ContainsKey(resource))
                {
                    GameResources.ThrowError($"Double registration for id {id}, file {context.RootAddress}");
                }
                _pathsToObjects[id] = resource;
                _objectsToPaths[resource] = new RidWithNetId(){ResId =  id, NetCrc64Id = id.RootID()};
            }
        }

        internal void Unregister(ResourceIDFull id)
        {
            lock (this)
            {

                HotLoadWasUsed = true;
                IResource res;
                if (!_pathsToObjects.TryGetValue(id, out res))
                    return;

                _pathsToObjects.Remove(id);
                if (res != null)
                    _objectsToPaths.Remove(res);
            }

        }

        public void Clear(string subString)
        {
            _pathsToObjects.RemoveAll((k, v) => k.Root?.Contains(subString) ?? false);
            _objectsToPaths.RemoveAll((k, v) => v.ResId.Root?.Contains(subString) ?? false);
        }
    }
}
