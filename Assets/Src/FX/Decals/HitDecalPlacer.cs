using Assets.ColonyShared.SharedCode.Aspects.FX.Decals;
using Assets.Src.Character.Events;
using Core.Reflection;
using Assets.Src.Shared;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.Src.FX.Decals
{
    //TODO: Объединить в единую систему декалей
    public static class HitDecalPlacer
    {
        public static void PlaceDecal(HitDecalInfo indicator, HitInfo hitInfo)
        {
            var type = indicator._hitDecalPlacer.Target;
            var gObj = indicator._decal.Target;
            if (type != null && gObj != null)
            {
                DecalPlacerSelector.PlaceDecal(type, hitInfo, gObj);
            }
        }
    }

    public static class DecalPlacerSelector
    {
        private static readonly Dictionary<Type, Action<IHitDecalPlacerDef, HitInfo, GameObject>> _typeToImpl;

        static DecalPlacerSelector()
        {
            var x = MethodBase.GetCurrentMethod().DeclaringType.GetMethodsSafe(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(method => (method.Name == nameof(PlaceDecalImpl))
                && (method.GetParameters().Length == 3)
                && (method.GetParameters()[1].ParameterType == typeof(HitInfo))
                && (method.GetParameters()[2].ParameterType == typeof(GameObject)));
            var y = x.Where(method =>
                (typeof(IHitDecalPlacerDef).IsAssignableFrom(method.GetParameters()[0].ParameterType)));

            _typeToImpl = MethodBase.GetCurrentMethod().DeclaringType.GetMethodsSafe(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(method => (method.Name == nameof(PlaceDecalImpl))
                && (method.GetParameters().Length == 3)
                && (method.GetParameters()[1].ParameterType == typeof(HitInfo))
                && (method.GetParameters()[2].ParameterType == typeof(GameObject))
                && (typeof(IHitDecalPlacerDef).IsAssignableFrom(method.GetParameters()[0].ParameterType)))
                .Select(method => new
                {
                    type = method.GetParameters()[0].ParameterType,
                    action = HitDelegateCreator.MagicMethod<IHitDecalPlacerDef>(method)
                })
                .ToDictionary(v => v.type, v => v.action);
        }

        public static void PlaceDecal(IHitDecalPlacerDef def, HitInfo hitInfo, GameObject gObj)
        {
            _typeToImpl[def.GetType()](def, hitInfo, gObj);
        }

        [UsedImplicitly]
        private static void PlaceDecalImpl(SimplifiedHitDecalPlacerDef def, HitInfo hitInfo, GameObject decal)
        {
            var hitPosition = hitInfo.Point;
            var shiftVector = (hitInfo.Rotation * Vector3.up) * 0.75f;
            var raycastPosition = hitPosition + shiftVector;
            var rHit = Physics.RaycastAll(raycastPosition, -shiftVector, 1.2f, PhysicsLayers.CheckIsGroundedMask);
            if (rHit.Length != 0)
            {
                var hit = rHit[0];
                var rotation = Quaternion.LookRotation(hit.normal);
                var hitEffectObject = GameObject.Instantiate(decal, hit.point, rotation);
                if (hitEffectObject != null)
                {
                    hitEffectObject.transform.rotation = rotation;
                    var remover = hitEffectObject.AddComponent<ObjectDelayedRemover>();
                    remover.DestroyWithDelay(120);
                }
            }
        }

        [UsedImplicitly]
        private static void PlaceDecalImpl(BasicHitDecalPlacerDef def, HitInfo hitInfo, GameObject decal)
        {
            throw new NotImplementedException();
        }
    }

    public static class HitDelegateCreator
    {
        private static Action<BaseType, HitInfo, GameObject> MagicMethodHelper<BaseType, TargetType>(MethodInfo method) where TargetType : BaseType
        {
            var func = (Action<TargetType, HitInfo, GameObject>)Delegate.CreateDelegate
                (typeof(Action<TargetType, HitInfo, GameObject>), method);
            Action<BaseType, HitInfo, GameObject> ret = (BaseType param, HitInfo hitInfo, GameObject obj) => func((TargetType)param, hitInfo, obj);
            return ret;
        }

    public static Action<BaseType, HitInfo, GameObject> MagicMethod<BaseType>(MethodInfo method)
        {
            MethodInfo genericHelper = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(MagicMethodHelper),
                BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo constructedHelper = genericHelper.MakeGenericMethod(typeof(BaseType), method.GetParameters()[0].ParameterType);
            object ret = constructedHelper.Invoke(null, new object[] { method });
            return (Action<BaseType, HitInfo, GameObject>)ret;
        }
    }
}
