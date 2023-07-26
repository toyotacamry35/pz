using UnityEngine;
using System.Collections;
using System.Linq;

namespace Assets.Src.SpawnSystem
{
    public class VisibilityHack : EntityGameObjectComponent
    {
        protected override void GotServer()
        {
            this.StartInstrumentedCoroutine(Coroutine());
        }
        IEnumerator Coroutine()
        {
            int iterations = 100;
            SphereCollider sp = null;
            while (sp == null)
            {
                sp = GetComponentsInChildren<SphereCollider>().Single(x => x.gameObject.layer == LayerMask.NameToLayer("ReplicationRangeCheck"));
                yield return null;
                iterations--;
                if(iterations == 0)
                    yield break;
            }
            sp.enabled = false;
            yield return null;
            sp.enabled = true;
        }
    }
}
