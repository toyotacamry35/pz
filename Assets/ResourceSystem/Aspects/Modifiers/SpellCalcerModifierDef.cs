using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Reactions;

namespace ResourceSystem.Aspects
{
    public class SpellCalcerModifierDef<T> : SpellModifierDef
    {
        public ResourceRef<ArgDef<T>> Variable;
        public T Value;
    }
}