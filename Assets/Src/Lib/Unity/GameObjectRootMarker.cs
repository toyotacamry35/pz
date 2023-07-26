using UnityEngine;

namespace Assets.Src.Lib.Unity
{
    // Mark root g.o. by this component to let `RootFinderExtension.GetRoot` find correct g.o.
    // (e.g. used in mineable interactives to get correct key-g.o. to register stats).
    public class GameObjectRootMarker : MonoBehaviour
    {
    }
}
