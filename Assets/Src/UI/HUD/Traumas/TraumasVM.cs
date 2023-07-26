using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ReactiveProps;
using Assets.Src.Utils;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using L10n;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    public class TraumasVM : BindingVmodel
    {
        // private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ListStream<IStatusEffectVM> _statusEffectVMs;
        public IListStream<IStatusEffectVM> StatusEffectVMs => _statusEffectVMs;

        public TraumasVM(OuterRef<IEntityObject> ego)
        {
            var touchableEntity = TouchableUtils.CreateEgoProxy<IWorldCharacterClientFull>(); // TouchableUtils.CreateEgoProxy<IHasBuffClientFull>(); // бл@#$!!
            D.Add(touchableEntity);

            touchableEntity.Connect(
                GameState.Instance.ClientClusterNode,
                ego.TypeId,
                ego.Guid,
                ReplicationLevel.ClientFull);
            var buffs = touchableEntity
                .Child(D, e => e.Buffs)
                .ToDictionaryStream(D, t => t.All,
                    (buffId, buff, buffTouchableFactory) => new SpellCastVM(buff))
                .ToListStream(D);
            
            var touchableWizard = TouchableUtils.CreateEgoProxy<IWizardEntityClientFull>();
            D.Add(touchableWizard);

            touchableWizard.Connect(
                GameState.Instance.ClientClusterNode,
                WizardEntity.StaticTypeId,
                ego.Guid,
                ReplicationLevel.ClientFull);
            var spells = touchableWizard.ToDictionaryStream(
                D,
                wizard => wizard.Spells,
                (spellId, spell, spellTouchableFactory) => new SpellCastVM(spell)
            ).ToListStream(D);

            _statusEffectVMs = buffs
                .Where(D, vm => vm.IsStatusEffect && vm.Sprite)
                .Func(D, vm => (IStatusEffectVM) vm)
                .ConcatListStreams(D, 
                    spells
                    .Where(D, vm => vm.IsStatusEffect && vm.Sprite)
                    .Func(D, vm => (IStatusEffectVM) vm)
                );

            D.Add(_statusEffectVMs);
        }
    }

    [Binding]
    public class SpellCastVM : BindingVmodel, IStatusEffectVM
    {
        public SpellCastVM(ISpellClientFull currentSpell)
        {
            var cast = currentSpell.CastData;

            StartTime = cast.StartAt;
            Duration = cast.Duration;
            SpriteRef = cast.Def?.Icon;
            IsInfinite = cast.Def?.IsInfinite ?? false;
            Description = cast.Def?.DescriptionLs ?? default;
            IsStatusEffect = cast.Def?.IsStatusEffect ?? false;
        }

        public SpellCastVM(IBuffClientFull buff)
        {
            StartTime = buff.StartTime;
            Duration = buff.Duration;
            SpriteRef = buff.Def?.Icon;
            IsInfinite = buff.Def?.IsInfinite ?? false;
            Description = buff.Def?.DescriptionLs ?? default;
            IsStatusEffect = buff.Def?.IsStatusEffect ?? false;
        }
 
        private UnityRef<Sprite> SpriteRef { get; }
        public bool IsStatusEffect { get; }
        public long StartTime { get; }
        public long Duration { get; }
        public bool IsInfinite { get; }
        public Sprite Sprite => SpriteRef?.Target;
        public LocalizedString Description { get; }
    }
}