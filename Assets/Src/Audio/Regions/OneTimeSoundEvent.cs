using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Audio.Regions
{
    public class OneTimeSoundEvent : MonoBehaviour
    {
        public AK.Wwise.Event _eventName = default(AK.Wwise.Event);
        public AK.Wwise.State _stateName = default(AK.Wwise.State);
        public bool _useOtherAkGameObj = false;
        private List<AkGameObj> _objectsVisitedThisCollider = new List<AkGameObj>();
        private AkGameObj _akGameObj;

        private void Awake()
        {
            _akGameObj = GetComponent<AkGameObj>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var relay = other.gameObject.transform.root.GetComponentInChildren<AkGameObj>();
            if ((relay != default(AkGameObj)) && (!_objectsVisitedThisCollider.Contains(relay)))
            {
                _objectsVisitedThisCollider.Add(relay);
                if (_eventName != default(AK.Wwise.Event))
                {
                    if (_useOtherAkGameObj)
                    {
                        AkGameObj otherAkGO = other.gameObject.transform.root.GetComponentInChildren<AkGameObj>();
                        
                        if (otherAkGO != default(AkGameObj))
                            _eventName.Post(otherAkGO.gameObject);
                        else
                            Debug.LogWarning($"AkGameObj not found on {other.name} GameObject (called from OneTimeSoundEvent on {gameObject.name})");
                    }
                    else
                    {
                        if (_akGameObj != default(AkGameObj))
                            _eventName.Post(gameObject);
                        else
                            Debug.LogWarning($"Sound event is set but no AkGameObj found on {gameObject.name}");
                    }
                }
                _stateName?.SetValue();
            }
        }
    }
}
