using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Utils;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ResourceSystem.Aspects.ManualDefsForSpells
{
    public abstract class ShapeDef : BaseResource
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int MaxTargetCount { get; set; } = -1;
        public bool CheckTargetOnly { get; set; } = false;

        public abstract float GetBoundingRadius();
    }
    public class ImpactInShapeDef : SpellImpactDef
    {
        public List<ResourceRef<SpellDef>> AppliedSpells { get; set; }
        public ResourceRef<ShapeDef> Shape { get; set; }
        public ResourceRef<PredicateDef> PredicateOnTarget { get; set; }
    }
  
    public class BoxShapeDef : ShapeDef
    {
        public Vector3 Extents { get; set; }
        public override float GetBoundingRadius()
        {
            if (Extents.x == Extents.y && Extents.x == Extents.z)
                return (Extents.x * 2) * 1.732050807568877f;
            else
                return (float)Math.Sqrt((Extents.x * Extents.x) + (Extents.y * Extents.y) + (Extents.z * Extents.z)) * 2;
        }
    }
    public class SphereShapeDef : ShapeDef
    {
        public float Radius { get; set; }

        public override float GetBoundingRadius()
        {
            return Radius;
        }
    }
    public class LinksOfTargetDef : ShapeDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<LinkTypeDef> LinkType { get; set; }

        public override float GetBoundingRadius()
        {
            return 0;
        }
    }


}
