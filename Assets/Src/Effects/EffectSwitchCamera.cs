using System.Threading.Tasks;
using Assets.Src.Camera;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;

namespace Assets.Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectSwitchCamera : IClientOnlyEffectBinding<EffectSwitchCameraDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSwitchCameraDef indef)
        {
            if (cast.OnClientWithAuthority())
                UnityQueueHelper.RunInUnityThread(() =>
                {
                    var def = indef;
                    FindCameraRigHolder(def, cast)?.CameraRig?.ActivateCamera(def.Camera, def);
                });
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSwitchCameraDef indef)
        {
            if (cast.OnClientWithAuthority())
                UnityQueueHelper.RunInUnityThread(() =>
                {
                    var def = indef;
                    FindCameraRigHolder(def, cast)?.CameraRig?.DeactivateCamera(def.Camera, def);
                });
            return new ValueTask();
        }

        private static ICameraRigHolder FindCameraRigHolder(EffectSwitchCameraDef def, SpellWordCastData cast)
        {
            return (def.Target.Target?.GetGo(cast) ?? cast.GetCaster()).GetComponent(typeof(ICameraRigHolder)) as ICameraRigHolder;
        }        
    }
}