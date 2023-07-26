using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using Src.ManualDefsForSpells;

namespace Assets.Src.Effects.AnimatorEffects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectAnimator : IClientOnlyEffectBinding<EffectAnimatorDef>
    {
        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectAnimatorDef def)
        {
            var targetRef = cast.Caster; 
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!targetRef.IsValid)
                targetRef = cast.Caster;

            var target = targetRef.To<IHasAnimationDoerOwnerClientBroadcast>();

            var causer = cast.WordCastId(def);
            using (var cont = await repo.Get(target))
            {
                var doer = cont?.Get(target, ReplicationLevel.ClientBroadcast)?.AnimationDoerOwner.AnimationDoer;
                if (doer != null)
                    foreach (var @ref in def.Actions)
                        if (@ref.Target.__When == EffectAnimatorDef.When.OnStart)
                        {
                            var modifiers = new List<IAnimationModifier>();
                            await doer.ModifiersFactory.Create(@ref, cast, target, repo, modifiers);
                            foreach (var mod in modifiers)
                                if (@ref.Target.__Detached)
                                    doer.Set(mod);
                                else
                                    doer.Push(causer, mod);
                        }
            }
        }

        public async ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectAnimatorDef def)
        {
            var targetRef = cast.Caster;
            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if(!targetRef.IsValid)
                targetRef = cast.Caster;

            var target = targetRef.To<IHasAnimationDoerOwnerClientBroadcast>();

            var causer = cast.WordCastId(def);
            using (var cont = await repo.Get(target))
            {
                var doer = cont?.Get(target, ReplicationLevel.ClientBroadcast)?.AnimationDoerOwner.AnimationDoer;
                if (doer != null)
                    foreach (var @ref in def.Actions)
                        if (@ref.Target.__When == EffectAnimatorDef.When.OnFinish)
                        {
                            var modifiers = new List<IAnimationModifier>();
                            await doer.ModifiersFactory.Create(@ref, cast, target, repo, modifiers);
                            foreach (var mod in modifiers)
                                if (@ref.Target.__Detached)
                                    doer.Set(mod);
                                else
                                    doer.Push(null, mod); //FIXME: имеет ли это вообще смысл?
                        }
                        else if (@ref.Target.__When == EffectAnimatorDef.When.OnStart && !@ref.Target.__Detached)
                        {
                            var modifiers = new List<IAnimationModifier>();
                            await doer.ModifiersFactory.Create(@ref, cast, target, repo, modifiers);
                            foreach (var mod in modifiers)
                                doer.Pop(causer, mod);
                        }
            }
        }
    }
}