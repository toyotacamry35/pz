using ReactivePropsNs;
using UnityWeld.Binding;

namespace Uins.Settings
{
    [Binding]
    public class ValAsStringRepresenterCtrl : ValRepresenterCtrlBase
    {
        [Binding]
        public string BindedString { get; protected set; }

        public void Init(IStream<string> stream)
        {
            Bind(stream, () => BindedString);
        }
    }
}
