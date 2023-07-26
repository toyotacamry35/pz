using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class CreditsLineContr : BindingController<CreditsLine>
    {
        [SerializeField, UsedImplicitly]
        private TextWithFormatModContr _nameContr;


        //=== Props ===========================================================

        [Binding]
        public string Description { get; private set; }

        [Binding]
        public bool HasDescription { get; private set; }


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            if (_nameContr.AssertIfNull(nameof(_nameContr)))
                return;

            var textWithFmStream = Vmodel.Func(D, line => (line.Name, line.FormatMod));
            textWithFmStream.Bind(D, _nameContr.Vmodel);
            Bind(Vmodel.Func(D, line => line.Description), () => Description);
            Bind(Vmodel.Func(D, line => !string.IsNullOrEmpty(line.Description)), () => HasDescription);
        }
    }
}