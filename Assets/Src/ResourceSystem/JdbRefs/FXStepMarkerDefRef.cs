using Assets.ResourceSystem.Aspects.Effects;
using System;
using UnityEngine;

namespace Assets.Src.ResourceSystem.JdbRefs
{
    [Serializable]
    public class FXStepMarkerDefRef : JdbRef<FXStepMarkerDef>
    {
        public ScriptableObject Metadata { get => _metadata; set => _metadata = (JdbMetadata)value; }
    }
}
