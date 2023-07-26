using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.GeneratedCode;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using NLog;
using ColonyShared.SharedCode;
using Scripting;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    public static class CalcerCalcerAsync
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        [CollectTypes, UsedImplicitly] private static CalcerBindingCollector Calcers;
        
        private static class CalcersCollection
        {
            public static bool TryGetCalcer(Type type, out CalcerBinding impl) => Calcers.Collection.TryGetValue(type, out impl);

            public static CalcerBinding GetCalcer(Type type)
            {
                if (!Calcers.Collection.TryGetValue(type, out var impl))
                    throw new Exception($"{nameof(CalcerBinding)} is not implemented for {type}");
                return impl;
            }
        }

        private static class CalcersCollection<ResultType>
        {
            public static bool TryGetCalcer(Type type, out CalcerBinding<ResultType> impl)
            {
                var rv = Calcers.Collection.TryGetValue(type, out var i);
                impl = (CalcerBinding<ResultType>)i;
                return rv;
            }

            public static CalcerBinding<ResultType> GetCalcer(Type type)
            {
                if (!Calcers.Collection.TryGetValue(type, out var impl))
                    throw new Exception($"{nameof(CalcerBinding)} is not implemented for {type}");
                return (CalcerBinding<ResultType>)impl;
            }
        }

        [System.Diagnostics.Contracts.Pure]
        public static async ValueTask<ReturnType> CalcAsync<ReturnType>([NotNull] this CalcerDef<ReturnType> def, OuterRef<IEntity> ent, IEntitiesRepository repo)
        {
            if (!ent.IsValid)
                return default;
            var calcer = CalcersCollection<ReturnType>.GetCalcer(def.GetType());
            using (var ew = await repo.Get(ent.TypeId, ent.Guid))
                return await calcer.Calc(def, new CalcerContext(ew, ent, repo));
        }

        [System.Diagnostics.Contracts.Pure]
        public static async ValueTask<ReturnType> CalcAsync<ReturnType>([NotNull] this CalcerDef<ReturnType> def, OuterRef<IEntity> ent, ScriptingContext fromCtx, IEntitiesRepository repo)
        {
            if (def == null)
                return default;
            if (!ent.IsValid)
                return default;
            var calcer = CalcersCollection<ReturnType>.GetCalcer(def.GetType());
            using (var ew = await repo.Get(ent.TypeId, ent.Guid))
                return await calcer.Calc(def, new CalcerContext(ew, ent, repo, null, null, fromCtx));
        }
        [System.Diagnostics.Contracts.Pure]
        public static async ValueTask<Value> CalcAsync([NotNull] this CalcerDef def, OuterRef<IEntity> ent, IEntitiesRepository repo)
        {
            if (!ent.IsValid)
                return default;
            var calcer = CalcersCollection.GetCalcer(def.GetType());
            using (var ew = await repo.Get(ent.TypeId, ent.Guid))
                return await calcer.Calc(def, new CalcerContext(ew, ent, repo));
        }
        [System.Diagnostics.Contracts.Pure]
        public static async ValueTask<Value> CalcAsync([NotNull] this CalcerDef def, OuterRef<IEntity> ent, ScriptingContext ctx, IEntitiesRepository repo)
        {
            if (!ent.IsValid)
                return default;
            if (def == null)
                return default;
            var calcer = CalcersCollection.GetCalcer(def.GetType());
            using (var ew = await repo.Get(ent.TypeId, ent.Guid))
                return await calcer.Calc(def, new CalcerContext(ew, ent, repo, ctx:ctx));
        }

        [System.Diagnostics.Contracts.Pure]
        public static async ValueTask<ReturnType> CalcAsyncFromSpell<ReturnType>([NotNull] this CalcerDef<ReturnType> def, OuterRef<IEntity> ent, IEntitiesRepository repo, SpellCastData spellCast)
        {
            if (!ent.IsValid)
                return default;
            var calcer = CalcersCollection<ReturnType>.GetCalcer(def.GetType());
            using (var ew = await repo.Get(ent.TypeId, ent.Guid))
                return await calcer.Calc(def, new CalcerContext(ew, ent, repo, spellCast));
        }

        [System.Diagnostics.Contracts.Pure]
        public static async ValueTask<Value> CalcAsyncFromSpell([NotNull] this CalcerDef def, OuterRef<IEntity> ent, IEntitiesRepository repo, SpellCastData spellCast)
        {
            if (!ent.IsValid)
                return default;
            var calcer = CalcersCollection.GetCalcer(def.GetType());
            using (var ew = await repo.Get(ent.TypeId, ent.Guid))
                return await calcer.Calc(def, new CalcerContext(ew, ent, repo, spellCast));
        }
        
        [System.Diagnostics.Contracts.Pure]
        public static ValueTask<ReturnType> CalcAsync<ReturnType>([NotNull] this CalcerDef<ReturnType> def, CalcerContext ctx)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            if (ctx.EntityContainer == null) throw new ArgumentNullException("ctx.EntityContainer");
            if (ctx.Repository == null) throw new ArgumentNullException("ctx.Repository");
            if (!ctx.EntityRef.IsValid) throw new ArgumentException("ctx.EntityRef");
            var calcer = CalcersCollection<ReturnType>.GetCalcer(def.GetType());
            return calcer.Calc(def, ctx);
        }

        [System.Diagnostics.Contracts.Pure]
        public static ValueTask<Value> CalcAsync([NotNull] this CalcerDef def, CalcerContext ctx)
        {
            if (ctx.EntityContainer == null) throw new ArgumentNullException("ctx.EntityContainer");
            if (ctx.Repository == null) throw new ArgumentNullException("ctx.Repository");
            if (!ctx.EntityRef.IsValid) throw new ArgumentException("ctx.EntityRef");
            var calcer = CalcersCollection.GetCalcer(def.GetType());
            return calcer.Calc(def, ctx);
        }
        
        [System.Diagnostics.Contracts.Pure]
        public static IEnumerable<StatResource> CollectStatNotifiers(this CalcerDef def, bool optional = false)
        {
            if (def == null)
                return Enumerable.Empty<StatResource>();
            if(!CalcersCollection.TryGetCalcer(def.GetType(), out var calcer))
            {
                if (!optional)
                    Logger.Log(LogLevel.Error, $" !!!!!!!! Calcer '{def.GetType()}' cant return stat notifiers !!!!!!!!");
                return new StatResource[] { null };
            }
            return calcer.CollectStatNotifiers(def);
        }

        [System.Diagnostics.Contracts.Pure]
        public static IEnumerable<StatResource> GetModifiers(this CalcerDef calcerRef)
            => calcerRef != null ? calcerRef.CollectStatNotifiers() : Enumerable.Empty<StatResource>();
        
        [System.Diagnostics.Contracts.Pure]
        public static IEnumerable<StatResource> GetModifiers(this IResourceRef<CalcerDef> calcerRef)
            => calcerRef != null && calcerRef.IsValid ? calcerRef.Target.CollectStatNotifiers() : Enumerable.Empty<StatResource>();
    }
}
