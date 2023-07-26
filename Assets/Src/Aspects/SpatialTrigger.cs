using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.Aspects
{
    public class SpatialTrigger : MonoBehaviour
    {
        public JdbMetadata OnEnterSpell;
        public JdbMetadata OnExitSpell;
        public JdbMetadata OnExitWithCasterSpell;
        public JdbMetadata WhileInsideSpell;
    }
}
