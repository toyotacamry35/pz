using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using UnityEngine;

namespace Assets.Src.ResourceSystem
{
    public class JdbMetadata : ScriptableObject
    {
        //Это используется, чтобы не генерить заново файлы из сцен, если те не менялись.
        public long GenerationVersion = 0;
        public ReferenceMapping[] UnityRefs = { };

        public string Type = "";
        public string RootPath = "";

        [NonSerialized] private object _object;

        public IResource Get()
        {
            return Get<IResource>();
        }

        public T Get<T>() where T : class, IResource
        {
            return (_object ?? (_object = GameResourcesHolder.Instance.LoadResource<T>(new ResourceIDFull(RootPath)))) as T;
        }
#if UNITY_EDITOR
        public IResource GetFullTreeCopy()
        {
            return EditorGameResourcesForMonoBehaviours.GetNew().LoadResource<IResource>(RootPath);
        }

        public T GetFullTreeCopy<T>() where T : IResource
        {
            return EditorGameResourcesForMonoBehaviours.GetNew().LoadResource<T>(RootPath);
        }

        public static JdbMetadata GetFromResource(IResource res)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath($"Assets{res.Address.Root}.jdb", typeof(JdbMetadata));
            if (asset == null)
                Debug.LogWarning($"No jdb for {res.Address.ToString()}");
            return (JdbMetadata)asset;
        }
#endif
    }

    [Serializable]
    public struct ReferenceMapping : IEquatable<ReferenceMapping>
    {
        public string Original;
        public string Resolved;

        public ReferenceMapping(string path, string resolved)
        {
            Original = path;
            Resolved = resolved;
        }

        public override bool Equals(object obj)
        {
            return obj is ReferenceMapping && Equals((ReferenceMapping)obj);
        }

        public bool Equals(ReferenceMapping other)
        {
            return Original == other.Original &&
                   Resolved == other.Resolved;
        }

        public override int GetHashCode()
        {
            var hashCode = -1488907986;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Original);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Resolved);
            return hashCode;
        }

        public static bool operator ==(ReferenceMapping mapping1, ReferenceMapping mapping2)
        {
            return mapping1.Equals(mapping2);
        }

        public static bool operator !=(ReferenceMapping mapping1, ReferenceMapping mapping2)
        {
            return !(mapping1 == mapping2);
        }
    }
}
