using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using L10n;
using UnityEngine;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Scripting;
using ColonyShared.SharedCode;
using JetBrains.Annotations;
using ResourceSystem.Aspects;

namespace SharedCode.Wizardry
{
    public abstract class SpellOrBuffDef : BaseResource
    {
        public LocalizedString DescriptionLs { get; [UsedImplicitly] set; }
        public UnityRef<Sprite> Icon { get; [UsedImplicitly] set; }
        public bool IsStatusEffect { get; set; } = false;
    }
    
    [Localized]
    public class SpellDef : SpellOrBuffDef
    {
        public bool IgnoresDeath { get; set; } = false;
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public bool IsInfinite { get; set; } = false;
        public bool AllowMultiple { get; set; } = false;
        [Obsolete] public UnityRef<Sprite> SpellIcon { get => Icon; set => Icon = value; }
        public ResourceRef<CooldownDef> Cooldown { get; set; }
        [Obsolete] public LocalizedString InteractionDescriptionLs { get => DescriptionLs; set => DescriptionLs = value; }
        public bool DontShowProgress { get; set; } = true;

        public ResourceRef<ScriptingContextDef> Context { get; set; } = new ScriptingContextDef();
        public ResourceRef<SpellGroupDef> Group { get; set; }
        public ResourceRef<SpellSlotDef> Slot { get; set; }
        public bool ClearsSlot { get; set; } = false;
        public int OutlineColorIndex { get; set; } = 0;
        public SubSpell[] SubSpells { get; set; } = Array.Empty<SubSpell>(); //Do not change to ResourceRef<SubSpell> - I identify different subspells with the same spell by ref
        public SpellWordDef[] Words { get; set; } = Array.Empty<SpellWordDef>(); //Do not change to ResourceRef<SpellWordDef> - I identify words on the same position with the same path by ref
        public SpellPredicateDef[] Predicates => _predicates ?? (_predicates = Words.Select(x => x.ResolveWordRef()).OfType<SpellPredicateDef>().ToArray());
        public SpellContextValueDef[] Params { get; set; }
        public ResourceArray<SpellTagDef> Tags { get; set; }
        public bool CanBeFinished { get; set; } = true;

        private SpellPredicateDef[] _predicates;
    }
    public class CooldownDef : BaseResource
    {
        public ResourceRef<CooldownGroupDef> Group { get; set; }
        public ResourceRef<CalcerDef<float>> Cooldown { get; set; }
        public bool FromEnd { get; set; } = false;
    }
    public class CooldownGroupDef : BaseResource
    {
    
    }


    public class SpellSlotDef : BaseResource
    {
       
    }
    
    public class BuffDef : SpellOrBuffDef, ISaveableResource
    {
        public bool IsInfinite { get; set; } = true;
        public float Duration { get; set; }
        public Guid Id { get; set; }
        public ResourceArray<SpellEffectDef> Effects { get; set; } = ResourceArray<SpellEffectDef>.Empty;
    }

}

namespace Scripting
{

    public class ScriptingContextDef : BaseResource
    {
        public ContextFieldDef<HostContextArg> Host { get; set; }
        public Dictionary<ResourceRef<ContextArgTypeDef>, ResourceRef<CalcerDef>> CustomArgs { get; set; }
    }

    public class BuffScriptingContextDef : ScriptingContextDef
    {
        public ContextFieldDef<SourceContextArg> Source { get; set; }
    }
    public class ActionScriptingContextDef : ScriptingContextDef
    {
        public ContextFieldDef<TargetContextArg> Target { get; set; }
    }
    public class ReactionScriptingContextDef : ScriptingContextDef
    {
        public ContextFieldDef<ProvocatorContextArg> Provocator { get; set; }
    }


    public interface IContextFieldDef
    {
        ResourceRef<CalcerDef> Calcer { get; set; }
    }
    public struct ContextFieldDef<T> : IContextFieldDef where T : ContextArg
    {
        public ResourceRef<CalcerDef> Calcer { get; set; }

    }

    public class HasNetIdAttribute : Attribute
    {

    }

    [HasNetId]
    public class ContextArg : BaseResource
    {
    }

    [HasNetId]
    public class HostContextArg : ContextArg
    {

    }
    [HasNetId]
    public class SourceContextArg : ContextArg
    {

    }


    [HasNetId]
    public class TargetContextArg : ContextArg
    {

    }

    [HasNetId]
    public class ProvocatorContextArg : ContextArg
    {

    }

    public class FloatCalcerDef : CalcerDef<float>
    {
        public ResourceRef<CalcerDef> Calcer { get; set; }
    }
    public class ContextArgCalcerDef : CalcerDef<Value>
    {
        public ResourceRef<ContextArg> Arg { get; set; }
    }

    public class CustomContextArgCalcerDef : CalcerDef<Value>
    {
        public ResourceRef<ContextArgTypeDef> Arg { get; set; }
    }

    public class ContextArgTypeDef : SaveableBaseResource
    {

    }
}
