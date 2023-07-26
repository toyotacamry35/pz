using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
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
    public interface IDealDamageCounter : IQuestCounter
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        float SumValue { get; }
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        float SumValueForClient { get; }
    }


    public partial class DealDamageCounter
    {
        private DealDamageCounterDef Def => (DealDamageCounterDef)CounterDef;
        private bool _counterCompleted = false;

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
                    (await entity.StatisticEngine.GetStatisticRouter<DealDamageEventArgs>()).RoutedEvent += StatisticsCounter_RoutedEvent;
                else
                    (await entity.StatisticEngine.GetStatisticRouter<DealDamageEventArgs>()).RoutedEvent -= StatisticsCounter_RoutedEvent;

            }
        }

        private async Task StatisticsCounter_RoutedEvent(DealDamageEventArgs arg)
        {
            if (_counterCompleted)
                return;

            if (parentEntity == null)
                return;
            
            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                if (arg.ObjectType != Def.ObjectType)
                    return;
                if (!Def.HaveTargets || Def.AllTargets.Contains(arg.TargetObjectDef))
                {
                    SumValueForClient = SumValue += arg.Value;
                    if (SumValue >= Def.Value)
                    {
                        SumValueForClient = SumValue = 0f;
                        await IncreaseItems();
                    }
                }
            }
        }

        private async Task IncreaseItems()
        {
            if (_counterCompleted)
                return;

            if ((Count = CountForClient = 1 + Count) >= 1)
            {
                Count = CountForClient = 1;
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
            sb.AppendFormat("DealDamageCounter: Def = {0}, Spells = ", Def.____GetDebugShortName());
            foreach (var target in Def.AllTargets)
                sb.AppendFormat("{0}; ", target.____GetDebugShortName());
            return sb.ToString();
        }
    }


    public static class IResourceExtension
    {
        public static string ____GetDebugShortName(this IResource _this)
        {
            var addr = _this.Address.ToString();
            var idx = addr.LastIndexOf('/');
            return idx != -1 ? addr.Substring(idx) : addr;
        }
    }
}
