using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
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
    public interface IBuildCounter : IQuestCounter
    {

    }


    public partial class BuildCounter 
    {
        private BuildCounterDef Def => (BuildCounterDef)CounterDef;
        private bool _counterCompleted = false;
        private bool unsubscribe = true;

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
            if ( ! _counterCompleted)
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
                    (await entity.StatisticEngine.GetStatisticRouter<BuildEventArgs>()).RoutedEvent += StatisticsCounter_RoutedEvent;
                else
                    (await entity.StatisticEngine.GetStatisticRouter<BuildEventArgs>()).RoutedEvent -= StatisticsCounter_RoutedEvent;

            }
        }

        private async Task StatisticsCounter_RoutedEvent(BuildEventArgs arg)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;
            
            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                if (!Def.HaveTargets || Def.AllTargets.Contains(arg.Building))
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
            sb.AppendFormat("BuildCounter: Def = {0}, Buildings = ", Def.____GetDebugShortName());
            foreach (var target in Def.AllTargets)
                sb.AppendFormat("{0}; ", target.____GetDebugShortName());
            return sb.ToString();
        }
    }
}
