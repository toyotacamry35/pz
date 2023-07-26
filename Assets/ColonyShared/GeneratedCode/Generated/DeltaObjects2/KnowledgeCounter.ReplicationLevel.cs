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
    public class KnowledgeCounterAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IKnowledgeCounterAlways
    {
        public KnowledgeCounterAlways(GeneratedCode.DeltaObjects.IKnowledgeCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IKnowledgeCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObjectBase__;
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
                case 10:
                    currProperty = QuestDef;
                    break;
                case 11:
                    currProperty = CounterDef;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -238910710;
    }

    public class KnowledgeCounterClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IKnowledgeCounterClientBroadcast
    {
        public KnowledgeCounterClientBroadcast(GeneratedCode.DeltaObjects.IKnowledgeCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IKnowledgeCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObjectBase__;
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
                case 10:
                    currProperty = QuestDef;
                    break;
                case 11:
                    currProperty = CounterDef;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1682755940;
    }

    public class KnowledgeCounterClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IKnowledgeCounterClientFullApi
    {
        public KnowledgeCounterClientFullApi(GeneratedCode.DeltaObjects.IKnowledgeCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IKnowledgeCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1337769083;
    }

    public class KnowledgeCounterClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IKnowledgeCounterClientFull
    {
        public KnowledgeCounterClientFull(GeneratedCode.DeltaObjects.IKnowledgeCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IKnowledgeCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObjectBase__;
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
                case 10:
                    currProperty = QuestDef;
                    break;
                case 11:
                    currProperty = CounterDef;
                    break;
                case 13:
                    currProperty = CountForClient;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1371376816;
    }

    public class KnowledgeCounterServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IKnowledgeCounterServerApi
    {
        public KnowledgeCounterServerApi(GeneratedCode.DeltaObjects.IKnowledgeCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IKnowledgeCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1594746607;
    }

    public class KnowledgeCounterServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IKnowledgeCounterServer
    {
        public KnowledgeCounterServer(GeneratedCode.DeltaObjects.IKnowledgeCounter deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.DeltaObjects.IKnowledgeCounter __deltaObject__
        {
            get
            {
                return (GeneratedCode.DeltaObjects.IKnowledgeCounter)__deltaObjectBase__;
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
                case 10:
                    currProperty = QuestDef;
                    break;
                case 11:
                    currProperty = CounterDef;
                    break;
                case 13:
                    currProperty = CountForClient;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1385627303;
    }
}