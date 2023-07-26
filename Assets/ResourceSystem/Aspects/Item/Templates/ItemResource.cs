using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using L10n;
using ResourceSystem.Aspects.Misc;
using SharedCode.Entities;
using Src.ManualDefsForSpells;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class ItemResource : BaseItemResource, IHasStatsResource, IHasStatsDef, IHasPackDef
    {
        /// <summary>
        /// Модификаторы аниматора применяемые при активации данного предмета 
        /// </summary>
        public ResourceArray<EffectAnimatorDef.AnimatorModifierDef> CharacterAnimatorModifiers { get; [UsedImplicitly] set; }

        /// <summary>
        /// Максимальное число айтемов в контейнере сабайтемов (айтем - это патроны/напиток с количеством в поле count)
        /// </summary>
        public int InnerContainerSize { get; set; }

        public ResourceRef<BaseItemResource>[] ApplicableInnerItems { get; set; }

        public ResourceRef<CalcerPiecewiseResourceDef> ActionOnDeathCalcer { get; set; }

        public ResourceRef<DurabilityDef> Durability { get; set; }

        public int Tier { get; set; }

        public ResourceRef<InventoryFiltrableTypeDef> InventoryFiltrableType { get; set; }

        public ResourceRef<ItemGeneralStats> GeneralStats { get; set; }

        public ResourceRef<ItemSpecificStats> SpecificStats { get; set; }

        public Dictionary<ResourceRef<InputActionHandlersLayerDef>, Dictionary<ResourceRef<InputActionDef>,ResourceRef<InputActionHandlerDef>>> InputActionHandlers { get; set; }
        
        public ResourceRef<BodyPartResource>[] HideBodyParts { get; set; }
        
        public ResourceRef<SlotDef>[] HideVisualSlots { get; set; }

        public ResourceRef<WeaponDef> WeaponDef { get; set; }

        public MountingData MountingData { get; set; }

        public ResourceRef<ConsumDef> ConsumableData { get; set; }

        public PackDef PackDef { get; set; }

        public ResourceRef<StatsDef> Stats { get; set; }

        /// <summary>
        /// Визуал показываемый на персонаже
        /// </summary>
        public UnityRef<GameObject> Visual { get; set; }

        public Dictionary<ResourceRef<VisualDollDef>, UnityRef<GameObject>> Visuals;

        public bool CorrectionInHandRequired;
    }

    public class DurabilityDef : BaseResource
    {
        public ItemPackDef[] ItemsOnBreak { get; set; }
        public ItemPackDef[] ItemsOnFullBreak { get; set; }
        public ItemPackDef[] ItemsOnZeroDurability { get; set; }

        public float FullBreakDurability { get; set; } = 0.333f;
        public float IncreaseDurabilityOnRepair { get; set; } = 0.25f;
        public float DecreaseMaxDurabilityOnRepair { get; set; } = 0.0666f;
        public ResourceRef<RepairRecipeDef> RepairRecipe { get; set; }
    }
}
