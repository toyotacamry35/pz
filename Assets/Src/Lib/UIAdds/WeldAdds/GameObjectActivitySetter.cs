using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using UnityEngine;

namespace WeldAdds
{
    public class GameObjectActivitySetter : MonoBehaviour
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public GameObject[] Targets;
        private bool _isAfterAwake;


        //=== Props ===============================================================

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (!enabled)
                    return;

                if (_isVisible != value)
                {
                    _isVisible = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            if (Targets.IsNullOrEmptyOrHasNullElements(nameof(Targets)))
            {
                Logger.IfError()?.Message("Targets.IsNullOrEmptyOrHasNullElements").UnityObj(gameObject).Write();
                enabled = false;
                return;
            }

            _isAfterAwake = true;
            SyncIfWoken();
        }


        //=== Private =============================================================

        private void SyncIfWoken()
        {
            for (int i = 0, len = Targets.Length; i < len; i++)
            {
                if (Targets[i].activeSelf != IsVisible)
                    Targets[i].SetActive(IsVisible);
            }
        }
    }
}