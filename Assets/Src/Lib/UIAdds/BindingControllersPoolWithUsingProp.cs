using UnityEngine;

namespace Uins
{
    public class BindingControllersPoolWithUsingProp<T> : BindingControllersPoolWithGoActivation<T>
    {
        //=== Ctor ============================================================

        public BindingControllersPoolWithUsingProp(Transform rootTransform, BindingControllerWithUsingProp<T> prefab)
            : base(rootTransform, prefab)
        {
        }

        //=== Protected =======================================================

        protected override BindingController<T> GetController()
        {
            var controller = CreateOrReuseController();
            if (controller != null)
            {
                ((BindingControllerWithUsingProp<T>) controller).IsInUsingRp.Value = true;
            }
            return controller;
        }


        protected override void ReleaseController(BindingController<T> controller)
        {
            ReleaseControllerBase(controller);
            if (controller != null)
            {
                ((BindingControllerWithUsingProp<T>) controller).IsInUsingRp.Value = false;
            }
        }
    }
}