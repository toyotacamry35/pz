using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using System.Collections.Generic;
using L10n;
using System;

namespace Assets.Src.Aspects.Impl.Traumas.Template
{
    [Localized]
    public class TraumasDef : SaveableBaseResource
    {
        public float MeanTimeForTraumasToHappen { get; set; }
        public bool DoNotWork { get; set; } = false;
        public Dictionary<string, ResourceRef<TraumaDef>> TraumaGivers { get; set; } = new Dictionary<string, ResourceRef<TraumaDef>>();
    }
}
