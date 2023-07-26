using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace ColonyHelpers
{
    /// <summary>
    /// Helper Functions
    /// </summary>
    public static class Helpers
    {
        [NotNull] internal static readonly NLog.Logger Logger = LogManager.GetLogger("Helpers");

        /// Quaternions: ///
        public static Quaternion QuatFromMatrix(Matrix4x4 m)
        {
            //#aK: got from here (http://answers.unity3d.com/answers/11372/view.html):
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = (float)Math.Sqrt(Math.Max(0, (double)(1 + m[0, 0] + m[1, 1] + m[2, 2]))) / 2;
            q.x = (float)Math.Sqrt(Math.Max(0, (double)(1 + m[0, 0] - m[1, 1] - m[2, 2]))) / 2;
            q.y = (float)Math.Sqrt(Math.Max(0, (double)(1 - m[0, 0] + m[1, 1] - m[2, 2]))) / 2;
            q.z = (float)Math.Sqrt(Math.Max(0, (double)(1 - m[0, 0] - m[1, 1] + m[2, 2]))) / 2;
            q.x *= Math.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Math.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Math.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }


        public static OuterRef<IEntity> GetObjectEntityRef(GameObject obj)
        {
            Guid guid = Guid.Empty;
            int typeId = 0;
            if (obj != null)
            {
                var ego = obj.GetComponent<EntityGameObject>();
                if (ego != null)
                {
                    guid = ego.EntityId;
                    typeId = ego.TypeId;
                }
            }

            return new OuterRef<IEntity>(guid, typeId);
        }

        // Write to log if true
        public static bool IsNullAssert<T>(T targetVar,
            string varName,
            GameObject context = null,
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMethodName = "")
        //where T : class 
        {
            if (default(T) != null)
            {
                Logger.IfError()?.Message("Using IsNullAssert method for not nullable type: " + typeof(T)).Write();
                return false;
            }

            bool res = targetVar == null;
            if (res)
            {
                Logger.Error(callerMethodName == ""
                        ? LogObjectExtensions.GetShortDescr(typeof(T), varName, "is null")
                        : LogObjectExtensions.GetFullDescr(typeof(T), varName, "is null", callerMethodName, callerFilePath, callerLineNumber),
                    context);
            }
            return res;
        }

        /// Unity: ///
        #region Unity

        public static T FindSingleComponentOnScene<T>(Scene scene) where T : MonoBehaviour
            {
                T res = null;
                foreach (var obj in scene.GetRootGameObjects())
                {
                    var comp = obj.GetComponent<T>();
                    if (comp != null)
                    {
                        if (res != null)
                        {
                            Logger.IfError()?.Message("Found multiple objects with component of type {0} scene hiererchy for obj {1} in scene {2}", typeof(T), obj, scene.name).Write();
                            throw new ArgumentOutOfRangeException();
                        }

                        res = comp;
                    }
                }

                return res;
            }

        #endregion //UtilTypes
           /// Physics ///
    #region Physics

        public static RaycastHit FindHit(Vector3 fromPoint, GameObject targetObj, float maxRayLength)
        {
            var targetCenter = targetObj.GetComponent<Collider>()?.bounds.center
                               ?? targetObj.transform.position;
            var hits = Physics.RaycastAll(fromPoint, targetCenter - fromPoint, PhysicsChecker.CheckDistance(maxRayLength, targetObj.name));
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == targetObj)
                {
                    //#Dbg:
                    if (!Mathf.Approximately(hit.normal.magnitude, 1f))
                        Logger.IfWarn()?.Message($"hit.normal.magnitude != 1 (=={hit.normal.magnitude})").Write();

                    return hit;
                }
            }

            return default(RaycastHit);
        }

        public static Vector3 GetCenterPoint([NotNull] GameObject go)
        {
            return go.GetComponent<Collider>()?.bounds.center
                ?? go.transform.position;
        }

    #endregion //Physics

    #region ClusterHelpers

        public static OuterRef<IEntity> GetCharacterRef()
        {
            var typeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter));
            var characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (characterId == Guid.Empty)
            {
                Logger.IfError()?.Message($"{nameof(characterId)} is empty").Write();
                return default;
            }

            var charRef = new OuterRef<IEntity>(characterId, typeId);
            return charRef;
        }

    #endregion ClusterHelpers

    }


    public static class DebugHelper
    {
        // Custom colors:
        public static readonly Color ColorOrange = new Color(255 / 255f, 145 / 255f, 0 / 255f);
        public static readonly Color ColorLightBlue = new Color(100 / 255f, 200 / 255f, 240 / 255f);
        public static readonly Color ColorDarkBlue = new Color(24 / 255f, 96 / 255f, 103 / 255f);
        public static readonly Color ColorLightYellow = new Color(200 / 255f, 200 / 255f, 80 / 255f);
        public static readonly Color ColorDarkYellow = new Color(169 / 255f, 123 / 255f, 39 / 255f);
        public static readonly Color ColorLightRed = new Color(252 / 255f, 110 / 255f, 130 / 255f);
        public static readonly Color ColorDarkRed = new Color(169 / 255f, 62 / 255f, 39 / 255f);
        public static readonly Color ColorDarkDarkRed = new Color(114 / 255f, 42 / 255f, 26 / 255f);
        public static readonly Color ColorLightGrey = new Color(115 / 255f, 115 / 255f, 115 / 255f);
        public static readonly Color ColorDarkGrey = new Color(85 / 255f, 85 / 255f, 85 / 255f);
        public static readonly Color ColorPurple = new Color(73 / 255f, 46 / 255f, 116 / 255f);
        public static readonly Color ColorEmerald = new Color(38 / 255f, 113 / 255f, 88 / 255f);

        public static readonly List<Color> ColorsSoft = new List<Color>()
        {
            ColorOrange     ,
            ColorLightBlue  ,
            ColorDarkBlue   ,
            ColorLightYellow,
            ColorDarkYellow ,
            ColorLightRed   ,
            ColorDarkRed    ,
            ColorLightGrey  ,
            ColorDarkGrey   ,
            ColorPurple     ,
            ColorEmerald    ,
        };

        public static readonly List<Color> Colors = new List<Color>()
        {
            Color.blue,
            Color.cyan,
            Color.gray,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow
        };

        public enum ColorsSet : byte
        {
            BaseColors,
            SoftColors
        }

        public static Color GetRandomColorByIndex(int i, ColorsSet colorsSet = ColorsSet.BaseColors)
        {
            List<Color> colors = (colorsSet == ColorsSet.BaseColors) ? Colors : ColorsSoft;
            //Clamp i into colors size:
            int index = i % colors.Count;
            if (index < 0)
                index = colors.Count + index;

            return colors[index];
        }

    }


    public static class UnityLogHelper
    {
        public static void Log(LogType logtype, string format, params object[] args)
        {
            switch (logtype)
            {
                case LogType.Log:
                    Debug.LogFormat(format, args);
                    break;
                case LogType.Warning:
                    Debug.LogWarningFormat(format, args);
                    break;
                case LogType.Error:  // no `break` is Ok: fall in
                case LogType.Assert: // no `break` is Ok: fall in
                case LogType.Exception:
                    Debug.LogErrorFormat(format, args);
                    break;
                default:
                    Debug.LogError("Unexpected default switch!");
                    break;
            }
        }
    }


}
