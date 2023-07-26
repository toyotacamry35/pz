using ReactivePropsNs;
using UnityEngine;

namespace Assets.Src.Aspects
{
    public interface ISubjectPawn : IViewCreatorDetacher, IEntityPawn
    {
        new IReactiveProperty<ISubjectView> View { get; }
        
        bool Alive { get; }
    }
}