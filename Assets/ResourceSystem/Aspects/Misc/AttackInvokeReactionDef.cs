using System;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class AttackInvokeReactionDef : AttackActionDef
    {
        [UsedImplicitly][JsonProperty(Required = Required.Always)] public ResourceRef<ReactionDef> Reaction { get; set; }
        [UsedImplicitly] public AttackArgsMappingDef Args { get; set; }

        
        public override string ToString() => $"When:{When} Reaction:{Reaction.Target.____GetDebugAddress()}";

        public override BaseResource IdResource => Reaction;
    }
}