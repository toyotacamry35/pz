using UnityEngine;

namespace Assets
{
    public class FXActivator : MonoBehaviour
    {
        public GameObject[] FX;
		
        public void Play(int a)
        {
            SetState(a, true);
        }

        public void Stop(int a)
        {
            SetState(a, false);
        }

        private void SetState(int a, bool enabled)
        {
            if (FX.Length >= a)
            {
                var go = FX[a];
                if (go == null)
                    return;
                go.SetActive(enabled);
                ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>();
				foreach(ParticleSystem thisPs in ps)
				{
				if (thisPs != null)
				{
					if (enabled)
						thisPs.Play();
					else
						thisPs.Stop();
				}
				}
				var an = go.GetComponent<Animation>();
				if (an != null)
				{
				if (enabled)
				{
                    an.Play(an.name);
					
				}
                else
                    an.Stop();
				}
				
				
            }
            else
            {
                Debug.LogError("Компонента с таким индексом нет в списке");
            }
			

        }
    }
}
