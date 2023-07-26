using System;
using SharedCode.EntitySystem;

namespace GeneratedCode.EntitySystem
{
    public abstract class BaseEntityWrapper: BaseDeltaObjectWrapper, IEntity
    {
        protected IEntity __entityBase__ => (IEntity) __deltaObjectBase__;

        public EntityMigrationState Migrating => __entityBase__.Migrating;

        protected BaseEntityWrapper(IEntity entity):base(entity)
        {
        }

        public Guid Id => __entityBase__.Id;

        //public int TypeId => __entityBase__.TypeId;

        public string TypeName => __entityBase__.TypeName;

        public IEntitiesRepository EntitiesRepository => __entityBase__.EntitiesRepository;
    }
}
