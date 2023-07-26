using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using NLog;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.Quests;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{

    [GenerateDeltaObjectCode]
    public interface ICraftCounter : IQuestCounter
    {

    }


    public partial class CraftCounter
    {
        private CraftCounterDef Def => (CraftCounterDef)CounterDef;
        private bool _counterCompleted = false;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            QuestDef = questDef;
            CounterDef = counterDef;
            _counterCompleted = false;
            return Task.CompletedTask;
        }

        public async Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            await Subscribe(repository, true);
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            if (!_counterCompleted)
                await Subscribe(repository, false);
        }

        private async Task Subscribe(IEntitiesRepository repository, bool subscribe)
        {
            using (var wrapper = await repository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var entity = wrapper.Get<IHasStatisticsServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                if (entity == null)
                    return;

                if (subscribe)
                    (await entity.StatisticEngine.GetStatisticRouter<CraftEventArgs>()).RoutedEvent += StatisticsCounter_RoutedEvent;
                else
                    (await entity.StatisticEngine.GetStatisticRouter<CraftEventArgs>()).RoutedEvent -= StatisticsCounter_RoutedEvent;

            }
        }

        private async Task StatisticsCounter_RoutedEvent(CraftEventArgs arg)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;

            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                if ((!Def.HaveTargets && Def.CraftSource.CheckFlag(arg.CraftSource)) || (Def.CraftSource.CheckFlag(arg.CraftSource) && Def.AllTargets.Contains(arg.Recipe)))
                    await IncreaseItems();
                
            }
        }

        private async Task IncreaseItems()
        {
            if (_counterCompleted)
                return;
            if ((Count = CountForClient = 1 + Count) >= Def.Count)
            {
                Count = CountForClient = Def.Count;
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
            sb.AppendFormat("CraftCounter: Def = {0}, Recipes = ", Def.____GetDebugShortName());
            foreach (var target in Def.AllTargets)
                sb.AppendFormat("{0}; ", target.____GetDebugShortName());
            return sb.ToString();
        }
    }
}
