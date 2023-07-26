using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.Character.Effects
{
    public abstract class FXMaterialProperty : MonoBehaviour
    {
        /// <summary>
        /// This field is implicitly used in FXMaterialController
        /// </summary>
        public JdbMetadata MaterialDef;
    }
}
