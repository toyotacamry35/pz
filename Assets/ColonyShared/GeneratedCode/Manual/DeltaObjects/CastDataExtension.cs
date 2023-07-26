using System;
using ColonyShared.SharedCode;
using System.Linq;
using SharedCode.Wizardry;
using SharedCode.EntitySystem;
using NLog;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using GeneratedCode.MapSystem;

namespace GeneratedCode.DeltaObjects
{
    public static class CastDataExtension
    {
        public static async ValueTask<Value> GetValue(this SpellContextValueDef def, SpellCastData cast, IEntitiesRepository repo)
        {
            switch (def)
            {
                case null:
                    throw new NullReferenceException($"Spell {cast.CastData.Def.____GetDebugAddress()}: {nameof(def)} is null");
                case SpellCasterDef _:
                    return new Value(cast.Caster);
                case SpellExplicitVector3Def d:
                    return new Value(new SharedCode.Utils.Vector3(d.x, d.y, d.z));
                case SpellExplicitVector2Def d:
                    return new Value(new SharedCode.Utils.Vector2(d.x, d.y));
                case SpellExplicitStringDef d:
                    return new Value(d.Value);
                case CurrentSpellIdDef _:
                    return ((cast as SpellWordCastData)?.SpellId ?? SpellId.Invalid).ToValue();
                case ISpellCalcerDef d:
                    return await d.Calcer.CalcAsyncFromSpell(cast.Caster.To<IEntity>(), repo, cast);
                case ISpellEntityHasAuthorityDef d:
                    return new Value((await d.Entity.GetOuterRef(cast, repo)).HasClientAuthority(repo));
                case ISpellParameterDef d:
                    if (SpellCastParameters.TryGetParameterTypeByDef(d, out var paramType))
                        return cast.CastData.GetParameter(paramType).Value;
                    break;
            }
            throw new NotSupportedException($"Spell {cast.CastData.Def.____GetDebugAddress()}: {def.GetType()} is not supported");
        }
        
        public static async ValueTask<OuterRef<IEntity>> GetOuterRef(this SpellEntityDef entityDef, SpellCastData castData, IEntitiesRepository repo)
        {
            if (entityDef == null)
                return default(OuterRef<IEntity>);
            if (entityDef is SpellCasterDef)
                return castData.Caster;
            if (entityDef is SpellTargetLinksDef spellLink)
            {
                var from = await GetOuterRef(spellLink.From, castData, repo);
                if (from == default)
                    return default;
                using (var ent = await repo.Get(from.TypeId, from.Guid))
                {
                    var linksEngine = ent.Get<IHasLinksEngineClientBroadcast>(from.TypeId, from.Guid, ReplicationLevel.ClientBroadcast);
                    if (linksEngine.LinksEngine.Links.TryGetValue(spellLink.LinkType, out var links))
                        return new OuterRef<IEntity>(links.Links.Keys.FirstOrDefault());
                    
                }
                return default;
            }
            if (entityDef is SpellTargetDef)
                return castData.CastData.GetTargetOptional();
            if (entityDef is SpellSpawnerOfDef spawnerOf)
            {
                var from = await GetOuterRef(spawnerOf.Target, castData, repo);
                if (from == default)
                    return default;
                var spawner = repo.TryGetLockfree<IHasSpawnedObjectClientBroadcast>(from, ReplicationLevel.ClientBroadcast).SpawnedObject.Spawner.To<IEntity>();
                return spawner;
            }
            if (entityDef is SpellEventOfDef eventOf)
            {
                var from = await GetOuterRef(eventOf.Target, castData, repo);
                if (from == default)
                    return default;
                MapOwner owner;
                using (var scenicW = await repo.Get(from))
                {
                    var scenic = scenicW.Get<IScenicEntityServer>(from, ReplicationLevel.Server);
                    owner = scenic.MapOwner;
                }
                var sceneE = repo.TryGetLockfree<ISceneEntityServer>(owner.OwnerScene, ReplicationLevel.Server);
                var eo = sceneE.EventOwner;
                return eo;
            }
            throw new ArgumentException($"Wrong entityDef type {entityDef.GetType()}");
        }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static OuterRef<IEntity> GetTarget(this ISpellCast cast)
        {
            if (!cast.AssertIfNull(nameof(cast)))
            {
                if (cast.TryGetParameter<SpellCastParameterTarget>(out var p))
                    return p.Target;
                Logger.IfError()?.Message($"Can't get Target {cast?.GetType()} {cast?.Def.____GetDebugAddress()}").Write();
            }
            return default(OuterRef<IEntity>);
        }

        public static OuterRef<IEntity> GetTargetOptional(this ISpellCast cast)
        {
            if (!cast.AssertIfNull(nameof(cast)))
            {
                if (cast.TryGetParameter<SpellCastParameterTarget>(out var p))
                    return p.Target;
            }
            return default(OuterRef<IEntity>);
        }

        public static SharedCode.Utils.Vector2 GetVector(this SpellVector2Def vectorDef, SpellPredCastData castData)
        {
            switch (vectorDef)
            {
                case null:
                    throw new NullReferenceException($"Spell {castData.CastData.Def.____GetDebugAddress()}: {nameof(vectorDef)} is null");
                case SpellDirection2Def _:
                    return castData.CastData.GetParameter<SpellCastParameterDirection2>().Direction;
                case SpellPoint2Def _:
                    return  castData.CastData.GetParameter<SpellCastParameterPosition2>().Position;  
                case SpellExplicitVector2Def def:
                    return new SharedCode.Utils.Vector2(def.x, def.y);
                case ISpellParameterDef d:
                    if (SpellCastParameters.TryGetParameterTypeByDef(d, out var paramType))
                        return castData.CastData.GetParameter(paramType).Value.Vector2;
                    break;
            }
            throw new NotSupportedException($"Spell {castData.CastData.Def.____GetDebugAddress()}: {vectorDef.GetType()} is not supported");
        }
        
        public static SharedCode.Utils.Vector3 GetVector(this SpellVector3Def vectorDef, SpellPredCastData castData)
        {
            if (Logger.IsDebugEnabled && ((IResource) vectorDef).Address.Root != ((IResource) castData.CastData.Def).Address.Root)
                Logger.IfError()?.Write($"Inconsistent spell data | Def:{vectorDef.____GetDebugAddress()} Spell:{castData.CastData.Def.____GetDebugAddress()} Data:{castData}");
            switch (vectorDef)
            {
                case null:
                    throw new NullReferenceException($"Spell {castData.CastData.Def.____GetDebugAddress()}: {nameof(vectorDef)} is null");
                case SpellDirectionDef _:
                    return castData.CastData.GetParameter<SpellCastParameterDirection>().Direction;
                case SpellTargetPointDef _:
                    return  castData.CastData.GetParameter<SpellCastParameterPosition>().Position;  
                case SpellExplicitVector3Def def:
                    return new SharedCode.Utils.Vector3(def.x, def.y, def.z);
                case ISpellParameterDef d:
                    if (SpellCastParameters.TryGetParameterTypeByDef(d, out var paramType))
                        return castData.CastData.GetParameter(paramType).Value.Vector3;
                    break;
            }
            throw new NotSupportedException($"Spell {castData.CastData.Def.____GetDebugAddress()}: {vectorDef.GetType()} is not supported");
        }
        
        
        public static SpellId GetSpellId(this SpellIdDef def, SpellWordCastData castData)
        {
            switch (def)
            {
                case null:
                    throw new NullReferenceException($"Spell {castData.CastData.Def.____GetDebugAddress()}: {nameof(def)} is null");
                case CurrentSpellIdDef _:
                    return castData.SpellId;
                case ISpellParameterDef d:
                    if (SpellCastParameters.TryGetParameterTypeByDef(d, out var paramType))
                        return castData.CastData.GetParameter(paramType).Value.SpellId();
                    break;
            }
            throw new NotSupportedException($"Spell {castData.CastData.Def.____GetDebugAddress()}: {def.GetType()} is not supported");
        }

    }
}