using System;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using ColonyShared.SharedCode.Entities.Reactions;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using NLog;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Utils;

namespace Src.SpellSystem
{
    [UsedImplicitly]
    public sealed class ImpactReaction : IImpactBinding<ImpactReactionDef> //SpellImpactBase<ImpactReactionDef>, ISpellImpact
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public  async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactReactionDef def)
        {
            var tRef = cast.Caster;
            if (def.Target.Target != null)
                tRef = await def.Target.Target.GetOuterRef(cast, repo);

            if (!tRef.IsValid)
                tRef = cast.Caster;

            var targetRef = tRef.To<IHasReactionsOwner>();

            using (var wrp = await repo.Get(targetRef))
            {
                    var ctx = new CalcerContext(wrp, targetRef, repo, cast);
                    var args = new ArgTuple[def.Args?.Count ?? 0];
                    int i = 0;
                    if (def.Args != null)
                        foreach (var kv in def.Args)
                            args[i++] = ArgTuple.CreateTypeless(kv.Key, await CreateValue(kv.Value.Target, ctx));
                    var target = wrp.Get(targetRef, ReplicationLevel.Master);
                    if (target != null)
                        await target.ReactionsOwner.InvokeReaction(def.Reaction, args);
                    else
                        if(Logger.IsDebugEnabled) Logger.IfError()?.Message($"Target {targetRef} is not IHasReactions").Write();
            }
        }

        private async Task<ArgValue> CreateValue(ImpactReactionDef.ArgValueDef varDef, CalcerContext ctx)
        {
            switch (varDef)
            {
                case ImpactReactionDef.ArgValueFloatDef def:
                    return new ArgValue<float>(await def.Value.Target.CalcAsync(ctx));
                case ImpactReactionDef.ArgValueVector2Def def:
                    return new ArgValue<Vector2>(new Vector2(await def.X.Target.CalcAsync(ctx), await def.Y.Target.CalcAsync(ctx)));
                case ImpactReactionDef.ArgValueVector3Def def:
                    return new ArgValue<Vector3>(new Vector3(await def.X.Target.CalcAsync(ctx), await def.Y.Target.CalcAsync(ctx), await def.Z.Target.CalcAsync(ctx)));
            }
            throw new NotImplementedException($"{varDef.GetType()}");
        }
    }
}