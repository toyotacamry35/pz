using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class SpriteByBoolSetter : BaseByBoolSetter
    {
        [SerializeField, UsedImplicitly]
        private Image Target;

        [SerializeField, UsedImplicitly]
        private Sprite UnsetSprite;

        [SerializeField, UsedImplicitly]
        private Sprite SetSprite;

        protected override void Init()
        {
            Target.AssertIfNull(nameof(Target), gameObject);
            Target.DisableSpriteOptimizations();
        }

        protected override void SyncIfWoken()
        {
            Target.overrideSprite = Flag ? SetSprite : UnsetSprite;
        }
    }
}