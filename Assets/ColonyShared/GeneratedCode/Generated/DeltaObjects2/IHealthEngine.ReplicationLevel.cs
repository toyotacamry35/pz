// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class HealthEngineAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IHealthEngineAlways
    {
        public HealthEngineAlways(SharedCode.Entities.Engine.IHealthEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IHealthEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IHealthEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 445919303;
    }

    public class HealthEngineClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IHealthEngineClientBroadcast
    {
        public HealthEngineClientBroadcast(SharedCode.Entities.Engine.IHealthEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IHealthEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IHealthEngine)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.ValueTask<float> GetHealthCurrent()
        {
            return __deltaObject__.GetHealthCurrent();
        }

        public System.Threading.Tasks.ValueTask<float> GetMaxHealth()
        {
            return __deltaObject__.GetMaxHealth();
        }

        public System.Threading.Tasks.ValueTask<float> GetMinHealth()
        {
            return __deltaObject__.GetMinHealth();
        }

        public System.Threading.Tasks.ValueTask<float> GetMaxHealthAbsolute()
        {
            return __deltaObject__.GetMaxHealthAbsolute();
        }

        public event SharedCode.Entities.Engine.DamageReceivedDelegate DamageReceivedEvent
        {
            add
            {
                __deltaObject__.DamageReceivedEvent += value;
            }

            remove
            {
                __deltaObject__.DamageReceivedEvent -= value;
            }
        }

        public override int TypeId => 221596371;
    }

    public class HealthEngineClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IHealthEngineClientFullApi
    {
        public HealthEngineClientFullApi(SharedCode.Entities.Engine.IHealthEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IHealthEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IHealthEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 676437041;
    }

    public class HealthEngineClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IHealthEngineClientFull
    {
        public HealthEngineClientFull(SharedCode.Entities.Engine.IHealthEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IHealthEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IHealthEngine)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.ValueTask<float> GetHealthCurrent()
        {
            return __deltaObject__.GetHealthCurrent();
        }

        public System.Threading.Tasks.ValueTask<float> GetMaxHealth()
        {
            return __deltaObject__.GetMaxHealth();
        }

        public System.Threading.Tasks.ValueTask<float> GetMinHealth()
        {
            return __deltaObject__.GetMinHealth();
        }

        public System.Threading.Tasks.ValueTask<float> GetMaxHealthAbsolute()
        {
            return __deltaObject__.GetMaxHealthAbsolute();
        }

        public event SharedCode.Entities.Engine.DamageReceivedDelegate DamageReceivedEvent
        {
            add
            {
                __deltaObject__.DamageReceivedEvent += value;
            }

            remove
            {
                __deltaObject__.DamageReceivedEvent -= value;
            }
        }

        public override int TypeId => -1835419375;
    }

    public class HealthEngineServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IHealthEngineServerApi
    {
        public HealthEngineServerApi(SharedCode.Entities.Engine.IHealthEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IHealthEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IHealthEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -313175165;
    }

    public class HealthEngineServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IHealthEngineServer
    {
        public HealthEngineServer(SharedCode.Entities.Engine.IHealthEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IHealthEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IHealthEngine)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.ValueTask<float> GetHealthCurrent()
        {
            return __deltaObject__.GetHealthCurrent();
        }

        public System.Threading.Tasks.ValueTask<float> GetMaxHealth()
        {
            return __deltaObject__.GetMaxHealth();
        }

        public System.Threading.Tasks.ValueTask<float> GetMinHealth()
        {
            return __deltaObject__.GetMinHealth();
        }

        public System.Threading.Tasks.ValueTask<float> GetMaxHealthAbsolute()
        {
            return __deltaObject__.GetMaxHealthAbsolute();
        }

        public System.Threading.Tasks.Task ChangeHealth(float deltaValue)
        {
            return __deltaObject__.ChangeHealth(deltaValue);
        }

        public System.Threading.Tasks.Task<Assets.ColonyShared.SharedCode.Aspects.Damage.DamageResult> ReceiveDamage(Assets.ColonyShared.SharedCode.Aspects.Damage.Damage damage, SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> aggressor)
        {
            return __deltaObject__.ReceiveDamage(damage, aggressor);
        }

        public event SharedCode.Entities.Engine.DamageReceivedDelegate DamageReceivedEvent
        {
            add
            {
                __deltaObject__.DamageReceivedEvent += value;
            }

            remove
            {
                __deltaObject__.DamageReceivedEvent -= value;
            }
        }

        public override int TypeId => 1359562645;
    }
}