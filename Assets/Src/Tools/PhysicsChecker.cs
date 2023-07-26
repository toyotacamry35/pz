using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.Tools
{
    public static class PhysicsChecker
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Vector3 CheckExtentsSize(Vector3 vec3, string tag = null, [CallerMemberName] string callerName = null)
        {
            if (vec3.x > 12 || vec3.y > 12 || vec3.z > 12)
                Logger.IfError()?.Message($"{nameof(CheckExtentsSize)} vec3 is too big {vec3} {tag ?? "null"} callerName {callerName}").Write();
            return vec3;
        }

        public static Vector3 CheckBoundsSize(Vector3 vec3, string tag = null, [CallerMemberName] string callerName = null)
        {
            if (vec3.x > 25 || vec3.y > 25 || vec3.z > 25)
                Logger.IfError()?.Message($"{nameof(CheckBoundsSize)} vec3 is too big {vec3} {tag ?? "null"} callerName {callerName}").Write();
            return vec3;
        }

        public static float CheckRadius(float radius, string tag = null, [CallerMemberName] string callerName = null)
        {
            if (radius > 70)
                Logger.IfError()?.Message($"{nameof(CheckRadius)} radius is too big {radius} {tag ?? "null"} callerName {callerName}").Write();
            return radius;
        }

        public static float CheckDistance(float distance, string tag = null, [CallerMemberName] string callerName = null)
        {
            if (distance > 150)
                Logger.IfError()?.Message($"{nameof(CheckDistance)}: distance is too big {distance} {tag ?? "null"} callerName {callerName}").Write();
            return distance;
        }

        public static float CheckClientDistance(float distance, string tag = null, [CallerMemberName] string callerName = null)
        {
            if (distance > 2001)
                Logger.IfError()?.Message($"{nameof(CheckClientDistance)}: distance is too big {distance} {tag ?? "null"} callerName='{callerName}'").Write();
            return distance;
        }
    }
}
