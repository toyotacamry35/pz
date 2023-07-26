using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using UnityEngine;

namespace Src.Tools
{
    [CreateAssetMenu(menuName = "Curves/Exported Animation Curve")]
    public sealed class ExportedAnimationCurve : Curve, IExportedAnimationCurve
    {
        [SerializeField] public string CurveName;
        [SerializeField] public AnimationClip Clip;
        [SerializeField] public bool AutoUpdate;
        [SerializeField] public bool Optimization;
        [SerializeField] public float OptimizationTolerance = 0.01f;
        [SerializeField] public float Scale = 1;

        bool IExportedAnimationCurve.AutoUpdate => AutoUpdate;
        IEnumerable<AnimationClip> IExportedAnimationCurve.Clips { get { yield return Clip; } }
    }
}