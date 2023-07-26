using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using L10n;
using UnityEngine;

namespace Assets.Src.Aspects.Impl.Traumas.Template
{
    [Localized]
    public class NegativeFactorWarningsDef : BaseResource
    {
        public UnityRef<Sprite> MessageIcon;
        public string MessageColor { get; set; }
        public Dictionary<int, WarningInfo> Warnings { get; set; }
    }

    [Localized]
    public struct WarningInfo
    {
        public LocalizedString Message { get; set; }
        public UnityRef<Sprite> WaringLevelIcon { get; set; }
    }
}