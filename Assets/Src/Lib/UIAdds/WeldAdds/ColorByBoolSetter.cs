using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ColorByBoolSetter : BaseByBoolSetter
    {
        [SerializeField, UsedImplicitly]
        private Graphic Target;

        [SerializeField, UsedImplicitly]
        private Color UnsetColor = Color.white;

        [SerializeField, UsedImplicitly]
        private Color SetColor = Color.white;

        protected override void Init()
        {
            Target.AssertIfNull(nameof(Target), gameObject);
        }

        protected override void SyncIfWoken()
        {
            Target.color = Flag ? SetColor : UnsetColor;
        }
    }
}