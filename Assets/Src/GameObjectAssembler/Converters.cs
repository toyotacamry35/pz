using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.GameObjectAssembler
{
    public class Converters
    {
        public static bool TryGet((Type, Type) pair, out Delegate converter) => converters.TryGetValue(pair, out converter);

        private static Dictionary<(Type, Type), Delegate> converters = new []
        {
            MakeConverter<SharedCode.Utils.Vector2, Vector2>(nameof(ConvertVector2)),
            MakeConverter<SharedCode.Utils.Vector3, Vector3>(nameof(ConvertVector3)),
            MakeConverter<SharedCode.Utils.Quaternion, Quaternion>(nameof(ConvertQuaternion)),
            MakeConverter<string[], LayerMask>(nameof(ConvertLayerMask))
        }.ToDictionary(x => x.Item1, x => x.Item2);

        private static ((Type,Type), Delegate) MakeConverter<TIn,TOut>(string method) => 
            ((typeof(TIn), typeof(TOut)), Delegate.CreateDelegate(typeof(Func<TIn, TOut>), typeof(Converters), method));

        private static Vector2 ConvertVector2(SharedCode.Utils.Vector2 v) => new Vector2(v.x, v.y);

        private static Vector3 ConvertVector3(SharedCode.Utils.Vector3 v) => new Vector3(v.x, v.y, v.z);
        
        private static Quaternion ConvertQuaternion(SharedCode.Utils.Quaternion v) => new Quaternion(v.x, v.y, v.z, v.w);
        
        private static LayerMask ConvertLayerMask(string[] names) => LayerMask.GetMask(names);

    }
}