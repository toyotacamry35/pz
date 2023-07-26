using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using Assets.Src.Audio;
using Assets.Src.Wizardry;
using ColonyShared.SharedCode;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using NLog;
using SharedCode;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Src.ManualDefsForSpells;

namespace Assets.Src.Effects.AnimatorEffects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectSound : IClientOnlyEffectBinding<EffectSoundDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSoundDef def)
        {
            var targetEntity = cast.Caster;
            if (def.Target.Target != null)
                targetEntity = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetEntity.IsValid)
                targetEntity = cast.Caster; 

            var targetObj = def.Target.Target?.GetGo(cast) ?? cast.GetCaster();
            if (!targetObj.AssertIfNull("target"))
            {
                AsyncUtils.RunAsyncTask(async () =>
                {
                    List<(string, Value)> @params = null;
                    if (def.Params != null && def.Params.Count > 0) 
                    {
                        @params = new List<(string, Value)>(def.Params.Count);
                        using (var cont = await repo.Get(targetEntity))
                        {
                            var ctx = new CalcerContext(cont, targetEntity, repo, cast);
                            foreach (var param in def.Params)
                            {
                                if (param.Value.Target != null)
                                {
                                    var val = await param.Value.Target.CalcAsync(ctx);
                                    @params.Add((param.Key, val));
                                }
                            }
                        }
                    }
                    UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        if (@params != null)
                            foreach (var tuple in @params)
                                SoundHelper.SetParameter(tuple.Item1, tuple.Item2, targetObj);
                        if (def.Event != null)
                            AkSoundEngine.PostEvent(def.Event, targetObj);
                    });
                }, repo);
            }
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSoundDef def)
        {
            if (def.Event != null && !def.Detach)
            {
                var targetObj = def.Target.Target?.GetGo(cast) ?? cast.GetCaster();
                if (!targetObj.AssertIfNull("target"))
                {
                    UnityQueueHelper.RunInUnityThreadNoWait(() => AkSoundEngine.ExecuteActionOnEvent(def.Event, AkActionOnEventType.AkActionOnEventType_Stop, targetObj));
                }
            }
            return new ValueTask();
        }
   }
}