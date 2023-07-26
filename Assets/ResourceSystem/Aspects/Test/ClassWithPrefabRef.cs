using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.Editor
{
    public class ClassWithPrefabRef : BaseResource
    {
        public UnityRef<UnityEngine.GameObject> Data { get; set; }
    }
}
