using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class HasDisposablesMonoBehaviour : MonoBehaviour
    {
        protected DisposableComposite D = new DisposableComposite();

        protected virtual void OnDestroy()
        {
            D.Dispose();
        }
    }
}