using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourceSystem.Utils;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace ResourceSystem.Reactions
{
    public abstract class ReactionHandlerDef : BaseResource, IReactionHandlerDescriptor
    {
        public abstract bool IsDummy { get; }
        public abstract string HandlerToString();
    }
    
    public class ReactionHandlerSpellDef : ReactionHandlerDef, IReactionHandlerSpellDescriptor
    {
        [JsonProperty(Required = Required.Always)][UsedImplicitly] public ResourceRef<SpellDef> Spell;
        [UsedImplicitly] public Dictionary<ResourceRef<SpellContextValueDef>, ResourceRef<VarDef>> Params;
        
        public override bool IsDummy => false;
        public override string HandlerToString() => $"{Spell.Target.____GetDebugAddress()}";
        SpellDef IReactionHandlerSpellDescriptor.Spell => Spell.Target;
        IEnumerable<(ISpellParameterDef, IVar)> IReactionHandlerSpellDescriptor.Params => Params?.Select(x => ((ISpellParameterDef)x.Key.Target, (IVar)x.Value.Target)) ?? Enumerable.Empty<(ISpellParameterDef, IVar)>();
    }

    public class ReactionHandlerSpellOnTargetDef : ReactionHandlerDef, IReactionHandlerSpellOnTargetDescriptor
    {
        [JsonProperty(Required = Required.Always)][UsedImplicitly] public ResourceRef<SpellDef> Spell;
        [JsonProperty(Required = Required.Always)][UsedImplicitly] public ResourceRef<VarDef<OuterRef>> Target;
        [UsedImplicitly] public Dictionary<ResourceRef<SpellContextValueDef>, ResourceRef<VarDef>> Params;
        
        public override bool IsDummy => false;
        public override string HandlerToString() => $"{Spell.Target.____GetDebugAddress()}";
        SpellDef IReactionHandlerSpellOnTargetDescriptor.Spell => Spell.Target;
        IVar<OuterRef> IReactionHandlerSpellOnTargetDescriptor.Target => Target.Target;
        IEnumerable<(ISpellParameterDef, IVar)> IReactionHandlerSpellOnTargetDescriptor.Params => Params?.Select(x => ((ISpellParameterDef)x.Key.Target, (IVar)x.Value.Target)) ?? Enumerable.Empty<(ISpellParameterDef, IVar)>();
    }
    
    public class ReactionHandlerSpellProlongableDef : ReactionHandlerSpellDef, IReactionHandlerSpellProlongableDescriptor
    {
        [UsedImplicitly] public float Timeout;
        public override string HandlerToString() => $"{Spell.Target.____GetDebugAddress()} Timeout:{Timeout}";
        float IReactionHandlerSpellProlongableDescriptor.Timeout => Timeout;
    }
}