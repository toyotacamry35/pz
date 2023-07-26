using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using ColonyShared.ManualDefsForSpells;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Src.Effects
{
    public class EffectInventoryObserver : IEffectBinding<EffectInventoryObserverDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(EffectInventoryObserver));

        // TODO: это НЕПРАВИЛЬНЫЙ способ хранения, так как не будет работать с телепортом. Необходимо перенести в entity персонажа.
        private static readonly ConcurrentDictionary<SpellPartGlobalCastId, EffectInstance> Instances =
            new ConcurrentDictionary<SpellPartGlobalCastId, EffectInstance>();

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectInventoryObserverDef indef)
        {
            if (cast.IsSlave)
                return;

            var def =  indef;
            var targetRef = cast.Caster;

            if (def.Target.Target != null)
                targetRef = await def.Target.Target.GetOuterRef(cast, repo);

            if(!targetRef.IsValid)
                targetRef = cast.Caster;

            var instance = new EffectInstance(targetRef, def, repo);
            if (Instances.TryAdd(cast.WordGlobalCastId(indef), instance))
            {
                using (var entityContainer = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    var hasInventory = entityContainer.Get<IHasInventory>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Master);
                    hasInventory.Inventory.ItemAddedToContainer += instance.OnItemAdded;
                    hasInventory.Inventory.ItemRemovedToContainer += instance.OnItemRemoved;
                }
                await instance.OnAttach();
            }
            return;
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectInventoryObserverDef indef)
        {
            if (cast.IsSlave)
                return;

            var def = indef;
            EffectInstance instance;
            if (Instances.TryRemove(cast.WordGlobalCastId(def), out instance))
            {
                var targetRef = cast.Caster;
                if (def.Target.Target != null)
                    targetRef = await def.Target.Target.GetOuterRef(cast, repo);

                if (!targetRef.IsValid)
                    targetRef = cast.Caster;

                using (var entityContainer = await repo.Get(targetRef.TypeId, targetRef.Guid))
                {
                    var hasInventory = entityContainer.Get<IHasInventory>(targetRef.TypeId, targetRef.Guid, ReplicationLevel.Master);
                    hasInventory.Inventory.ItemAddedToContainer -= instance.OnItemAdded;
                    hasInventory.Inventory.ItemRemovedToContainer -= instance.OnItemRemoved;
                }
            }
        }

        class EffectInstance
        {
            private readonly EffectInventoryObserverDef _effectDef;
            private readonly OuterRef<IEntity> _target;
            private readonly IEntitiesRepository _repository;
            private int _itemsCount = -1;

            public EffectInstance(OuterRef<IEntity> target, EffectInventoryObserverDef effectDef, IEntitiesRepository repository)
            {
                _target = target;
                _effectDef = effectDef;
                _repository = repository;
            }

            public Task OnAttach()
            {
                return CheckInventoryAndCastSpells(_effectDef.SpellOnPresence, _effectDef.SpellOnAbsence, 0);
            }

            public Task OnItemAdded(BaseItemResource item, int index, int count, bool manual)
            {
                    return CheckInventoryAndCastSpells(_effectDef.SpellOnAppearance, _effectDef.SpellOnDisappearance, 0);
            }

            public Task OnItemRemoved(BaseItemResource item, int index, int count, bool manual)
            {
                return CheckInventoryAndCastSpells(_effectDef.SpellOnAppearance, _effectDef.SpellOnDisappearance, -count); // при удалении, эвент вызывается ДО фактического уменьшения итемов в инвентаре 
            }

            private async Task CheckInventoryAndCastSpells(SpellDef spellAppearance, SpellDef spellDisappearance, int itemsChange)
            {
                using (var entityContainer = await _repository.Get(_target.TypeId, _target.Guid))
                {
                    var hasInventory = entityContainer.Get<IHasInventoryServer>(_target.TypeId, _target.Guid, ReplicationLevel.Server);
                    var itemsCount = hasInventory.Inventory.Items.Where(x => x.Value.Item.ItemResource == _effectDef.Item).Sum(x => x.Value.Stack) + itemsChange;
                    if ((_itemsCount == -1 || _itemsCount < _effectDef.Count) && itemsCount >= _effectDef.Count)
                        await CastSpell(entityContainer, _target, _effectDef.SpellOnAppearance, _repository);
                    else
                    if ((_itemsCount == -1 || _itemsCount >= _effectDef.Count) && itemsCount < _effectDef.Count)
                        await CastSpell(entityContainer, _target, _effectDef.SpellOnDisappearance, _repository);
                    _itemsCount = itemsCount;
                }
            }
            
            private static async Task CastSpell(IEntitiesContainer container, OuterRef<IEntity> target, SpellDef spellDef, IEntitiesRepository repo)
            {
                Logger.IfDebug()?.Message("CastSpell: {0} for {1}", spellDef, target).Write();
                if (spellDef == null)
                    return;
                var hasWizard = container.Get<IHasWizardEntityServer>(target.TypeId, target.Guid, ReplicationLevel.Server);
                using (var entCont2 = await repo.Get(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id))
                {
                    var wizard = entCont2.Get<IWizardEntityServer>(hasWizard.Wizard.TypeId, hasWizard.Wizard.Id, ReplicationLevel.Server);
                    await wizard.CastSpell(new SpellCast {Def = spellDef});
                }
            }
        }
    }
}