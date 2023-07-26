using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Repositories;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class QuestObject
    {
        protected override void constructor()
        {
            base.constructor();
            PhaseSuccCounter = null;
            PhaseFailCounter = null;
        }
        public async Task SetStatusImpl(QuestStatus newStatus)
        {
            Status = newStatus;
        }
        public async Task AddPhaseCountersImpl(IQuestCounter succCounter, IQuestCounter failCounter)
        {
            if (PhaseSuccCounter != null)
            {
                PhaseSuccCounter.OnCounterChanged -= Counter_OnCounterChanged;
                await PhaseSuccCounter.OnDestroy(EntitiesRepository);
            }

            PhaseSuccCounter = succCounter;
            HavePhaseSuccCounter = PhaseSuccCounter != null;

            if (PhaseFailCounter != null)
            {
                PhaseFailCounter.OnCounterChanged -= Counter_OnCounterChanged;
                await PhaseFailCounter.OnDestroy(EntitiesRepository);
            }

            PhaseFailCounter = failCounter;
            HavePhaseFailCounter = PhaseFailCounter != null;

            if (HavePhaseSuccCounter)
                PhaseSuccCounter.OnCounterChanged += Counter_OnCounterChanged;
            if (HavePhaseFailCounter)
                PhaseFailCounter.OnCounterChanged += Counter_OnCounterChanged;

        }

        private async Task Counter_OnCounterChanged(IQuestCounter arg)
        {
            if (arg.CounterDef is CounteredQuestCounterDef counter)
                await RunImpacts(counter.OnEveryCounterChangeImpacts);
        }

        private async Task RunImpacts(ResourceRef<SpellImpactDef>[] impacts)
        {
            if (impacts == null || impacts.Length == 0)
                return;

            var mockCastData = new SpellWordCastData(
                wizard: new OuterRef<IWizardEntity>(parentEntity.Id, ReplicaTypeRegistry.GetIdByType(typeof(IWizardEntity))),
                castData: new SpellCast(),
                caster: new OuterRef<IEntity>(parentEntity.Id, parentEntity.TypeId),
                spellId: SpellId.FirstMasterValid,
                subSpellCount: 0,
                currentTime: 0,
                wordTimeRange: default,
                spellStartTime: 0,
                parentSubSpellStartTime: 0,
                slaveMark: null,
                firstOrLast: false,
                canceled: false,
                context: null,
                modifiers: null,
                repo: EntitiesRepository
            );

            foreach (var impact in impacts)
            {
                if (impact == null)
                    continue;
                await SpellImpacts.CastImpact(mockCastData, impact, EntitiesRepository);
            }
        }
    }
}
