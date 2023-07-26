using System;
using System.Collections;
using Assets.Src.Camera.Effects.DeferredProjectiveDecals;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class DecalFadeOut : MonoBehaviour, IFXElementController
    {
        private struct DecalInfo
        {
            public float startValue;
            public float transition;
            public bool end;
            public FastDecal decal;
        }

        [SerializeField]
        private float fadeOutTime;

        [SerializeField]
        private float endValue = 0;

        [SerializeField]
        private bool autostart;

        [SerializeField]
        private DecalInfo[] _decals;

        public event Action<IFXElementController> Completed;

        public bool IsComplete { get; set; }

        public bool Autostart => autostart;

        public bool Init()
        {
            return Init(false);
        }

        public bool Init(bool force)
        {
            if (_decals != null && _decals.Length !=0 && !force)
                return true;

            var decals = GetComponentsInChildren<FastDecal>();
            if (decals.Length <= 0)
            {
                OnCompleted();
                return false;
            }

            _decals = new DecalInfo[decals.Length];
            for (int i = 0; i < _decals.Length; i++)
            {
                _decals[i].decal = decals[i];
                _decals[i].startValue = decals[i].Fade;
                _decals[i].transition = 0;
                _decals[i].end = false;
            }

            return true;
        }

        private void OnValidate()
        {
            Init(true);
        }

        void OnEnable()
        {
            if (fadeOutTime > 0)
            {
                if (autostart)
                    this.StartInstrumentedCoroutine(FadeOut());
            }
            else
            {
                OnCompleted();
            }
        }

        private void OnCompleted()
        {
            IsComplete = true;
            Completed?.Invoke(this);
        }

        public void Tick()
        {
            var ended = true;
            for (int i = 0; i < _decals.Length; i++)
            {
                if (_decals[i].end)
                    continue;

                _decals[i].transition += Time.deltaTime / fadeOutTime;
                _decals[i].decal.Fade = Mathf.Lerp(_decals[i].startValue, this.endValue, _decals[i].transition);
                if (_decals[i].decal.Fade <= float.Epsilon)
                    _decals[i].end = true;
                else
                    ended = false;
            }

            if (ended)
                OnCompleted();
        }

        public void HideImmediately()
        {
            Hide();
        }

        public void Show(FXElementParams fxElementParams)
        {
            gameObject.SetActive(true);

            for (int i = 0; i < _decals.Length; i++)
            {
                _decals[i].transition = 0;
                _decals[i].end = false;
            }
        }

        public void Hide()
        {
            if (autostart)
                StopAllCoroutines();
            gameObject.SetActive(false);
        }

        private IEnumerator FadeOut()
        {
            while (!IsComplete)
            {
                Tick();
                yield return null;
            }
        }
    }
}