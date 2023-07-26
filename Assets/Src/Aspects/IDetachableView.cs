using ResourceSystem.Aspects.Misc;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace Assets.Src.Aspects
{
    public interface IDetachableView
    {
        void Attach(OuterRef entityRef, IEntitiesRepository entityRepo, VisualDollDef dollDef);
        
        void Detach(OuterRef entityRef, IEntitiesRepository entityRepo);
    }
}