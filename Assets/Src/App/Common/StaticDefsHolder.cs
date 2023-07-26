using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.App.Common
{
    public class StaticDefsHolder : MonoBehaviour
    {
        [SerializeField] public JdbMetadata DestrucControllersStaticConstsJdb;

        private void Awake()
        {
            Debug.Assert(DestrucControllersStaticConstsJdb != null);
        }
    }
}
