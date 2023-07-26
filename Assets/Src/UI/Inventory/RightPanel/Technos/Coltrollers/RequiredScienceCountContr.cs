using ReactivePropsNs;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class RequiredScienceCountContr : RequiredCountContr<ScienceDef>
    {
        //=== Props ==============================================================

        [Binding]
        public Sprite ScienceMiniIcon { get; protected set; }

        [Binding]
        public Sprite ScienceInactiveMiniIcon { get; protected set; }


        //=== Public ==========================================================

        protected override void Awake()
        {
            base.Awake();
            Bind(Vmodel.Func(D, vm => vm.Resource?.MiniSprite?.Target), () => ScienceMiniIcon);
            Bind(Vmodel.Func(D, vm => vm.Resource?.InactiveMiniSprite?.Target), () => ScienceInactiveMiniIcon);
        }
    }
}