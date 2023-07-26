using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameplayDebugger
{
    public class DebuggerDrawer : MonoBehaviour
    {
        public Dictionary<Type, ObjectSnapshot> LastSnapshots = new Dictionary<Type, ObjectSnapshot>();
        public Action OnGuiAction;
        public Action OnGizmoAction;

#if UNITY_EDITOR        
        private void OnGUI()
        {
            OnGuiAction?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnGizmoAction?.Invoke();
        }
#endif
    }
}
