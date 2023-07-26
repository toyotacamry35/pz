using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using ColonyShared.SharedCode.Modifiers;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using ResourceSystem.Aspects;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public partial class SpellModifiersCollector : IHookOnInit, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public Task OnInit()
        {
            return Initialization();
        }

        public Task OnDatabaseLoad()
        {
            return Initialization();
        }
        
        public ValueTask<bool> AddModifiersImpl(SpellModifiersCauser causer, PredicateDef condition, SpellModifierDef[] modifiers)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add Modifier | Causer:{causer} Condition:{condition} Modifiers:{modifiers.ToStringExt()}").Write();
            var hasModifiers = (IHasSpellModifiers)parentEntity;
            if (hasModifiers.SpellModifiers == null )
                hasModifiers.SpellModifiers = new DeltaDictionary<SpellModifiersCauser, SpellModifiersCollectionEntry>();
            hasModifiers.SpellModifiers.Add(causer, new SpellModifiersCollectionEntry(condition, modifiers));
            return new ValueTask<bool>(true);
        }

        public ValueTask<bool> RemoveModifiersImpl(SpellModifiersCauser causer)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Remove Modifier | Causer:{causer}").Write();
            var hasModifiers = (IHasSpellModifiers)parentEntity;
            if (hasModifiers.SpellModifiers != null)
                hasModifiers.SpellModifiers.Remove(causer);
            return new ValueTask<bool>(true);
        }
        
        private async Task Initialization()
        {
            using (var wrapper = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.Id))
            {
                var hasPerks = wrapper?.Get<IHasPerks>(parentEntity.TypeId, parentEntity.Id, ReplicationLevel.Master);
                if (hasPerks != null)
                {
                    await InitPerksList(hasPerks.PermanentPerks.Items);
                }
            }
        }

        private async Task InitPerksList(IDeltaDictionary<int, ISlotItem> items)
        {
            items.OnItemAddedOrUpdated += OnPerkAddedOrUpdated;
            items.OnItemRemoved += OnPerkRemoved;
            foreach (var perk in items.Values)
                await UpdatePerk(null, perk?.Item);
        }

        private async Task OnPerkAddedOrUpdated(DeltaDictionaryChangedEventArgs<int, ISlotItem> eventArgs)
        {
            using (await parentEntity.GetThis())
                await UpdatePerk(eventArgs.OldValue?.Item, eventArgs.Value?.Item).AsTask();
        }
        
        private async Task OnPerkRemoved(DeltaDictionaryChangedEventArgs<int, ISlotItem> eventArgs)
        {
            using (await parentEntity.GetThis())
                await UpdatePerk(eventArgs.OldValue?.Item, null).AsTask();
        }

        private async ValueTask UpdatePerk(IItem oldPerk, IItem newPerk)
        {
            
             if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Spell Modifiers From Perk | OldPerk:{oldPerk?.Id} {oldPerk?.ItemResource.____GetDebugAddress()} NewPerk:{newPerk?.Id} {newPerk?.ItemResource.____GetDebugAddress()}").Write();
             if (oldPerk != null)
             {
                 await RemoveModifiers(new SpellModifiersCauser(oldPerk.Id));
             }
             if (newPerk != null)
             {
                 var (condition, modifiers) = ((PerkResource) newPerk.ItemResource).SpellModifiers;
                 if (modifiers != null && modifiers.Count > 0)
                     await AddModifiers(new SpellModifiersCauser(newPerk.Id), condition, modifiers.ToArray());
             }
        }
    }
}