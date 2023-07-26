using Assets.Src.Aspects;
using L10n;
using ReactivePropsNs;
using Uins.Cursor;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class KnockdownContr : BindingController<TmpKnockdownInterface>
    {
        private CursorControl.Token _cursorToken;

        [Binding]
        public LocalizedString ButtonTextLs { get; private set; }

        private static readonly PropertyBinder<KnockdownContr, LocalizedString> ButtonTextLsBinder
            = PropertyBinder<KnockdownContr>.Create(_ => _.ButtonTextLs);

        [Binding]
        public bool IsVisible { get; private set; }

        private static readonly PropertyBinder<KnockdownContr, bool> IsVisibleBinder
            = PropertyBinder<KnockdownContr>.Create(_ => _.IsVisible);

        [Binding]
        public bool IsInteractable { get; private set; }

        private static readonly PropertyBinder<KnockdownContr, bool> IsInteractableBinder
            = PropertyBinder<KnockdownContr>.Create(_ => _.IsInteractable);


        //=== Unity ===========================================================

        private void Awake()
        {
            Bind(Vmodel.SubStream(D, vm => vm.ButtonTextRp), ButtonTextLsBinder);
            var isVisibleStream = Vmodel.SubStream(D, vm => vm.IsButtonVisibleStream);
            Bind(isVisibleStream, IsVisibleBinder);
            Bind(Vmodel.SubStream(D, vm => vm.IsButtonInteractiveStream), IsInteractableBinder);

            isVisibleStream.Action(
                D,
                isVisible =>
                {
                    if (isVisible)
                    {
                        if (_cursorToken == null)
                            _cursorToken = CursorControl.AddCursorFreeRequest(this);
                    }
                    else
                    {
                        if (_cursorToken != null)
                        {
                            _cursorToken.Dispose();
                            _cursorToken = null;
                        }
                    }
                });
        }


        //=== Public ==========================================================

        public void OnClick()
        {
            Vmodel.Value?.OnButtonClick();
        }
    }
}