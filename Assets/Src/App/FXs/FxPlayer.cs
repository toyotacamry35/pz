using System.Collections.Generic;
using Assets.Src.Effects.FX;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using NLog;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.App.FXs
{
    internal static class FxPlayer
    {
        [NotNull]
        public static readonly NLog.Logger Logger = LogManager.GetLogger("FXes");

        internal static readonly Vector3 DefaultDirection = new Vector3(0, 1, 0);
        internal static readonly float FxSmallOffseet = 0.05f;

        public class FX
        {
            public readonly SpellWordDef effect;
            public readonly GameObject obj;

            public FX(SpellWordDef setEffect, GameObject setObj)
            {
                effect = setEffect;
                obj = setObj;
            }
        }

        static Dictionary<SpellId, List<FX>> dictionary = new Dictionary<SpellId, List<FX>>();

        //эффект
        internal static void StartPlay(FxData fxData, FXInfo fxInfo, SpellId orderId, SpellWordDef effect)
        {
            GameObject prefab = GetPrefab(fxData, fxInfo.markerId);
            StartPlay(prefab, fxInfo, orderId, effect);
        }

        internal static void StartPlay(GameObject prefab, FXInfo fxInfo, SpellId orderId, SpellWordDef effect)
        {
            if (prefab != null)
            {
                GameObject go = StartFxQueue(prefab, fxInfo, true);
                if (!go.AssertIfNull(nameof(go)))
                    AddToDictionary(go, orderId, effect);
            }
        }

        //impact
        internal static void StartPlay(FxData fxData, FXInfo fxInfo, bool autoDestoy = false)
        {
            GameObject prefab = GetPrefab(fxData, fxInfo.markerId);
            if (prefab != null)
            {
                if (!autoDestoy)
                {
                    StartFxQueue(prefab, fxInfo, false);
                }
                else
                {
                    GameObject go = StartFxInstantiate(prefab, fxInfo);
                    GameObject.Destroy(go, fxData.LifeTimeSec);
                }
            }
        }

        internal static GameObject StartPlay(GameObject prefab, FXInfo fxInfo, bool canLoop = false)
        {
            if (prefab != null)
            {
                return StartFxQueue(prefab, fxInfo, canLoop);
            }

            return null;
        }

        //
        internal static void StopPlay(SpellId orderId, SpellWordDef effect, bool immediatlyHide = true)
        {
                List<FX> needRemove = new List<FX>();
                foreach (var fx in dictionary[orderId])
            {
                    if (fx.effect == effect)
                    {
                        needRemove.Add(fx);
                    }
                }

                foreach (var fx in needRemove)
                {
                    dictionary[orderId].Remove(fx);

                    FXElement fxElement = fx.obj.GetComponent<FXElement>();
                    if (fxElement != null)
                    {
                        if (immediatlyHide)
                        {
                            fxElement.HideImmediatly();
                        }
                        else
                        {
                            fxElement.HideLoop();
                        }
                    }
                    else
                    {
                        GameObject.Destroy(fx.obj);
                    }
                }

                if (dictionary[orderId].Count == 0)
                {
                dictionary.Remove(orderId);
            }
        }

        internal static GameObject GetPrefab(FxData fxData, BaseResource marker = null)
        {
            GameObject prefab = null;
            if (fxData.MarkerToPrefab != null && marker != null)
            {
                prefab = fxData.MarkerToPrefab.Get(marker);
            }

            if (prefab == null)
            {
                prefab = fxData.ParticlePrefab;
            }

            return prefab;
        }

        // --- Privates: --------------------------------------------------------------------------------------------------------

        private static void AddToDictionary(GameObject go, SpellId orderId, SpellWordDef effect)
        {
            if (dictionary.ContainsKey(orderId))
            {
                dictionary[orderId].Add(new FX(effect, go));
            }
            else
            {
                List<FX> list = new List<FX>();
                list.Add(new FX(effect, go));
                dictionary.Add(orderId, list);
            }
        }

        private static GameObject StartFxInstantiate(GameObject prefab, FXInfo fxInfo)
        {
            GameObject go = GameObject.Instantiate(prefab);
            if (fxInfo.parent != null)
            {
                go.transform.SetParent(fxInfo.parent.transform);
            }

            go.transform.position = fxInfo.position;
            go.transform.rotation = fxInfo.rotation;
            go.transform.localScale = Vector3.one;
            return go;
        }

        private static GameObject StartFxQueue(GameObject prefab, FXInfo fxInfo, bool canLoop)
        {
            GameObject go = FXQueue.Instance.Get(prefab, fxInfo.position, fxInfo.rotation, canLoop, fxInfo.pair);
            if (fxInfo.parent != null && go != null)
            {
                go.transform.SetParent(fxInfo.parent.transform);
            }

            go.transform.localScale = Vector3.one;
            return go;
        }
    }
}