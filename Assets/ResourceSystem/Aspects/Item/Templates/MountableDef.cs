using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities;
using L10n;
using SharedCode.Entities.GameObjectEntities;
using UnityEngine;
using ResourceSystem.Aspects.Templates;
using SharedCode.Wizardry;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public abstract class MountableDef : SaveableBaseResource, IEntityObjectDef, IBruteDef, IHasDefaultStatsDef
    {
        public virtual bool SelfRegistry => false;
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
        public LocalizedString NameLs { get; set; }
        public List<ResourceRef<ShapeDef>> Colliders { get; set; }
        public ResourceRef<WeaponColliderDef> AttackCollider { get; set; }
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
        public float ForwardDamageMultiplier { get; set; }
        public float SideDamageMultiplier { get; set; }
        public float BackwardDamageMultiplier { get; set; }
        public ResourceRef<StatisticType> ObjectType { get; set; }
        public float DestructionPowerRequired { get; set; } = 100;
    }

    [Localized]
    public abstract class MountableWithTypeDef : MountableDef
    {
        public ResourceRef<WorkbenchTypeDef> WorkbenchType { get; set; }
    }

    [Localized]
    public class WorldMachineDef : MountableWithTypeDef, IHasStatsDef, IHasInitialSpells
    {
        public FuelDef[] AcceptableFuel { get; set; }
        public int InventorySize { get; set; }
        public int FuelContainerSize { get; set; }
        public int OutContainerSize { get; set; }
        public UnityRef<Sprite> OnIcon { get; set; }
        public UnityRef<Sprite> OffIcon { get; set; }
        public UnityRef<Sprite> TitleIcon { get; set; }
        public ResourceRef<StatsDef> Stats { get; set; }
        public List<ResourceRef<SpellDef>> InitialSpells { get; set; } = new List<ResourceRef<SpellDef>>();
    }

    public struct FuelDef
    {
        public ResourceRef<BaseItemResource> Fuel { get; set; }
        public float BurnTime { get; set; }
    }

    [Localized]
    public class WorldBenchDef : MountableWithTypeDef, IHasStatsDef
    {
        public UnityRef<Sprite> TitleIcon { get; set; }
        public ResourceRef<StatsDef> Stats { get; set; }
    }

    [Localized]
    public class WorldBakenDef : MountableDef, IHasStatsDef, IHasDefaultStatsDef
    {
        public float Cooldown { get; set; }
        public float VerticalDistanceFromSpawnPointToCenter { get; set; }
        public ResourceRef<StatsDef> Stats { get; set; }
        public ResourceRef<ItemSpecificStats> DefaultStats { get; set; }
    }

    [Localized]
    public class CharacterChestDef : WorldBoxDef, IHasStatsDef
    {
        public ResourceRef<StatsDef> Stats { get; set; }
    }

    [Localized]
    public class WorldBoxDef : MountableDef, ILimitedLifetimeDef
    {
        public int Size { get; set; } = 100;
        public ResourceRef<LimitedLifetimeDef> LimitedLifetimeDef { get; set; }
    }

    [Localized]
    public class WorldPersonalMachineDef : MountableDef, IHasStatsDef, IHasInitialSpells
    {
        public ResourceRef<WorkbenchTypeDef> WorkbenchType { get; set; }
        public UnityRef<Sprite> TitleIcon { get; set; }
        public ResourceRef<StatsDef> Stats { get; set; }
        public List<ResourceRef<SpellDef>> InitialSpells { get; set; } = new List<ResourceRef<SpellDef>>();
        public int OutContainerSize { get; set; }
        public int MaxQueueSize { get; set; }

        //TODO: Узнать как делаются SLotType для информации о типе ячеек(контейнера)
    }
}