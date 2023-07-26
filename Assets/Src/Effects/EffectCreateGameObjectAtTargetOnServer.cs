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
    public class EffectCreateGameObjectAtTargetOnServer : IEffectBinding<EffectCreateGameObjectAtTargetOnServerDef>
    {
        public ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectCreateGameObjectAtTargetOnServerDef indef)
        {
            if (cast.OnServerMaster())
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

        public ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectCreateGameObjectAtTargetOnServerDef def)
        {
            if (cast.OnServerMaster())
                UnityQueueHelper.RunInUnityThread(() => FxPlayer.StopPlay(cast.SpellId, def));
            return new ValueTask();
        }
    }
}