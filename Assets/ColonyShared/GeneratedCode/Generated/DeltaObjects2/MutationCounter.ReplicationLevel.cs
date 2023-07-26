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
    public class MutationCounterAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationCounterAlways
    {
        public MutationCounterAlways(GeneratedCode.DeltaObjects.IMutationCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IMutationCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IMutationCounter)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.QuestDef QuestDef => __deltaObject__.QuestDef;
        public Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef CounterDef => __deltaObject__.CounterDef;
        public event System.Func<Assets.Src.Aspects.Impl.Factions.Template.QuestDef, SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterCompleted
        {
            add
            {
                __deltaObject__.OnCounterCompleted += value;
            }

            remove
            {
                __deltaObject__.OnCounterCompleted -= value;
            }
        }

        public event System.Func<SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterChanged
        {
            add
            {
                __deltaObject__.OnCounterChanged += value;
            }

            remove
            {
                __deltaObject__.OnCounterChanged -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 12:
                    currProperty = QuestDef;
                    break;
                case 13:
                    currProperty = CounterDef;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1558448527;
    }

    public class MutationCounterClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationCounterClientBroadcast
    {
        public MutationCounterClientBroadcast(GeneratedCode.DeltaObjects.IMutationCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IMutationCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IMutationCounter)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.QuestDef QuestDef => __deltaObject__.QuestDef;
        public Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef CounterDef => __deltaObject__.CounterDef;
        public event System.Func<Assets.Src.Aspects.Impl.Factions.Template.QuestDef, SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterCompleted
        {
            add
            {
                __deltaObject__.OnCounterCompleted += value;
            }

            remove
            {
                __deltaObject__.OnCounterCompleted -= value;
            }
        }

        public event System.Func<SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterChanged
        {
            add
            {
                __deltaObject__.OnCounterChanged += value;
            }

            remove
            {
                __deltaObject__.OnCounterChanged -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 12:
                    currProperty = QuestDef;
                    break;
                case 13:
                    currProperty = CounterDef;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1592520993;
    }

    public class MutationCounterClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationCounterClientFullApi
    {
        public MutationCounterClientFullApi(GeneratedCode.DeltaObjects.IMutationCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IMutationCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IMutationCounter)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1576514867;
    }

    public class MutationCounterClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationCounterClientFull
    {
        public MutationCounterClientFull(GeneratedCode.DeltaObjects.IMutationCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IMutationCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IMutationCounter)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.QuestDef QuestDef => __deltaObject__.QuestDef;
        public Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef CounterDef => __deltaObject__.CounterDef;
        public int CountForClient => __deltaObject__.CountForClient;
        public System.Threading.Tasks.Task OnInit(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            return __deltaObject__.OnInit(questDef, counterDef, repository);
        }

        public System.Threading.Tasks.Task OnDatabaseLoad(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            return __deltaObject__.OnDatabaseLoad(repository);
        }

        public System.Threading.Tasks.Task OnDestroy(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            return __deltaObject__.OnDestroy(repository);
        }

        public event System.Func<Assets.Src.Aspects.Impl.Factions.Template.QuestDef, SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterCompleted
        {
            add
            {
                __deltaObject__.OnCounterCompleted += value;
            }

            remove
            {
                __deltaObject__.OnCounterCompleted -= value;
            }
        }

        public event System.Func<SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterChanged
        {
            add
            {
                __deltaObject__.OnCounterChanged += value;
            }

            remove
            {
                __deltaObject__.OnCounterChanged -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 12:
                    currProperty = QuestDef;
                    break;
                case 13:
                    currProperty = CounterDef;
                    break;
                case 15:
                    currProperty = CountForClient;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1441979257;
    }

    public class MutationCounterServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationCounterServerApi
    {
        public MutationCounterServerApi(GeneratedCode.DeltaObjects.IMutationCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IMutationCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IMutationCounter)__deltaObjectBase__;
            }
        }

        public override int TypeId => 663723582;
    }

    public class MutationCounterServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IMutationCounterServer
    {
        public MutationCounterServer(GeneratedCode.DeltaObjects.IMutationCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IMutationCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IMutationCounter)__deltaObjectBase__;
            }
        }

        public Assets.Src.Aspects.Impl.Factions.Template.QuestDef QuestDef => __deltaObject__.QuestDef;
        public Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef CounterDef => __deltaObject__.CounterDef;
        public int CountForClient => __deltaObject__.CountForClient;
        public System.Threading.Tasks.Task OnInit(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            return __deltaObject__.OnInit(questDef, counterDef, repository);
        }

        public System.Threading.Tasks.Task OnDatabaseLoad(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            return __deltaObject__.OnDatabaseLoad(repository);
        }

        public System.Threading.Tasks.Task OnDestroy(SharedCode.EntitySystem.IEntitiesRepository repository)
        {
            return __deltaObject__.OnDestroy(repository);
        }

        public event System.Func<Assets.Src.Aspects.Impl.Factions.Template.QuestDef, SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterCompleted
        {
            add
            {
                __deltaObject__.OnCounterCompleted += value;
            }

            remove
            {
                __deltaObject__.OnCounterCompleted -= value;
            }
        }

        public event System.Func<SharedCode.Entities.Engine.IQuestCounter, System.Threading.Tasks.Task> OnCounterChanged
        {
            add
            {
                __deltaObject__.OnCounterChanged += value;
            }

            remove
            {
                __deltaObject__.OnCounterChanged -= value;
            }
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 12:
                    currProperty = QuestDef;
                    break;
                case 13:
                    currProperty = CounterDef;
                    break;
                case 15:
                    currProperty = CountForClient;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -688941554;
    }
}