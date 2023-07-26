using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TextContr : BindingController<string>
    {
        //=== Props ===========================================================

        [Binding]
        public string Text { get; protected set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            Bind(Vmodel, () => Text);
        }
    }
}