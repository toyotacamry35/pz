using L10n;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins
{
    /// <summary>
    /// Вкладка таба
    /// </summary>
    [Binding]
    public class TechnoTabContr : BindingController<TechnoTabVmodel>
    {
        //=== Props ===========================================================

        [Binding]
        public bool IsSelected { get; set; }

        [Binding]
        public LocalizedString Title { get; protected set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            Vmodel.Action(D, vm => name = $"Tab_{vm?.TechnologyTabDef?.____GetDebugShortName()}");
            var isSelectedStream = Vmodel.SubStream(D, vm => vm.IsSelectedRp, false);
            Bind(isSelectedStream, () => IsSelected);

            var titleStream = Vmodel.Func(D, vm => vm?.TechnologyTabDef.TitleLs ?? LsExtensions.Empty);
            Bind(titleStream, () => Title);
        }


        //=== Public ==========================================================

        public void OnClick()
        {
            Vmodel.Value?.TechnoAtlasVmodel.SetSelectedTabVmodel(Vmodel.Value);
        }
    }
}