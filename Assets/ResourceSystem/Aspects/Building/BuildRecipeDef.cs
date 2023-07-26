using Assets.ColonyShared.SharedCode.Aspects;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using L10n;
using UnityEngine;

namespace SharedCode.Aspects.Building
{
    [Localized]
    public class BuildRecipeDef : BaseRecipeDef
    {
        public float DestructionPowerRequired { get; set; } = 100;
        public enum BuildType
        {
            None = 0,
            Building = 1,
            Fence = 2,
        }

        public class ElementStats
        {
            public float Health { get; set; }
            public float PassiveDamage { get; set; }
            public ResourceRef<DamageTypeDef> PassiveDamageType { get; set; } // По-умолчанию (если null) используется GlobalConstsHolder.GlobalConstsDef.DefaultDamageType (см. DamagePipelineHelper)
            public float DamageResistanceSlashing { get; set; }
            public float DamageResistanceCrushing { get; set; }
            public float DamageResistancePiercing { get; set; }
        }

        public class ElementResource
        {
            public class Resource
            {
                public int ClaimCount { get; set; }
                public int ReclaimCount { get; set; }
                public ResourceRef<BaseItemResource> Item { get; set; }
            }

            public List<Resource> Resources { get; set; }
        }

        public class ElementInteraction
        {
            public class Interaction
            {
                public int NextInteraction { get; set; }
            }

            public bool Enable { get; set; }
            public int DefaultInteraction { get; set; }
            public List<Interaction> Interactions { get; set; }
        }

        public class ElementVisual
        {
            public class Version
            {
                public int Weight { get; set; }
                public string Name { get; set; }
            }
            public UnityRef<GameObject> HitPrefab { get; set; } // hit prefab
            public float HitTime { get; set; } // self destruct time for destruction animation
            public float SelfDestructTime { get; set; } // self destruct time for destruction animation
            public bool VersionsEnable { get; set; }
            public string VersionsCommon { get; set; }
            public List<Version> Versions { get; set; }
        }

        public override string ToString()
        {
            return $"[{nameof(BuildRecipeDef)}: '{NameLs}' {nameof(Type)}={Type}]";
        }

        public BuildType Type { get; set; }

        public UnityRef<Sprite> Icon { get; set; }
        public UnityRef<Sprite> Image { get; set; }
        public int OrderIndex { get; set; }
        public ResourceRef<BuildRecipeGroupDef> BuildRecipeGroupDef { get; set; }

        public ResourceRef<BuildTimerDef> TimerDef { get; set; }
        public ResourceRef<BuildElementDef> ElementDef { get; set; }

        public ElementStats Stats { get; set; }
        public ElementResource Resource { get; set; }
        public ElementInteraction Interaction { get; set; }
        public ElementVisual Visual { get; set; }
        public UnityRef<GameObject> Prefab { get; set; }
        public ResourceRef<UnityGameObjectDef> PrefabDef { get; set; }
    }
}