using System.Collections.Generic;
using ResourceSystem.Utils;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace ResourceSystem.Reactions
{
    public interface IReactionHandlerDescriptor
    {
        bool IsDummy { get; }
        string HandlerToString();
    }

    public interface IReactionHandlerSpellDescriptor : IReactionHandlerDescriptor
    {
        SpellDef Spell { get; }
        IEnumerable<(ISpellParameterDef, IVar)> Params { get; }
    }

    public interface IReactionHandlerSpellOnTargetDescriptor : IReactionHandlerDescriptor
    {
        SpellDef Spell { get; }
        IVar<OuterRef> Target { get; }
        IEnumerable<(ISpellParameterDef, IVar)> Params { get; }
    }
    
    public interface IReactionHandlerSpellProlongableDescriptor : IReactionHandlerSpellDescriptor
    {
        float Timeout { get; }
    }
}