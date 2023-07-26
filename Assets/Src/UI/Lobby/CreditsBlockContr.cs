using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public abstract class CreditsBlockContr<T> : BindingController<T> where T : CreditsBlock
    {
        [SerializeField, UsedImplicitly]
        private TextWithFormatModContr _titleContr; //м.б. null


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            if (_titleContr != null)
            {
                var textWithFmStream = Vmodel.Func(D, def => def != null ? (def.Title, def.TitleFormatMod) : ("", default));
                textWithFmStream.Bind(D, _titleContr.Vmodel);
            }
        }
    }
}