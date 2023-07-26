using System;
using Assets.ColonyShared.SharedCode.Entities;
using UnityEngine;

namespace Assets.Src.Aspects
{
    public interface IAnimatedView
    {
        [Obsolete("Желательно использовать AnimationDoer")] Animator Animator { get; }
        IAnimationDoer AnimationDoer { get; }
    }
}