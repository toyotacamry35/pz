using System.Threading.Tasks;
using Assets.Src.App.FXs;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using UnityEngine;
using TimeUnits = System.Int64;


namespace Assets.Src.Effects
{
    [UsedImplicitly]
    public class EffectCreateGameObjectAtPoint : IClientOnlyEffectBinding<EffectCreateGameObjectAtPointDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectCreateGameObjectAtPointDef indef)
        {
            UnityQueueHelper.RunInUnityThread(() =>
            {
                var def = indef;
                var prefab = def.Prefab.Target;
                if (prefab)
                {
                    var parentGo = def.OptionalParentObj.Target?.GetGo(cast);
                    var position = def.AtPoint.Target.GetVec3(cast, Vector3.zero);
                    FxPlayer.StartPlay(prefab, new FXInfo(parentGo, position, Quaternion.identity), cast.SpellId, def);
                }
            });
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectCreateGameObjectAtPointDef def)
        {
            UnityQueueHelper.RunInUnityThread(() => FxPlayer.StopPlay(cast.SpellId, def));
            return new ValueTask();
        }
    }
}
