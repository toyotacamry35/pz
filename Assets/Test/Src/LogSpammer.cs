using Core.Cheats;
using UnityEngine;

namespace Assets.Test.Src
{
    public class LogSpammer : MonoBehaviour
    {
        private static int _qty = 100;
        public void Update()
        {
            for(int i=0; i< _qty; ++i)
                Debug.LogError($"Log message {i}");
        }

        [Cheat]
        public static void Set_Spammer_Qty(int qty)
        {
            _qty = qty;
        }
    }
}
