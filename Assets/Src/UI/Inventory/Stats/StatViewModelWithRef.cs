using Assets.Src.ResourceSystem.L10n;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class StatViewModelWithRef : StatViewModel
    {
        public StatResourceRef StatResourceRef;


        //=== Unity ===========================================================

        private void Start()
        {
            if (!StatResourceRef.Target.AssertIfNull(nameof(StatResourceRef), gameObject))
                SetStatResource(StatResourceRef.Target);
        }
    }
}