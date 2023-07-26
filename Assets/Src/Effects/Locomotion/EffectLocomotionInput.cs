using System.Threading.Tasks;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using Src.Locomotion;
using Src.ManualDefsForSpells;

namespace Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectLocomotionInput : IClientOnlyEffectBinding<EffectLocomotionInputDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectLocomotionInputDef indef)
        {
            if (!cast.OnClientWithAuthority())
                return new ValueTask();
            var def = indef;
            var caster = cast.GetCaster();
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var inputReceiver = caster.GetComponentInChildren(typeof(ILocomotionInputReceiver)) as ILocomotionInputReceiver;
                inputReceiver?.PushInput(cast.WordCastId(def), def.Input, def.Value);
            });            
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectLocomotionInputDef indef)
        {
            if (!cast.OnClientWithAuthority())
                return new ValueTask();
            var def = indef;
            var caster = cast.GetCaster();
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var inputReceiver = caster.GetComponentInChildren(typeof(ILocomotionInputReceiver)) as ILocomotionInputReceiver;
                inputReceiver?.PopInput(cast.WordCastId(def), def.Input);
            });
            return new ValueTask();
        }
    }
}