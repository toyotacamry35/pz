using System.Collections;
using System.Collections.Generic;
using Assets.Src.Character.Events;
using Assets.Src.ManualDefsForSpells;
using Assets.Src.Shared.Impl;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Entities.Reactions;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ResourceSystem.Reactions;
using UnityEngine;
using Array = System.Array;

namespace Assets.Src.Effects.FX
{
    [UsedImplicitly, PredictableEffect]
    public class EffectPostVisualEvent : IClientOnlyEffectBinding<EffectPostVisualEventDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPostVisualEventDef def)
        {
            if (cast.OnClient())
            {
                GameObject castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
                if (castergo)
                {
                    OuterRef<IEntity> targetEntityRef = await def.Target.Target.GetOuterRef(cast, repo);
                    if (!targetEntityRef.IsValid)
                        if (cast.CastData.TryGetParameter<SpellCastParameterTarget>(out var p))
                            targetEntityRef = p.Target;
                    GameObject targetgo = targetEntityRef.IsValid ? GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(targetEntityRef) : null;
                    var @params = await MapParameters(cast, def.Params, repo);
                    UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                            VisualEventProxy vep = castergo.GetComponent<VisualEventProxy>();
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Def:{def} CasterGO:{castergo.name} TargetRef:{targetEntityRef} TargetGO:{targetgo.name} VEP:{vep}").Write();
                            if (vep)
                                vep.PostEvent(CreateEvent(def.TriggerName, castergo, targetgo, targetEntityRef, castergo.transform, repo, @params));
                            else
                                Logger.IfWarn()?.Message("No '{0}' component on GameObject {1}", nameof(VisualEventProxy), castergo.name).Write();
                    });
                }
            }
            else
                if (Logger.IsDebugEnabled) Logger.IfError()?.Message("Game object not found for entity {entity}", cast.Caster).Entity(cast.Caster.Guid).Write();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPostVisualEventDef indef)
        {
            return new ValueTask();
        }

        public static VisualEvent CreateEvent(
            FXEventType trigger, 
            GameObject casterGo, 
            GameObject targetGo, 
            OuterRef<IEntity> targetRef, 
            Transform transform, 
            IEntitiesRepository repo,
            ArgTuple[] @params)
        {
            var evt = new VisualEvent()
            {
                casterRepository = repo,
                eventType = trigger,
                casterGameObject = casterGo,
                targetGameObject = targetGo,
                targetEntityRef = targetRef,
                position = transform?.position ?? Vector3.zero, 
                rotation = transform?.rotation ?? Quaternion.identity,
                parameters = @params 
            };
            return evt;
        }
        
        public static async ValueTask<ArgTuple[]> MapParameters(SpellWordCastData castData, Dictionary<ResourceRef<ArgDef>, ResourceRef<SpellContextValueDef>> @params, IEntitiesRepository repo)
        {
            if (@params == null)
                return Array.Empty<ArgTuple>();
            var rv = new ArgTuple[@params.Count];
            int i = 0;
            foreach(var pair in @params)
                rv[i++] = ArgTuple.CreateTypeless(pair.Key.Target, ArgValue.Create(await pair.Value.Target.GetValue(castData, repo)));
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Map parameters | Spell:{castData.CastData.Def.____GetDebugAddress()}\nTO MAP:\n{string.Join("\n", @params)}\nMAPPED:\n{string.Join("\n", (IEnumerable)rv)}").Write();
            return rv;
        }
    }
}
