using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class MiningIndicator : InteractionIndicator
    {
        protected override void Visual_AppearingStart()
        {
            base.Visual_AppearingStart();
            ProgressSetup();
        }

        protected override void Visual_ChangingStart()
        {
            base.Visual_ChangingStart();
            ProgressSetup(true);
        }
    }
}