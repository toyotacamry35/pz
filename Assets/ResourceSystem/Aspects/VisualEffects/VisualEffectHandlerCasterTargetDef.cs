using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using ResourceSystem.Reactions;
using UnityEngine;

namespace Assets.Src.Character.Events
{
    public abstract class VisualEffectHandlerCasterTargetDef : BaseResource { }

    public abstract class VisualEffectHandlerCasterTargetWithSubFXDef : VisualEffectHandlerCasterTargetDef
    {
        public ResourceArray<VisualEffectHandlerCasterTargetDef> SubFX { get; set; }
    }
    
    public class SoundStateDef : VisualEffectHandlerCasterTargetDef
    {
        public string SoundEvent;
        public float Duration;
    }

    public class SoundEventDef : VisualEffectHandlerCasterTargetDef
    {
        public string SoundEvent;
        public Dictionary<string, ResourceRef<VarDef>> Params { get; [UsedImplicitly] set; }

    }

    // Use this to place FX with existing parent as root object. If parent is not found script does nothing.
    // If parent is not specified FX will be attached to the nearest transform of current game object.
    public class PlaceParentedFXDef : VisualEffectHandlerCasterTargetWithSubFXDef
    {
        public UnityRef<GameObject> FX { get; set; }
        public float DestroyDelay { get; set; } = 10f; // 0 for disable removing
        public ResourceRef<VarDef<string>> Parent { get; set; }
        public ResourceRef<VarDef<SharedCode.Utils.Vector3>> LocalPosition { get; set; }
        public ResourceRef<VarDef<SharedCode.Utils.Quaternion>> LocalRotation { get; set; }
        public ResourceRef<VarDef<SharedCode.Utils.Vector3>> Position { get; set; }
        public ResourceRef<VarDef<SharedCode.Utils.Quaternion>> Rotation { get; set; }
    }

    public class PlaceUnparentedFXDef : VisualEffectHandlerCasterTargetWithSubFXDef
    {
        public UnityRef<GameObject> FX { get; set; }
        public float DestroyDelay { get; set; } = 10f; // 0 for disable removing
        public SharedCode.Utils.Vector3 Shift { get; set; } = default(SharedCode.Utils.Vector3);
        public bool RelativeToGameObject { get; set; } = false;
        public ResourceRef<VarDef<SharedCode.Utils.Vector3>> Position { get; set; }
        public ResourceRef<VarDef<SharedCode.Utils.Quaternion>> Rotation { get; set; }
    }

    public class PlaceUnparentedFXWithTargetDef : VisualEffectHandlerCasterTargetWithSubFXDef
    {
        public UnityRef<GameObject> FX { get; set; }
        public float DestroyDelay { get; set; } = 10f; // 0 for disable removing
        public SharedCode.Utils.Vector3 Shift { get; set; } = default(SharedCode.Utils.Vector3);
    }

    public class SetFXOnDestroyDef : VisualEffectHandlerCasterTargetDef
    {
        public UnityRef<GameObject> FX { get; set; }
        public float DestroyDelay { get; set; } = 10f; // 0 for disable removing
        public SharedCode.Utils.Vector3 Shift { get; set; } = default(SharedCode.Utils.Vector3);
    }

    public class DisableVisualDef : VisualEffectHandlerCasterTargetDef
    { }
}
