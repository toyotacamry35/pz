using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NLog;

using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

using Vector2 = SharedCode.Utils.Vector2;
using Vector3 = SharedCode.Utils.Vector3;
using Quaternion = SharedCode.Utils.Quaternion;
using System.Linq;
using System.Threading;

namespace ColonyHelpers
{
    /// <summary>
    /// Helper Functions
    /// </summary>
    public static class SharedHelpers
    {
        [NotNull] internal static readonly NLog.Logger Logger = LogManager.GetLogger("Helpers");

        private static readonly ThreadLocal<System.Random> _rnd = new ThreadLocal<Random>(() => new System.Random());
        private static System.Random _random => _rnd.Value;

    /// Math /// ---------------------------------------------------------------------
    #region Math

        public static float Rnd(float valueMinMaxAbs)
        {
            return (float)_random.NextDouble() * valueMinMaxAbs  * 2 - valueMinMaxAbs;
        }

        public static bool RndBool()
        {
            return _random.NextDouble() > 0.5;
        }

        public static Vector3 RndVec3()
        {
            return new Vector3((float)_random.NextDouble(), (float)_random.NextDouble(), (float)_random.NextDouble()).Normalized;
        }

        public static Vector3 MultiplyVectorsComponentwise(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        /// Casts angle (in degrees) to range [-180, +180]
        /// </summary>
        public static float NormalizeAngleInto180Abs(float a)
        {
            var fullTurnsWithSign = (int)(a / 360f);
            var result = a - fullTurnsWithSign * 360f;
            if (Math.Abs(result) > 180)
                result -= 360 * Math.Sign(result);
            
            return result;
        }

        public static int StochasticRound(float value)
        {
            var frac = value % 1.0f;
            frac = frac < 0.0f ? frac + 1.0f : frac;

            int min = (int)Math.Floor(value);
            int max = (int)Math.Ceiling(value);

            return _random.NextDouble() > frac ? min : max;
        }

    #endregion Math

        /// Geometry ///
        #region Geometry

        // @param `sectorAnglesDegLR`: (From Left (start), To Right (end)) clockwise, degrees
        // @param `range`: leave default float.MaxValue to skip range check
        public static bool IsPointInsideSector(SharedCode.Utils.Vector2 sectorAnglesDegLR, SharedCode.Utils.Vector3 subjectPos, SharedCode.Utils.Vector3 subjectForward, SharedCode.Utils.Vector3 targetPos, bool debugDraw = false, float range = float.MaxValue)
        {
            if (range < float.MaxValue)
            {
                var distance = SharedCode.Utils.Vector3.Distance(targetPos, subjectPos);
                if (distance > range)
                    return false;
            }

            // 0. Normalize Angles Into +/-180 deg:
            var sector = new SharedCode.Utils.Vector2(NormalizeAngleInto180Abs(sectorAnglesDegLR.x),
                                     NormalizeAngleInto180Abs(sectorAnglesDegLR.y));
            // 1. Calc. total angle:
            float totalAngle;
            void recalcTotalAngle()
            {
                totalAngle = sector.y - sector.x;
                if (totalAngle < 0f)
                    totalAngle += 360f;
            }
            recalcTotalAngle();

            // 2. Inverse sector (& task) if total angle > 180: (to simplify solution)
            bool isSectorInversed = false;
            if (totalAngle > 180f)
            {
                Swap(ref sector.x, ref sector.y);
                isSectorInversed = true;
                //Update totalAngle:
                recalcTotalAngle();
            }

            // 3. Find sector bisector:
            var avrgAngle = (sector.x + sector.y)/2;
            // W/o this trick, bisector is independent of borders order & always closer to 0.
            bool sameSigns = Sign(sector.x) == Sign(sector.y);
            var bisectorAngle = (sameSigns || sector.x <= 0f)
                ? avrgAngle
                : avrgAngle + 180f * (-1) * Sign(avrgAngle);

            // 4. Calc. result:
            var halfAngle = totalAngle / 2f;
            var fwd2D = subjectForward.XZ;
            var bisectorVector = fwd2D.RotateDeg(bisectorAngle);
            var toTrg2D = (targetPos - subjectPos).XZ;
            var angle = SharedCode.Utils.Vector2.Angle(bisectorVector, toTrg2D);

            var result = isSectorInversed ^ angle <= halfAngle;

            Logger.Trace(string.Format("#answ: {6}. sector:{0}, total:{1}, halfA{2}, bisectorA:{3}, bisectorVec:{4}. Angle:{5}. fwd:{7}"
                , sectorAnglesDegLR, totalAngle, halfAngle, bisectorAngle, bisectorVector, angle, result, fwd2D));

            /*if (DebugExtension.Draw && debugDraw)
            {
                var p0 = subjectPos;
                var r = 20f;
                var duration = 2f;
                Debug.DrawLine(p0.ToUnityVector3(), (p0 + subjectForward * r).ToUnityVector3(), Color.black, duration, false);
                DebugExtension.DebugCircle(p0.ToUnityVector3(), Vector3.up, Color.gray, r, duration, false);
                Debug.DrawLine(p0.ToUnityVector3(), (p0 + bisectorVector.AsXZ * r).ToUnityVector3(), Color.yellow, duration, false);
                Debug.DrawLine(p0.ToUnityVector3(), (p0 + bisectorVector.RotateDeg(halfAngle).AsXZ * r).ToUnityVector3(), Color.magenta, duration, false);
                Debug.DrawLine(p0.ToUnityVector3(), (p0 + bisectorVector.RotateDeg(-halfAngle).AsXZ * r).ToUnityVector3(), Color.blue, duration, false);
                var colr = result ? Color.green : Color.red;
                Debug.DrawLine(p0.ToUnityVector3(), targetPos.ToUnityVector3(), colr, 1f, false);
            }*/

            return result;
        }

    #endregion Geometry

    /// Data & Data structures: ///
    #region DataStructures

        public static void Swap<T>(ref T a, ref T b) where T : struct
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

    #endregion

    /// Logic: ///
    #region Logic

        public static bool IsOneOf<T>(T dArtagnan, T[] musketeers)
        {
            foreach (var mu in musketeers)
            {
                //if (dArtagnan == mu)
                if (EqualityComparer<T>.Default.Equals(dArtagnan, mu))
                    return true;
            }

            return false;
        }

    #endregion //Logic

    /// UtilTypes: ///
    #region UtilTypes

        public struct PosRotScale
        {
            public Vector3 Pos;
            public Quaternion Rot;
            public Vector3 Scale;

            public PosRotScale(Vector3 pos, Quaternion rot, Vector3 scale)
            {
                Pos = pos;
                Rot = rot;
                Scale = scale;
            }

            public PosRotScale(Vector3 pos, Quaternion rot)
            {
                Pos = pos;
                Rot = rot;
                Scale = Vector3.one;
            }
        }

    #endregion //UtilTypes



    }//of class Hf


    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(this IEnumerable<T> source, Random rng)
        {
            var buffer = source is IList<T> ? (IList<T>)source : source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }

    public enum Subset : byte
    {
        None,
        Any,
        All
    }
}
