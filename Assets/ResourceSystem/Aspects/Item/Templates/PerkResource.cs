using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects;
using SharedCode.Entities;
using UnityEngine;

namespace SharedCode.Aspects.Item.Templates
{
    [Localized]
    public class PerkResource : BaseItemResource, IHasStatsResource, IHasPackDef
    {
        public PerkResource()
        {
            MaxStack = 1;
        }

        public ResourceRef<ItemGeneralStats> GeneralStats { get; set; }

        public ResourceRef<ItemSpecificStats> SpecificStats { get; set; }

        public SpellModifiersTuple SpellModifiers;
        
        public PackDef PackDef { get; set; }

        [KnownToGameResources]
        public struct SpellModifiersTuple
        {
            public ResourceRef<PredicateDef> Condition;
            public ResourceArray<SpellModifierDef> Modifiers;
            public void Deconstruct(out PredicateDef condition, out IReadOnlyList<SpellModifierDef> modifiers)
            {
                condition = Condition;
                modifiers = Modifiers;
            }
        }
    }
}
