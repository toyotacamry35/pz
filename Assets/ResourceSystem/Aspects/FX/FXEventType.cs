using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using ResourceSystem.Reactions;
using SharedCode.Utils;

namespace Assets.Src.Character.Events
{
    public class FXEventType : BaseResource
    {
    }
    
    public class HitFXEventType : FXEventType
    {
        [KnownToGameResources]
        public class ParamsBundle
        {
            public ResourceRef<ArgDef<string>> HitObject;
            public ResourceRef<ArgDef<Vector3>> HitPoint;
            public ResourceRef<ArgDef<Quaternion>> HitRotation;
            public ResourceRef<ArgDef<Vector3>> HitLocalPoint;
            public ResourceRef<ArgDef<Quaternion>> HitLocalRotation;
            public ResourceRef<ArgDef<BaseResource>> DamageType;
        }

        public ParamsBundle Params;
    }
}
