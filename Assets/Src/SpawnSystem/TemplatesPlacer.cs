using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    public class TemplatesPlacer : MonoBehaviour
    {
        [System.Serializable]
        public struct TemplateSettings
        {
            public ProceduralTemplate Template;
            public int Count;
        }

        public TemplateSettings[] Settings;
    }
}
