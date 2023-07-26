using System.Collections;
using System.Collections.Generic;
using Assets.Src.Audio;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class FXElement : MonoBehaviour
    {
        [SerializeField]
        bool _returnToStack = true;

        [SerializeField]
        GameObject _defaultPrefab;
        // bool _wasInit = false;


        void OnDestroy()
        {
            if (_decalFadeOut != null)
            {
                for (int i = 0; i < _decalFadeOut.Length; i++)
                {
                    //_decalFadeOut[i].EndEvent -= EndDecal;
                }
            }
        }

        /*public void Init() //префаб эффекта нужно истанцировать
        {
            if (!_wasInit)
            {
                _wasInit = true;

                if (_defaultPrefab != null)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(_defaultPrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
                    go.transform.localScale = Vector3.one;
                }
                Init(null);
            }
        }*/

        public void Init(GameObject prefab, bool canLoop = false)
        {
            _prefab = prefab;
            _maxTime = 1f;
            _canLoop = canLoop;

            //particle
            _particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            _isParticleEnd = new bool[_particle.Length];
            for (int i = 0; i < _particle.Length; i++)
            {
                _isParticleEnd[i] = false;
                if (!_canLoop && _particle[i].main.loop)
                {
                    Debug.LogWarning(
                        string.Format("Loop particle in {0}.prefab in {1} obj", prefab != null ? prefab.name : _defaultPrefab != null ? _defaultPrefab.name : gameObject.name, _particle[i].name));
                    var main = _particle[i].main;
                    main.loop = false;
                }
            }

            //decal

            //добавляем DecalFadeOut на каждый Decal
            /*Decal[] decal = gameObject.GetComponentsInChildren<Decal>();
            _decalFadeOut = new DecalFadeOut[decal.Length];
            for (int i = 0; i < decal.Length; i++)
            {
                _decalFadeOut[i] = decal[i].gameObject.GetComponent<DecalFadeOut>();
                if (_decalFadeOut[i] == null)
                {
                    _decalFadeOut[i] = decal[i].gameObject.AddComponent<DecalFadeOut>();
                    _decalFadeOut[i].fadeOutTime = _maxTime;
                }
                _decalFadeOut[i].EndEvent += EndDecal;
            }*/

            //используем только уже добавленные DecalFadeOut
            _decalFadeOut = gameObject.GetComponentsInChildren<DecalFadeOut>(true);
            for (int i = 0; i < _decalFadeOut.Length; i++)
            {
                /*_decalFadeOut[i].EndEvent += EndDecal;
                if (_decalFadeOut[i].fadeOutTime > _maxTime)
                {
                    _maxTime = _decalFadeOut[i].fadeOutTime;
                }*/
            }

            _maxTime++;
        }

        public void Show(Vector3 position, Quaternion rotation, bool canLoop = false, FXParamsOnObj.FXParamsValue[] pair = null)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            Show(canLoop, pair);
        }

        public void Show(bool canLoop = false, FXParamsOnObj.FXParamsValue[] pair = null)
        {
            gameObject.SetActive(true);

            _canLoop = canLoop;
            _index = 0;

            if (_isParticleEnd != null)
            {
                for (int i = 0; i < _isParticleEnd.Length; i++)
                {
                    _isParticleEnd[i] = false;
                }
            }

            if (_canLoop)
            {
                foreach (var particle in _needReturnLoop)
                {
                    var main = particle.main;
                    main.loop = true;
                }

                _needReturnLoop.Clear();
            }

            if (pair != null)
            {
                foreach (var param in pair)
                {
                    if (param.param.RotationParam == true)
                    {
                        if (_particle.Length >= 1)
                        {
                            var main = _particle[0].main;
                            main.startRotation = Mathf.Deg2Rad * param.value;
                        }
                    }
                }
            }

            this.StartInstrumentedCoroutine(SoundCoroutine());
        }

        public void HideLoop() //остановить loop
        {
            _canLoop = false;
            _maxTime = 0;
            _index = _decalFadeOut.Length;

            if (_particle != null)
            {
                foreach (var particle in _particle)
                {
                    if (particle.main.loop)
                    {
                        var main = particle.main;
                        main.loop = false;
                        _needReturnLoop.Add(particle);
                    }
                }
            }
        }

        public void HideImmediatly() //возвращение в стек
        {
            gameObject.SetActive(false);
            if (_returnToStack)
            {
                FXQueue.Instance.Add(_prefab, this);
            }
        }

        IEnumerator SoundCoroutine()
        {
            yield return null;
            AkEvent soundAkEvent = gameObject.GetComponent<AkEvent>();
            if (soundAkEvent != null)
            {
                AkSoundEngine.PostEvent(soundAkEvent.data.Id, soundAkEvent.gameObject);
            }
            else
            {
                AkEventRelay akRelay = gameObject.GetComponent<AkEventRelay>();
                if (akRelay != null)
                {
                    Transform parent = gameObject.transform.parent;
                    while (parent != null)
                    {
                        AkGameObj soundAkObj = parent.gameObject.GetComponent<AkGameObj>();
                        if (soundAkObj != null)
                        {
                            AkSoundEngine.PostEvent(akRelay.eventID, soundAkObj.gameObject);
                            break;
                        }
                        else
                        {
                            parent = parent.transform.parent;
                        }
                    }
                }
            }

            yield return null;
        }

        void EndDecal()
        {
            _index++;
            if (!_canLoop && _maxTime < 0 && _index >= _particle.Length + _decalFadeOut.Length)
            {
                HideImmediatly();
            }
        }

        void Update()
        {
            if (_canLoop)
            {
                return;
            }

            _maxTime -= Time.deltaTime;
            if (_maxTime > 0)
            {
                return;
            }

            for (int i = 0; i < _particle.Length; i++)
            {
                if (_particle[i].IsAlive())
                {
                    return;
                }
                else if (!_isParticleEnd[i])
                {
                    _isParticleEnd[i] = true;
                    _index++;
                }
            }

            if (_index >= (_particle != null ? _particle.Length : 0) + (_decalFadeOut != null ? _decalFadeOut.Length : 0))
            {
                HideImmediatly();
            }
        }

        bool _canLoop = false;
        GameObject _prefab;

        ParticleSystem[] _particle;
        bool[] _isParticleEnd;

        DecalFadeOut[] _decalFadeOut;

        List<ParticleSystem> _needReturnLoop = new List<ParticleSystem>(); //партиклы, которым выключен луп и нужно его вернуть при завершении

        int _index = 0;
        float _maxTime;
    }
}