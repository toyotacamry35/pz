using System.Collections;
using System.Collections.Generic;
using Assets.Src.App.FXs;
using Uins;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class FXQueue : MonoBehaviour
    {
        static FXQueue _instance = null;

        public static FXQueue Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("FXQueue");
                    go.hideFlags = HideFlags.HideAndDontSave;
                    _instance = go.AddComponent<FXQueue>();
                }

                return _instance;
            }
        }

        public class ListElement
        {
            public GameObject prefab;
            List<FXElement> _list;

            public int count
            {
                get
                {
                    if (_list == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return _list.Count;
                    }
                }
            }

            public int getCount { get; set; } = 0; //количество элементов забаранных из очереди
            public int needCount { get; set; } = 0; //предполагаемое количество элементов, которые могут пригодится

            public void Add(FXElement element)
            {
                if (_list == null)
                {
                    _list = new List<FXElement>();
                }

                _list.Add(element);
            }

            public GameObject Get(Vector3 position, Quaternion rotation, bool canLoop = false, FXParamsOnObj.FXParamsValue[] pair = null)
            {
                getCount++;
                if (_list == null || _list.Count == 0)
                    return null;

                var fxElement = _list[0];
                _list.RemoveAt(0);

                GameObject fxElementGameObject = null;
                if (fxElement != null && fxElement.gameObject )
                {
                    fxElement.Show(position, rotation, canLoop, pair);
                    fxElementGameObject = fxElement.gameObject;
                }

                return fxElementGameObject;
            }

            public FXElement Create(bool addToQueue, Vector3 position, Quaternion rotation, bool canLoop = false)
            {
                if (prefab != null)
                {
                    if (addToQueue)
                    {
                        prefab.SetActive(false);
                    }

                    GameObject go = Instantiate(prefab, position, rotation);
                    FXElement[] elements = go.GetComponentsInChildren<FXElement>(true);
                    FXElement element;
                    if (elements != null && elements.Length > 0)
                    {
                        element = elements[0];
                    }
                    else
                    {
                        element = go.AddComponent<FXElement>();
                    }

                    element.Init(prefab, canLoop);
                    if (addToQueue)
                    {
                        Add(element);
                    }

                    return element;
                }

                return null;
            }

            public void Remove()
            {
                if (count >= 1)
                {
                    var lastElement = _list[count - 1];
                    _list.RemoveAt(count - 1);
                    if (lastElement != null)
                        GameObject.Destroy(lastElement.gameObject);
                }
            }
        }

        public void Add(GameObject prefab, FXElement element) //уже использованный эффект добавляется в очередь
        {
            if (prefab == null || element == null)
            {
                FxPlayer.Logger.Info($"FX queue: Add({nameof(prefab)}={(prefab == null ? "null" : prefab.name)}, " +
                                      $"{nameof(element)}={(element == null ? "null" : element.name)}) Attempt to add null param(s)");
                return;
            }

            if (_queue == null)
                _queue = new Dictionary<GameObject, ListElement>();

            if (!_queue.ContainsKey(prefab))
            {
                _queue.Add(prefab, new ListElement());
                _queue[prefab].prefab = prefab;
            }

            _queue[prefab].Add(element);
        }

        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, bool canLoop = false,
            FXParamsOnObj.FXParamsValue[] pair = null) //запустить указанный эффект
        {
            if (prefab.AssertIfNull(nameof(prefab)))
                return null;

            if (!_isCoroutineRun)
            {
                this.StartInstrumentedCoroutine(CoroutineNeedCount());
            }

            if (_queue == null)
            {
                _queue = new Dictionary<GameObject, ListElement>();
            }

            if (!_queue.ContainsKey(prefab))
            {
                _queue.Add(prefab, new ListElement());
                _queue[prefab].prefab = prefab;
            }

            GameObject go = _queue[prefab].Get(position, rotation, canLoop, pair);
            if (go != null)
                return go;

            //в очереди эффекта нет, создаем его

            FXElement fxElement = _queue[prefab].Create(false, position, rotation, canLoop);
            if (fxElement != null && fxElement.gameObject != null)
                fxElement.Show(position, rotation, canLoop, pair);

            return fxElement?.gameObject;
        }

        IEnumerator CoroutineNeedCount()
        {
            _isCoroutineRun = true;
            yield return new WaitForSeconds(UPDATE_TIME);

            while (_isCoroutineRun)
            {
                float time = UPDATE_TIME;
                foreach (var pair in _queue)
                {
                    pair.Value.needCount = pair.Value.getCount;
                    pair.Value.getCount = 0;
                }

                time -= Time.deltaTime;
                yield return null;

                foreach (var pair in _queue)
                {
                    if (time > 0 && pair.Value.count > pair.Value.needCount) //лишние элементы
                    {
                        int currentCount = pair.Value.count;
                        for (int i = pair.Value.needCount; i < currentCount; i++)
                        {
                            pair.Value.Remove();
                        }

                        time -= Time.deltaTime;
                    }
                    else
                    {
                        while (time > 0 && pair.Value.count + 2 <= pair.Value.needCount)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                pair.Value.Create(true, Vector3.zero, Quaternion.identity);
                            }

                            time -= Time.deltaTime;
                        }
                    }
                }

                yield return new WaitForSeconds(time);
            }

            _isCoroutineRun = false;
            yield return null;
        }


        Dictionary<GameObject, ListElement> _queue;
        bool _isCoroutineRun = false;
        const float UPDATE_TIME = 30f;
    }
}