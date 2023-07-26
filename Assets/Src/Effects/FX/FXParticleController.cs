using System;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class FXParticleController : MonoBehaviour, IFXElementController
    {
        [Serializable]
        private struct ParticleInfo
        {
            public ParticleSystem system;
            public bool isEnded;
            public bool isLooped;
        }

        [SerializeField]
        private ParticleInfo[] _particles;

        private bool _canLoop;
        private int _index;

        public event Action<IFXElementController> Completed;

        public bool Init()
        {
            return Init(false);
        }
        
        public bool Init(bool force)
        {
            if (_particles != null && _particles.Length != 0 && !force)
                return true;
            
            var particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            if (particles.Length == 0)
            {
                OnCompleted();
                return false;
            }

            _particles = new ParticleInfo[particles.Length];
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].system = particles[i];
                _particles[i].isEnded = false;
                _particles[i].isLooped = _particles[i].system.main.loop;
            }

            return true;
        }

        public void Tick()
        {
            _index = 0;
            for (int i = 0; i < _particles.Length; i++)
            {
                if (_particles[i].isEnded)
                    _index++;

                if (!_particles[i].system.IsAlive())
                {
                    _particles[i].isEnded = true;
                    _index++;
                }
            }

            if (_index >= _particles.Length)
                OnCompleted();
        }

        public void Hide()
        {
            for (var i = 0; i < _particles.Length; i++)
            {
                var main = _particles[i].system.main;
                main.loop = false;
            }
        }

        public void HideImmediately()
        {
            gameObject.SetActive(false);
        }

        public void Show(FXElementParams fxElementParams)
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                _particles[i].isEnded = false;

                var main = _particles[i].system.main;
                main.loop = fxElementParams != null ? fxElementParams.CanLoop && _particles[i].isLooped : _particles[i].isLooped;
            }

            //Какая-то очень странная логика тут происходит
            if (fxElementParams != null && fxElementParams.CheckRotationParam)
            {
                if (_particles.Length >= 1)
                {
                    var main = _particles[0].system.main;
                    // main.startRotation = Mathf.Deg2Rad * param.value;
                }
            }
        }
        
        private void OnValidate()
        {
            Init(true);
        }

        protected virtual void OnCompleted()
        {
            Completed?.Invoke(this);
        }
    }
}