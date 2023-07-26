using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Quests;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects.Counters.Template;

namespace GeneratedCode.DeltaObjects
{

    [GenerateDeltaObjectCode]
    public interface ICombinatorCounter : IQuestCounter
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaList<IQuestCounterWrapper> SubCounters { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task SetCombinatorCount(int Count);

    }

    [GenerateDeltaObjectCode]
    public interface IQuestCounterWrapper : IDeltaObject
    {
        [OverrideSerializeSettings(DynamicType = true, AsReference = false)]
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IQuestCounter Counter { get; set; }
    }
    public partial class CombinatorCounter
    {
        private CombinatorCounterDef Def => (CombinatorCounterDef)CounterDef;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool _counterCompleted = false;

        public async Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            QuestDef = questDef;
            CounterDef = counterDef;
            _counterCompleted = false;

            foreach (var subcounter in Def.SubCounters)
                if (subcounter.IsValid)
                {
                    var counterType = DefToType.GetType(subcounter.Target.GetType());
                    var counterInstance = (IQuestCounter)Activator.CreateInstance(counterType);
                    
                    if (counterInstance != null)
                    {
                        var wrapper = (IQuestCounterWrapper)Activator.CreateInstance(typeof(QuestCounterWrapper));
                        SubCounters.Add(wrapper);
                        wrapper.Counter = counterInstance;
                        await counterInstance.PreventOnCompleteEvent();
                        await counterInstance.OnInit(questDef, subcounter.Target, repository);
                    }
                }
        }

        public async Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            foreach (var subcounter in SubCounters)
            {
                await subcounter.Counter.PreventOnCompleteEvent();
                await subcounter.Counter.OnDatabaseLoad(repository);
            }


            await Subscribe(repository, true);
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            if( ! _counterCompleted)
                await Subscribe(repository, false);
        }

        private async Task Subscribe(IEntitiesRepository repository, bool subscribe)
        {
            foreach (var subcounter in SubCounters)
            {
                if (subscribe)
                    subcounter.Counter.OnCounterChanged += CounterInstance_OnCounterChanged;
                else
                    subcounter.Counter.OnCounterChanged -= CounterInstance_OnCounterChanged;
            }

            if (subscribe)
                await IncreaseItems();
        }

        private async Task CounterInstance_OnCounterChanged(IQuestCounter arg)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;

            await IncreaseItems();
        }

        private async Task IncreaseItems()
        {
            if (_counterCompleted)
                return;

            await SetCombinatorCount(Math.Min(Def.Count, SubCounters.Sum(c => c.Counter.Count)));

            if (Count >= Def.Count)
            {
                _counterCompleted = true;
                await Subscribe(EntitiesRepository, false);
                await OnDestroySubcounters(EntitiesRepository);
                await OnOnCounterChangedInvoke(this);
                await OnOnCounterCompletedInvoke(QuestDef, this);
                return;
            }
            await OnOnCounterChangedInvoke(this);
        }
        private void dump(string state = "Current")
        {
            string str = $"{state} state:\n\tCombinatorCount: {Count} / {Def.Count}\nSubCounterState:";
            foreach (var item in SubCounters)
                str += $"\n\t\tCounterDef:{item.Counter.CounterDef.____GetDebugShortName()}, count: {item.Counter.Count}";

            str += "\n";
            Logger.IfError()?.Message(str).Write();
        }
        public async Task OnDestroySubcounters(IEntitiesRepository repository)
        {
            foreach (var subcounter in SubCounters)
                await subcounter.Counter.OnDestroy(repository);
        }
        public async Task SetCombinatorCountImpl(int Count)
        {
            this.Count = this.CountForClient = Count;
        }
        public Task PreventOnCompleteEventImpl()
        {
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("CombinatorCounter: Def = {0}, Def = ", Def.____GetDebugShortName());
            return sb.ToString();
        }
    }
}
