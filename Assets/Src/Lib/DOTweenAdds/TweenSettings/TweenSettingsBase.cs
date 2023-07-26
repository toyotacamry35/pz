using DG.Tweening;
using UnityEngine;

namespace Assets.Src.Lib.DOTweenAdds
{
    /// <summary>
    /// Контейнер типовых настроек твинера, чтобы не писать их отдельными характеристиками в скриптах
    /// </summary>
    public abstract class TweenSettingsBase : MonoBehaviour
    {
        public string Name;
        public float Delay;
        public float Duration = 1;
        public Ease Ease = Ease.Linear;
        public int Loops = 1;
        public LoopType LoopType = LoopType.Restart;
    }
}