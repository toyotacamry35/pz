using System.Threading.Tasks;
using Assets.Src.App.FXs;
using Assets.Src.Lib.Extensions;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Effects
{
    [UsedImplicitly]
    public class EffectCreateGameObjectAtTarget : IClientOnlyEffectBinding<EffectCreateGameObjectAtTargetDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectCreateGameObjectAtTargetDef indef)
        {
            UnityQueueHelper.RunInUnityThread(() =>
            {
                var def = indef;
                var prefab = def.Prefab.Target;
                if (prefab)
                {
                    var parentGo = def.OptionalParentObj.Target?.GetGo(cast);
                    var targetGo = def.TargetToPlaceFxAtItsPosition.Target?.GetGo(cast);
                    var position = def.LocalPosition.ToUnityVector3();
                    if (targetGo != null)
                        position += targetGo.transform.position;
                    FxPlayer.StartPlay(prefab, new FXInfo(parentGo, position, Quaternion.identity), cast.SpellId, def);
                }
            });
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectCreateGameObjectAtTargetDef def)
        {
            UnityQueueHelper.RunInUnityThread(() => FxPlayer.StopPlay(cast.SpellId, def));
            return new ValueTask();
        }
    }
}