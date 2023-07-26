using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;

namespace Assets.Test.Src.Editor
{
    public class EmptyBundleBuildContent : IBundleBuildContent
    {
        public List<GUID> Assets { get; } = new List<GUID>();
        public List<GUID> Scenes { get; } = new List<GUID>();
        public Dictionary<string, List<GUID>> BundleLayout { get; } = new Dictionary<string, List<GUID>>();
        public Dictionary<GUID, string> Addresses { get; } = new Dictionary<GUID, string>();
        
        //Interface members
        public List<CustomContent> CustomAssets { get; }
        public Dictionary<string, List<ResourceFile>> AdditionalFiles { get; }
    }
}