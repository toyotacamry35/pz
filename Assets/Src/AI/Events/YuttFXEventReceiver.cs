using Assets.Src.Character.Events;
using Assets.Src.ResourceSystem;
using UnityEngine;

namespace Assets.Src.AI.Events
{
    public class YuttFXEventReceiver : MonoBehaviour
    {
        public JdbMetadata evtType;
        public void YuttDrinkFX()
        {
            var rootObj = transform.root.gameObject;
            var evp = rootObj.GetComponent<VisualEventProxy>();
            if (evp)
            {
                var repo = evp.Repository;
                var entity = evp.Entity;
                var evt = new VisualEvent
                {
                    eventType = evtType.Get<FXEventType>(),
                    casterEntityRef = entity,
                    casterGameObject = gameObject,
                    casterRepository = repo
                };
                evp.PostEvent(evt);
            }
        }
    }
}
