using System;
using UnityEngine;

namespace Src.NetworkedMovement
{
    public class IsVisibleByCameraAgent : MonoBehaviour
    {
        public bool Visible { get; set; }
        
        public event Action<IsVisibleByCameraAgent, bool> VisibilityChanged;

        private void OnBecameVisible()
        {
            if (!Visible)
            {
                Visible = true;
                VisibilityChanged?.Invoke(this, true);
            }
        }

        private void OnBecameInvisible()
        {
            if (Visible)
            {
                Visible = false;
                VisibilityChanged?.Invoke(this, false);
            }
        }

        private void OnDisable()
        {
            if (Visible)
            {
                Visible = false;
                VisibilityChanged?.Invoke(this, false);
            }
        }
    }
}