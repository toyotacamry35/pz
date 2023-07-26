using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using ResourceSystem.Aspects;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Wizardry;


namespace ColonyShared.SharedCode.Modifiers
{
    public interface IHasSpellModifiers
    {
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] [BsonIgnore]
        IDeltaDictionary<SpellModifiersCauser, SpellModifiersCollectionEntry> SpellModifiers { get; set; }
        
        [ReplicationLevel(ReplicationLevel.Master)]
        ISpellModifiersCollector SpellModifiersCollector { get; set; }
    }

    [GenerateDeltaObjectCode]
    public interface ISpellModifiersCollector : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<bool> AddModifiers(SpellModifiersCauser causer, [NotNull] PredicateDef condition, [NotNull] SpellModifierDef[] modifiers);

        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask<bool> RemoveModifiers(SpellModifiersCauser causer);
    }

    
    [ProtoContract]
    public struct SpellModifiersCollectionEntry
    {
        [ProtoMember(1)] public PredicateDef Condition;
        [ProtoMember(2)] public SpellModifierDef[] Modifiers;

        public SpellModifiersCollectionEntry([NotNull] PredicateDef condition, [NotNull] SpellModifierDef[] modifiers)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Modifiers = modifiers ?? throw new ArgumentNullException(nameof(modifiers));
        }
    }
    
    
    [ProtoContract]
    public struct SpellModifiersCauser : IEquatable<SpellModifiersCauser>
    {
        [ProtoMember(1)] public Guid Uid;
        [ProtoMember(2)] public ulong SpellId;

        public SpellModifiersCauser(Guid uid)
        {
            Uid = uid;
            SpellId = 0;
        }

        public SpellModifiersCauser(Guid uid, SpellId spellId)
        {
            Uid = uid;
            SpellId = spellId.Counter;
        }

        public override string ToString() => Uid.ToString();

        public bool Equals(SpellModifiersCauser other) => Uid.Equals(other.Uid) && SpellId == other.SpellId;

        public override bool Equals(object obj) => obj is SpellModifiersCauser other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Uid.GetHashCode() * 397) ^ SpellId.GetHashCode();
            }
        }
    }


    public static class SpellModifiers
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public static async ValueTask<IReadOnlyList<SpellModifierDef>> GetModifiersForSpell(
            ISpellCast spellCast,
            OuterRef<IEntity> entityRef,
            IEntitiesRepository repository)
        {
            using (var container = await repository.Get(entityRef.TypeId, entityRef.Guid))
                return await GetModifiersForSpell(spellCast, entityRef, container, repository);
        }
        
        public static async ValueTask<IReadOnlyList<SpellModifierDef>> GetModifiersForSpell(
            ISpellCast spellCast,
            OuterRef<IEntity> entityRef,
            IEntitiesContainer entityContainer,
            IEntitiesRepository repository)
        {
            var list = new List<SpellModifierDef>();
            await GetModifiersForSpell(spellCast, entityRef, entityContainer, repository, list);
            return list;
        }
        
        public static async ValueTask GetModifiersForSpell(
            ISpellCast spellCast,
            OuterRef<IEntity> entityRef,
            IEntitiesRepository repository,
            IList<SpellModifierDef> result)
        {
            using (var container = await repository.Get(entityRef.TypeId, entityRef.Guid))
                await GetModifiersForSpell(spellCast, entityRef, container, repository, result);
        }

        public static async ValueTask GetModifiersForSpell(
            ISpellCast spellCast,
            OuterRef<IEntity> entityRef,
            IEntitiesContainer entityContainer,
            IEntitiesRepository repository,
            IList<SpellModifierDef> result)
        {
            var hasSpellModifiers = entityContainer.Get<IHasSpellModifiersClientBroadcast>(entityRef.TypeId, entityRef.Guid, ReplicationLevel.ClientBroadcast);
            if (hasSpellModifiers == null)
                return;
            var modifiers = hasSpellModifiers.SpellModifiers;
            if (modifiers == null)
                return;
            var entries = hasSpellModifiers.SpellModifiers.Values;
            if (entries.Count == 0)
                return;
            var calcersContext = new CalcerContext(entityContainer, entityRef, repository, new SpellCastData(spellCast, entityRef));
            foreach (var entry in entries)
                if (await entry.Condition.CalcAsync(calcersContext))
                    foreach (var modifier in entry.Modifiers)
                        result.Insert(result.Count, modifier);
        }
    }
}