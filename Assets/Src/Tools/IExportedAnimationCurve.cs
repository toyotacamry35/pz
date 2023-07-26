using System.Collections.Generic;
using UnityEngine;

namespace Src.Tools
{
    public interface IExportedAnimationCurve
    {
        bool AutoUpdate { get; }
        IEnumerable<AnimationClip> Clips { get; }
    }
}