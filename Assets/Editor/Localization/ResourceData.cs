using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem;
using UnityEditor;

namespace L10n
{
    public class ResourceData
    {
        public JdbMetadata JdbMetadata;
        public IResource BaseResource;

        private string _relPath;
        private GameResources _gameResources;


        //=== Props ===========================================================

        public string HierPath { get; }


        //=== Ctor ============================================================

        public ResourceData(string relPath, GameResources gameResources, string hierPath = null)
        {
            _relPath = relPath;
            HierPath = hierPath;
            _gameResources = gameResources;
            Reload();
        }


        //=== Public ==========================================================

        public void Reload()
        {
            BaseResource = _gameResources.LoadResource<IResource>(_relPath);
            JdbMetadata = GetJdbMetadata(_relPath);
        }


        //=== Private =========================================================

        private JdbMetadata GetJdbMetadata(string relPath)
        {
            var obj = AssetDatabase.LoadAssetAtPath($"Assets{relPath}.jdb", typeof(JdbMetadata));
            return obj == null ? null : (JdbMetadata) obj;
        }
    }
}