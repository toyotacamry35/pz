using Assets.ColonyShared.SharedCode.Aspects.Navigation;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Uins
{
    public class NavIndicator : INavigationIndicatorSettings, IMapIndicatorSettings
    {
        private readonly Color _transparentColor = new Color(0, 0, 0, 0);

        public NavIndicator(NavIndicatorDef navIndicatorDef, string description = null)
        {
            if (navIndicatorDef.AssertIfNull(nameof(navIndicatorDef)))
                return;

            Description = description;
            Icon = navIndicatorDef.Icon?.Target;
            MapIcon = navIndicatorDef.MapIcon?.Target;
            FovToDisplay = navIndicatorDef.FovToDisplay;
            DistanceToDisplay = navIndicatorDef.DistanceToDisplay;
            IsSelectable = navIndicatorDef.IsSelectable;
            IsShowDist = navIndicatorDef.IsShowDist;
            QuestZoneDiameter = navIndicatorDef.QuestZoneDiameter;
            IsPlayer = navIndicatorDef.IsPlayer;
            Color iconColor;
            if (ColorUtility.TryParseHtmlString(navIndicatorDef.IconColorHexCode, out iconColor))
            {
                IconColor = iconColor;
            }
            else
            {
                UI.Logger.IfError()?.Message($"Unable to parse color from '{navIndicatorDef.IconColorHexCode}'").Write();
                IconColor = _transparentColor;
            }
        }

        public Sprite Icon { get; }
        public Color IconColor { get; }
        public float FovToDisplay { get; }
        public float DistanceToDisplay { get; }
        public bool IsSelectable { get; }
        public bool IsShowDist { get; }
        public Sprite MapIcon { get; }
        public int QuestZoneDiameter { get; }
        public string Description { get; set; }
        public bool IsPlayer { get; }

        Sprite IMapIndicatorSettings.MapIcon => MapIcon;
        Color IMapIndicatorSettings.PointColor => IconColor;
        bool IMapIndicatorSettings.IsSelectable => IsSelectable;
        string IMapIndicatorSettings.Description => Description;
        bool IMapIndicatorSettings.IsPlayer => IsPlayer;
    }
}