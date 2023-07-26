using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ColorByBoolMultiSetter : BaseByBoolSetter
    {
        [SerializeField, UsedImplicitly]
        private Graphic[] Targets;

        [SerializeField, UsedImplicitly]
        private Color UnsetColor = Color.white;

        [SerializeField, UsedImplicitly]
        private Color SetColor = Color.white;

        protected override void Init()
        {
            Targets.AssertIfNull(nameof(Targets), gameObject);
        }

        protected override void SyncIfWoken()
        {
            foreach (var target in Targets)
                if (target != null)
                    target.color = Flag ? SetColor : UnsetColor;
        }
    }
}