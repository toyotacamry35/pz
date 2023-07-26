using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    [Serializable]
    public class PointOfInterestNotifierSettings : INavigationIndicatorSettings, IMapIndicatorSettings
    {
        [SerializeField, UsedImplicitly]
        private Sprite _icon;

        [SerializeField, UsedImplicitly]
        private Color _iconColor = Color.green;

        [SerializeField, UsedImplicitly]
        private bool _isShowDist = true;

        [SerializeField, UsedImplicitly]
        private Sprite _mapIcon;

        [SerializeField, UsedImplicitly]
        private int _questZoneDiameter = -1;

        public Sprite Icon => _icon;
        public Color IconColor => _iconColor;
        public float FovToDisplay => 360;
        public float DistanceToDisplay => float.MaxValue;

        public Sprite MapIcon => _mapIcon;
        public Color PointColor => _iconColor;
        public bool IsSelectable => false;
        public bool IsShowDist => _isShowDist;
        public int QuestZoneDiameter => _questZoneDiameter;
        public string Description => null;
        public bool IsPlayer => false;
    }
}