using Assets.Src.Camera;

namespace Assets.ASkyLighting.SSShadows
{
    public static class SSShadowsControl
    {
        private static bool _SSShadowsEnabled = true;
        public static bool SSShadowsEnabled
        {
            get
            {
                return _SSShadowsEnabled;
            }
            set
            {
                _SSShadowsEnabled = value;
                var sssComponent = GameCamera.Camera?.GetComponent<SSShadows>();
                if (sssComponent)
                    sssComponent.enabled = _SSShadowsEnabled;
            }
        }
    }
}
