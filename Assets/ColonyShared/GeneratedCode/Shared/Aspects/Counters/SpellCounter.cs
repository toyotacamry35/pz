using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Wizardry;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Serializers.Protobuf;
using GeneratedCode.EntitySystem;
using System;

namespace GeneratedCode.DeltaObjects
{

    [GenerateDeltaObjectCode]
    public interface ISpellCounter : IQuestCounter
    {
    }


    public partial class SpellCounter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private SpellCounterDef Def => (SpellCounterDef)CounterDef;
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
            if (parentEntity == null)
                Logger.IfError()?.Message("SpelLCounter parent entity is null on database load, this is very bad").Write();

            _counterCompleted = false;
            await Subscribe(true, repository);
        }

        public async Task OnDestroyImpl(IEntitiesRepository repository)
        {
            if (!_counterCompleted)
                await Subscribe(false, repository);
        }

        private async Task Subscribe(bool subscribe, IEntitiesRepository repository)
        {
            OuterRef<IWizardEntity> wizardRef;
            using (var wrapper = await repository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var hasWizardRef = wrapper.Get<IHasWizardEntityServer>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Server);
                if (hasWizardRef == null)
                    return;

                wizardRef = new OuterRef<IWizardEntity>(hasWizardRef.Wizard.Id, hasWizardRef.Wizard.TypeId);
            }

            if (wizardRef.IsValid)
            {
                using (var wizardWrapper = await repository.Get(wizardRef.TypeId, wizardRef.Guid))
                {
                    var wizardEntity = wizardWrapper.Get<IWizardEntityServer>(wizardRef.TypeId, wizardRef.Guid, ReplicationLevel.Server);
                    if (wizardEntity == null)
                        return;

                    if (subscribe)
                    {
                        wizardEntity.Spells.OnItemAddedOrUpdated += Spells_OnItemAddedWrap;
                        if (Def.AllTargets.Intersect(wizardEntity.Spells.Select(v => v.Value.CastData.Def)).Any())
                            await IncreaseItems();
                    }
                    else
                        wizardEntity.Spells.OnItemAddedOrUpdated -= Spells_OnItemAddedWrap;
                }
            }
        }

        private async Task Spells_OnItemAddedWrap(DeltaDictionaryChangedEventArgs<SpellId, ISpellServer> args)
        {
            if (_counterCompleted)
                return;
            else if (parentEntity == null)
            {
                //Logger.IfError()?.Message("SpellCounter {0}. parentEntity == null, parentDeltaObject == {1}", this, this.parentDeltaObject?.GetType()?.GetFriendlyName() ?? "null").Write();
                return;
            }
            if (Def.AllTargets.Any(x => x == args.Value.CastData.Def))
                using (await parentEntity.GetThisExclusive())
                {
                    await Spells_OnItemAdded(args);
                }
        }
        private async Task Spells_OnItemAdded(DeltaDictionaryChangedEventArgs<SpellId, ISpellServer> args)
        {
            if (_counterCompleted)
                return;
            using (var sender = await EntitiesRepository.Get(args.Sender.ParentTypeId, args.Sender.ParentEntityId))
            {
                if (!Def.HaveTargets || Def.AllTargets.Contains(args.Value.CastData.Def))
                    using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
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
                await Subscribe(false, EntitiesRepository);
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
            sb.AppendFormat("SpellCounter: Def = {0}, Spells = ", Def.____GetDebugShortName());
            foreach (var target in Def.AllTargets)
                sb.AppendFormat("{0}; ", target?.____GetDebugShortName() ?? "<null>");
            return sb.ToString();
        }
    }
}
