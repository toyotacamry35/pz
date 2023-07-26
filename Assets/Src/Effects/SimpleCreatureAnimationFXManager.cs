using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.Effects
{
    class SimpleCreatureAnimationFXManager : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        Dictionary<Object, FxState> _objects = new Dictionary<Object, FxState>();
        public Transform AttachTo;
        public void StartCreatureFX(Object obj)
        {
            FxState state = null;
            if (!_objects.TryGetValue(obj, out state))
            {
                if (AttachTo == null)
                    AttachTo = transform;
                _objects.Add(obj, state = new FxState() { Count = 0, Instance = (GameObject)GameObject.Instantiate(obj, AttachTo) });
            }
            state.Count++;
        }

        public void StopCreatureFX(Object obj)
        {
            FxState state = null;
            if (!_objects.TryGetValue(obj, out state))
            {
                Logger.IfError()?.Message($"Stop creature FX without start {obj.name} {gameObject.name}").Write();
            }
            state.Count--;
            if(state.Count == 0)
            {
                _objects.Remove(obj);
                GameObject.Destroy(state.Instance);
            }
        }

        class FxState
        {
            public GameObject Instance;
            public int Count;
        }
    }

}
