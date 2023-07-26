using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    public abstract partial class BaseDeltaObject
    {
        private static Guid DefaultMigrationgid = Guid.Empty;

        [ProtoIgnore]
        [BsonIgnore]
        public virtual ref Guid MigratingId
        {
            get
            {
                if (parentEntity != null)
                    return ref ((IDeltaObjectExt)parentEntity).MigratingId;
                return ref DefaultMigrationgid;
            }
        }

        public virtual MigrationIncrementCounterType IncrementExecutedMethodsCounter(out IEntity parentEntityToDecrement)
        {
            if (parentEntity == null)
            {
                parentEntityToDecrement = null;
                return MigrationIncrementCounterType.None;
            }

            return ((IEntityExt)parentEntity).IncrementExecutedMethodsCounter(out parentEntityToDecrement);
        }

        //public virtual void DecrementExecutedMethodsCounter(MigrationIncrementCounterType counterType)
        //{
        //    if (counterType == MigrationIncrementCounterType.None)
        //        return;

        //    if (parentEntity == null)
        //    {
        //        Logger.IfError()?.Message("DecrementExecutedMethodsCounter deltaObject {0} is detached (parentEntity == null), but counterType is {1}. possible migration freeze", GetType().Name, counterType).Write();
        //        return;
        //    }

        //    ((IEntityExt)parentEntity).DecrementExecutedMethodsCounter(counterType);
        //}

        protected virtual Guid GetActualMigratingId()
        {
            if (parentEntity == null)
                return Guid.Empty;

            return ((BaseEntity) parentEntity).GetActualMigratingId();
        }
    };
}
