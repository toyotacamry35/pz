using System;
using Assets.ColonyShared.SharedCode.Aspects.Navigation;
using Assets.Src.ResourceSystem;

namespace Uins
{
    public abstract class NavigationIndicatorNotifier : NavigationIndicatorNotifierBase
    {
        public NavIndicatorDefRef NavIndicatorDefRef;

        public Guid MarkerGuid;


        //=== Props ===========================================================

        public override INavigationIndicatorSettings NavigationIndicatorSettings => NavIndicator;

        public override IMapIndicatorSettings MapIndicatorSettings => NavIndicator;

        private NavIndicatorDef _navIndicatorDef;

        public NavIndicatorDef NavIndicatorDef
        {
            get => _navIndicatorDef;
            set
            {
                if (_navIndicatorDef != value)
                {
                    _navIndicatorDef = value;
                    NavIndicator.Description = "";
                }
            }
        }

        private NavIndicator _navIndicator;

        public NavIndicator NavIndicator
        {
            get
            {
                if (_navIndicator == null)
                    _navIndicator = new NavIndicator(_navIndicatorDef);
                return _navIndicator;
            }
        }


        //=== Unity ===========================================================

        protected virtual void Start()
        {
            if (NavigationProvider.Instance == null)
                return;

            if (NavIndicatorDef == null)
            {
                if ((NavIndicatorDefRef?.Target).AssertIfNull(nameof(NavIndicatorDefRef)))
                    return;

                NavIndicatorDef = NavIndicatorDefRef.Target;
            }

            IsDisplayable = GetIsDisplayable(); //Если true, то пошлет нотификацию
        }


        //=== Protected =======================================================

        protected virtual bool GetIsDisplayable()
        {
            return true;
        }
    }
}