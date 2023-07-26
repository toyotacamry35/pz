using L10n;
using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins.Settings
{
    [Binding]
    public class ValAsLocalizedStringRepresenterCtrl : ValRepresenterCtrlBase
    {
        [Binding]
        public LocalizedString BindedLocalizedString { get; protected set; }

        public void Init(IStream<LocalizedString> stream)
        {
            Bind(stream, () => BindedLocalizedString);
        }
    }
}