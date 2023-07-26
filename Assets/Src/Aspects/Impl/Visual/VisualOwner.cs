using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using UnityEngine;

namespace Assets.Src.Aspects.Impl.Visual
{
    [DisallowMultipleComponent]
    internal class VisualOwner : MonoBehaviour
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetLogger("VisualOwner");

        [SerializeField]
        internal GameObject VisualPrefab;
        private GameObject _instantiatedGo;

        // --- API: --------------------------------------------------

        internal void ResetVisual(GameObject newPrefab)
        {
            Logger.IfDebug()?.Message/*Error2335*/($"#Dbg: Zz. ResetVisual. `{nameof(newPrefab)}`: {newPrefab}. Previous `{nameof(_instantiatedGo)}`: {_instantiatedGo}.")
                .Write();

            VisualPrefab = newPrefab;
            DestroyVisual();
            if (VisualPrefab != null)
                _instantiatedGo = Instantiate(VisualPrefab, gameObject.transform);
        }

        internal void OnStartClient()
        {
            ResetVisual(VisualPrefab);
        }

        internal void OnStopClient()
        {
            DestroyVisual();
        }

        // --- Unity: --------------------------------------------------

        private void Awake()
        {
            var visualStateOwner = GetComponent<IVisualStateOwner>();
            if (visualStateOwner != null)
                visualStateOwner.VisualChanged += ResetVisual;
        }

        private void OnDestroy()
        {
            var visualStateOwner = GetComponent<IVisualStateOwner>();
            if (visualStateOwner != null)
                visualStateOwner.VisualChanged -= ResetVisual;
        }

        // --- Privates: --------------------------------------------------

        private void DestroyVisual()
        {
            if (_instantiatedGo != null)
            {
                Destroy(_instantiatedGo);
                _instantiatedGo = null;
            }
        }
    }
}
