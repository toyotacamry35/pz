using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs.Touchables;

namespace Uins
{
    public class PremiumRefreshStatusToucher : ToucherUnity<IWorldCharacterClientFull>
    {
        public override void OnAdd(IWorldCharacterClientFull entity)
        {
            //var premiumStatusClientFull = entity.PremiumStatus;
            //premiumStatusClientFull.RefreshStatus();
            UI.CallerLog("RefreshStatus() ok"); //DEBUG
            Dispose();
        }
    }
}