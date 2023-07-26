using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Audio
{
    public class AmbientSoundController : MonoBehaviour
    {
        private List<AmbientSoundEventDescriptor> _ambients = new List<AmbientSoundEventDescriptor>();
        private string _lastEvent;

        public void AddAmbientEvent(string eventName, int priority, float validTime)
        {
            AmbientSoundEventDescriptor descr = new AmbientSoundEventDescriptor() { eventName = eventName, priority = priority, removedByTimer = true };
            _ambients.Add(descr);
            this.StartInstrumentedCoroutine(RemoveAfterSpecifiedTime(validTime, descr));
            PostAmbientMusic();
        }

        public bool RemoveAmbientEvent(string eventName, int priority)
        {
            var ambientSoundEventDescriptor = new AmbientSoundEventDescriptor() { eventName = eventName, priority = priority, removedByTimer = false };
            foreach (var descr in _ambients)
            {
                if (descr.HasSameValues(ambientSoundEventDescriptor))
                {
                    _ambients.Remove(descr);
                    PostAmbientMusic();
                    return true;
                }
            }
            return false;
        }

        private void PostAmbientMusic()
        {
            int maxPriority = 0;
            AmbientSoundEventDescriptor descriptorWithMaxPriority = default(AmbientSoundEventDescriptor);
            foreach(var descr in _ambients)
            {
                if (descr.priority > maxPriority)
                {
                    maxPriority = descr.priority;
                    descriptorWithMaxPriority = descr;
                }
            }
            if (descriptorWithMaxPriority == default(AmbientSoundEventDescriptor))
            {
                AkSoundEngine.SetState("music_state", "exploration");
                _lastEvent = default(string);
            }
            else if (descriptorWithMaxPriority.eventName != _lastEvent)
            {
                _lastEvent = descriptorWithMaxPriority.eventName;
                switch (descriptorWithMaxPriority.eventName)
                {
                    case "CombatEvent":
                        AkSoundEngine.SetState("music_state", "combat");
                        break;
                    case "ExplorationEvent":
                    default:
                        AkSoundEngine.SetState("music_state", "exploration");
                        break;
                }
            }

        }

        IEnumerator RemoveAfterSpecifiedTime(float time, AmbientSoundEventDescriptor descr)
        {
            yield return new WaitForSeconds(time);
            if (_ambients.Contains(descr))
            {
                _ambients.Remove(descr);
                PostAmbientMusic();
            }

        }
    }

    internal class AmbientSoundEventDescriptor
    {
        public string eventName;
        public bool removedByTimer;
        public int priority;

        public bool HasSameValues(AmbientSoundEventDescriptor other)
        {
            if (eventName == other.eventName            &&
                removedByTimer == other.removedByTimer  &&
                priority == other.priority)
                return true;
            else
                return false;
        }
    }
}
