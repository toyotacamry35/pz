using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Uins
{
    public class BindingControllersPoolWithGoActivation<T> : BindingControllersPool<T>
    {
        protected HashSet<BindingController<T>> ReleasedControllers = new HashSet<BindingController<T>>();


        //=== Ctor ============================================================

        public BindingControllersPoolWithGoActivation(Transform rootTransform, BindingController<T> prefab)
            : base(rootTransform, prefab)
        {
        }

        //=== Protected =======================================================

        protected override BindingController<T> GetController()
        {
            var controller = CreateOrReuseController();
            controller.gameObject.SetActive(true);
            return controller;
        }

        protected BindingController<T> CreateOrReuseController()
        {
            BindingController<T> controller;
            if (ReleasedControllers.Count > 0)
            {
                controller = ReleasedControllers.First();
                ReleasedControllers.Remove(controller);
            }
            else
            {
                controller = Object.Instantiate(prefab, rootTransform);
                controller.name = $"{prefab.name}_{++controllersCount}";
            }

            return controller;
        }

        protected override void ReleaseController(BindingController<T> controller)
        {
            ReleaseControllerBase(controller);
            if (controller != null) controller.gameObject.SetActive(false);
        }

        protected void ReleaseControllerBase(BindingController<T> controller)
        {
            if (controller.AssertIfNull(nameof(controller)))
                return;

            if (ReleasedControllers.Contains(controller))
            {
                UI.Logger.IfError()?.Message($"{GetType().NiceName()} '{rootTransform.FullName()}' {nameof(ReleasedControllers)} already contains '{controller}'").Write();
                return;
            }

            ReleasedControllers.Add(controller);
            controller.transform.SetAsLastSibling();
        }
    }
}