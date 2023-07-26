using ColonyShared.SharedCode.Aspects.Locomotion;
using SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using NLog;
using System.Diagnostics;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Src.Locomotion
{
    public struct LocomotionVariables
    {
        public LocomotionTimestamp Timestamp;
        
        /// <summary>
        /// Флаги типа перемещения 
        /// </summary>
        public LocomotionFlags Flags;

        /// <summary>
        /// Исходная мировая позиция 
        /// </summary>
        public LocomotionVector Position;

        /// <summary>
        /// Вектор скорости 
        /// </summary>
        public LocomotionVector Velocity;

        /// <summary>
        /// Дополнительная скорость для с snap to ground, step up и т п. Не передаётся по сети. 
        /// </summary>
        public LocomotionVector ExtraVelocity;

        /// <summary>
        /// Ориентация тела заданная углом поворота вокруг вертикальной оси (в радианах, + против часовой стрелки, - по часовой).
        /// </summary>
        public float Orientation;

        /// <summary>
        /// Скорость повотора. В рад/сек, положительное значение - против часовой стрелки, отрицательное значение - по часовой стрелке  
        /// </summary>
        public float AngularVelocity
        {
            get => _angularVelocity;
            set
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                if (float.IsNaN(value))
                {
                    StackTrace st = new StackTrace();
                    Logger.IfError()?.Message($"AngularVelocity is NaN, attempt to set 0\n{st.ToString()}").Write();
                    _angularVelocity = 0;
                }
                else if (float.IsInfinity(value) || float.IsNegativeInfinity(value))
                {
                    StackTrace st = new StackTrace();
                    Logger.IfError()?.Message($"AngularVelocity Is Infinity (or Is NegativeInfinity), attempt to set 0\n{st.ToString()}").Write();
                    _angularVelocity = 0;
                }
                else
#endif
                    _angularVelocity = value;

            }
        }
        private float _angularVelocity;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///  Непосредственное изменение координат в виде смещения относительно Position
        /// </summary>
        public LocomotionVector ExtraPosition;

        public LocomotionVariables(
            LocomotionTimestamp timestamp,
            LocomotionFlags flags,
            LocomotionVector position,
            LocomotionVector velocity,
            LocomotionVector extraVelocity,
            float orientation,
            float angularVelocity = 0,
            LocomotionVector extraPosition = default(LocomotionVector)
        )
        {
            Timestamp = timestamp; 
            Flags = flags;
            Position = position;
            Velocity = velocity;
            ExtraVelocity = extraVelocity;
            Orientation = orientation;

            if (float.IsNaN(angularVelocity))
            {
                StackTrace st = new StackTrace();
                Logger.IfError()?.Message($"AngularVelocity is NaN, attempt to set 0\n{st.ToString()}").Write();
                _angularVelocity = 0;
            }
            else if (float.IsInfinity(angularVelocity) || float.IsNegativeInfinity(angularVelocity))
            {
                StackTrace st = new StackTrace();
                Logger.IfError()?.Message($"AngularVelocity Is Infinity (or Is NegativeInfinity), attempt to set 0\n{st.ToString()}").Write();
                _angularVelocity = 0;
            }
            else
                _angularVelocity = angularVelocity;

            ExtraPosition = extraPosition;
        }
        
        public LocomotionVariables(
            LocomotionTimestamp timestamp,
            LocomotionFlags flags,
            LocomotionVector position,
            LocomotionVector velocity,
            float orientation,
            float angularVelocity = 0
        )
            : this(timestamp, flags, position, velocity, LocomotionVector.Zero, orientation, angularVelocity, LocomotionVector.Zero)
        {
        }
        
        public LocomotionVariables(
            LocomotionFlags flags,
            LocomotionVector position,
            LocomotionVector velocity,
            float orientation,
            float angularVelocity = 0
        )
            : this(LocomotionTimestamp.None, flags, position, velocity, LocomotionVector.Zero, orientation, angularVelocity, LocomotionVector.Zero)
        {
        }
        
        public LocomotionVariables(LocomotionVariables vars)
        {
            this = vars;
        }
        
        public static readonly LocomotionVariables None = new LocomotionVariables(0, LocomotionVector.Zero, LocomotionVector.Zero, 0);
        
        public override string ToString()
        {
            var sb = StringBuildersPool.Get
                .Append(Flags)
                .Append(" | ").Append(Position.ToString())
                .Append(" | ").Append(Velocity.ToString())
                .Append(" | ").Append((int) (Orientation * Mathf.Rad2Deg));
            if (!AngularVelocity.ApproximatelyZero())
                sb.Append(" | ").Append((int) (AngularVelocity * Mathf.Rad2Deg));
            return sb.ToString();
        }
    }
}
