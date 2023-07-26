using System;
using System.Collections.Generic;
using ColonyShared.SharedCode;
using ColonyShared.SharedCode.Entities.Reactions;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;
using Scripting;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    public readonly struct CalcerContext
    {
        private readonly SpellCastData _spellCastData;

        public readonly OuterRef EntityRef;
        public OuterRef<IEntity> EntityRefGeneric => new OuterRef<IEntity>(EntityRef);
        public readonly IEntitiesContainer EntityContainer; // Содержит заранее залоченную entity
        public readonly IEntitiesRepository Repository;
        [CanBeNull] public readonly Arg[] Args;
        [CanBeNull] public readonly ScriptingContext Ctx;
        [CanBeNull] public SpellCastData SpellCastData => _spellCastData;
        [CanBeNull] public SpellPredCastData SpellPredCastData => _spellCastData as SpellPredCastData;
        [CanBeNull] public SpellWordCastData SpellWordCastData => _spellCastData as SpellWordCastData;

        public CalcerContext(IEntitiesContainer entityContainer, OuterRef entityRef, IEntitiesRepository repository, SpellCastData spellCast = null, Arg[] args = null, ScriptingContext ctx = null)
        {
            EntityContainer = entityContainer ?? throw new ArgumentNullException(nameof(entityContainer));
            EntityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Args = args;
            Ctx = ctx ?? (spellCast as SpellWordCastData)?.Context;
            _spellCastData = spellCast;
        }
        
        public IEntity GetEntity() 
            => EntityContainer.Get<IEntity>(EntityRef.Guid);

        public TEntity GetEntity<TEntity>(ReplicationLevel level) where TEntity : class
            => EntityContainer.Get<TEntity>(EntityRef, level) ?? throw new Exception($"Entity {EntityRef} is not a {typeof(TEntity).Name}");

        public TEntity TryGetEntity<TEntity>(ReplicationLevel level) where TEntity : class
        {
            EntityContainer.TryGet<TEntity>(EntityRef.TypeId, EntityRef.Guid, level, out var e);
            return e;
        }

        public CalcerContext SetArgs(Arg[] args)
        {
            return new CalcerContext(EntityContainer, EntityRef, Repository, _spellCastData, args);
        }
        
        public readonly struct Arg
        {
            [Obsolete] public readonly string Name;
            public readonly ArgDef Def;
            public readonly Value Value;
            
            public Arg(string name, Value value)
            {
                Name = name;
                Value = value;
                Def = null;
            }
            
            public Arg(ArgDef def, Value value)
            {
                Def = def;
                Value = value;
                Name = null;
            }
        }
    }
}
