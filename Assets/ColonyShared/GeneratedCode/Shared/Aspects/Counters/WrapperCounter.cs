using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Quests;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{

    [GenerateDeltaObjectCode]
    public interface IWrapperCounter : IQuestCounter
    {
        [OverrideSerializeSettings(DynamicType = true, AsReference = false)]
        [ReplicationLevel(ReplicationLevel.ClientFull)] IQuestCounter SubCounter { get; set; }
    }


    public partial class WrapperCounter
    {
        private WrapperCounterDef Def => (WrapperCounterDef)CounterDef;
        private bool _counterCompleted = false;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            QuestDef = questDef;
            CounterDef = counterDef;
            _counterCompleted = false;

            if (Def.SubCounter.IsValid)
            {
                var counterType = DefToType.GetType(Def.SubCounter.Target.GetType());
                var counterInstance = (IQuestCounter)Activator.CreateInstance(counterType);

                if (counterInstance != null)
                {
                    SubCounter = counterInstance;
                    await counterInstance.OnInit(questDef, Def.SubCounter.Target, EntitiesRepository);
                }
            }
        }

        public async Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            await SubCounter.OnDatabaseLoad(repository);
            await Subscribe(repository, true);
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            if (!_counterCompleted)
            {
                await SubCounter.OnDestroy(repository);
                await Subscribe(repository, false);
            }
        }

        private Task Subscribe(IEntitiesRepository repository, bool subscribe)
        {
            if (subscribe)
                SubCounter.OnCounterChanged += CounterInstance_OnCounterChanged;
            else
                SubCounter.OnCounterChanged -= CounterInstance_OnCounterChanged;
            return Task.CompletedTask;
        }

        private async Task CounterInstance_OnCounterChanged(IQuestCounter arg)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;

            Count = CountForClient = SubCounter.Completed ? 1 : 0;
            if (Count == 1)
            {
                _counterCompleted = true;
                await Subscribe(EntitiesRepository, false);
                await OnOnCounterChangedInvoke(this);
                await OnOnCounterCompletedInvoke(QuestDef, this);
                return;
            }
            await OnOnCounterChangedInvoke(this);
        }

        public Task PreventOnCompleteEventImpl()
        {
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("WrapperCounter: Def = {0}, Def = ", Def.____GetDebugShortName());
            return sb.ToString();
        }
    }
}
