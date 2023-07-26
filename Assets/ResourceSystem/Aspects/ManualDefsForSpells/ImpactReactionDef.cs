using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourceSystem.Reactions;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public sealed class ImpactReactionDef : SpellImpactDef
    {
        [UsedImplicitly] 
        public ResourceRef<SpellEntityDef> Target { get; private set; }
        
        [UsedImplicitly, JsonProperty(Required = Required.Always)] 
        public ResourceRef<ReactionDef> Reaction { get; private set; }        
        
        [UsedImplicitly] 
        public Dictionary<ResourceRef<ArgDef>,ResourceRef<ArgValueDef>> Args { get; private set; }

        public class ArgValueDef : BaseResource {}

        public class ArgValueFloatDef : ArgValueDef
        {
            [UsedImplicitly] public ResourceRef<CalcerDef<float>> Value;
        }
        
        public class ArgValueVector2Def : ArgValueDef
        {
            [UsedImplicitly] public ResourceRef<CalcerDef<float>> X, Y;
        }

        public class ArgValueVector3Def : ArgValueDef
        {
            [UsedImplicitly] public ResourceRef<CalcerDef<float>> X, Y, Z;
        }
    }
}