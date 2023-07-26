using System;
using Assets.ColonyShared.SharedCode.Interfaces;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using SharedCode.Utils;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace Src.Locomotion.Unity
{
    public class NavMeshGroundSensor : ILocomotionGroundSensor, ILocomotionUpdateable, IResettable
    {
        private readonly ISettings _settings;
        private readonly NavMeshAgent _agent;
        private readonly ILocomotionBody _body;
        [CanBeNull]
        private readonly ICurveLoggerProvider _curveLogProv;

        public NavMeshGroundSensor(ISettings settings, NavMeshAgent agent, ILocomotionBody body, [CanBeNull] ICurveLoggerProvider curveLogProv = null )
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            _agent = agent;
            _body = body;
            _curveLogProv = curveLogProv;
            _settings = settings;
        }

        public void Reset()
        {
            DistanceToGround = 0f;
            OnGround = true;
        }

        public bool OnGround { get; private set; } = true;

        public float DistanceToGround { get; private set; }

        public LocomotionVector GroundNormal => LocomotionVector.Up;

        public bool HasGroundContact => OnGround;

        public void ForcedUpdateDistanceToGroundLongRay() //#todo: (#PZ-7910) think about: Is it correct solution? May be make it pure func (then for character just do same as `DistanceToGround`)
        {
            //do nothing
        }

        void ILocomotionUpdateable.Update(float deltaTime)
        {
            var v = _body.Position;
            var bodyPosition = VectorConvert.LocomotionToWorldVectorAndToUnity(ref v);
            
            if (!_agent.isOnNavMesh)
            {
                DistanceToGround = 0;
            }
            else if (!_agent.isOnOffMeshLink)
            {
                //#wrong?(dist-to-grnd could be negative!): DistanceToGround = _body.Position.Vertical - _agent.nextPosition.y; ///#PZ-13568 del comment , but ask Andrey
                DistanceToGround = Math.Max(_body.Position.Vertical - _agent.nextPosition.y, 0);
            }
            else
            {
                var data = _agent.currentOffMeshLinkData;
                Vector3 ptOnPlane; 
                if (data.OffMeshLinkPlaneRaycast(bodyPosition, Vector3.down, out ptOnPlane))
                    DistanceToGround = bodyPosition.y - Mathf.Clamp(ptOnPlane.y, Mathf.Min(data.startPos.y, data.endPos.y), Mathf.Max(data.startPos.y, data.endPos.y));
                else
                    DistanceToGround = bodyPosition.y - data.endPos.y;
            }

            _curveLogProv?.CurveLogger?.IfActive?.AddData("0.03) GrndSensor.isOnNavMesh", SyncTime.Now, _agent.isOnNavMesh ? 1 : 0);

            OnGround = DistanceToGround <= _settings.RaycastGroundTolerance;
        }

        public interface ISettings
        {
            float RaycastGroundTolerance { get; }
        }
    }
}