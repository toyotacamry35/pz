using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace Assets.Src.Hints
{
    [Localized]
    public class HintsDef : BaseResource
    {
        public Hint[] Hints { get; set; }
    }

    [Localized]
    public struct Hint
    {
        public LocalizedString TextLs { get; set; }

        public float Time { get; set; }
    }
}
