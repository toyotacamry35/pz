using JetBrains.Annotations;

namespace Uins
{
    [UsedImplicitly]
    public class NpcBadgePoint : InteractiveBadgePoint
    {
        protected override void Start()
        {
            base.Start();

            if (VisualMarker == null)
                return;

            if (!VisualMarker.ShowMarker)
                IsAllowedToShowRp.Value = false;
        }
    }
}