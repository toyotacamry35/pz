using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    public class TweenSettingsAlpha : TweenSettingsBase
    {
        [Range(0, 1)]
        public float From;

        [Range(0, 1)]
        public float To;
    }
}