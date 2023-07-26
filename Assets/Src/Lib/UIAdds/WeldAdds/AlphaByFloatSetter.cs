using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class AlphaByFloatSetter : BaseByFloatSetter
    {
        [SerializeField, UsedImplicitly]
        private Graphic Target;
        
        protected override void Init()
        {
            Target.AssertIfNull(nameof(Target), gameObject);
        }

        protected override void SyncIfWoken()
        {
            Target.SetAlpha(Amount);
        }
    }
}