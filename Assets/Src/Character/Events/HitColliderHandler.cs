using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Lib.Extensions;
using Assets.Src.Shared;
using Assets.Src.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using NLog;
using Plugins.DebugDrawingExtension;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Utils.DebugCollector;
using Src.Animation;
using Src.Locomotion.Unity;
using Src.Tools;
using UnityEngine;
using PositionRotation = Assets.Src.Aspects.Impl.PositionRotation;

namespace Assets.Src.Character.Events
{
    public delegate void HitColliderDelegate(SpellPartCastId id, in HitInfo hit);
    
    public class HitColliderHandler : MonoBehaviour
    {
        private const int IterationsLimit = 16;
        
        private static readonly DebugDraw Drawer = DebugDraw.Manager.GetDrawer("HitColliderHandler");

        [SerializeField] private Collider HitCollider;
        [SerializeField] private QueryTriggerInteraction QueryTriggers = QueryTriggerInteraction.Collide;

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Collider[] CollidersBuffer = new Collider[100];
        
        private HitColliderDelegate _onAttackColliderHit;
        private bool _initialized;
        private float _colliderWidth;
        private PositionRotation _colliderPose; // смещение _центра коллайдера_ и его поворот относительно кости к которой прикреплено оружие
        private DamageTypeDef _damageType = default(DamageTypeDef);
        private readonly List<Session> _sessions = new List<Session>();
        private GameObject _root;
        private GameObjectMatcher _objectMatcher;

        public GameObjectMarkerDef Marker { get; private set; }
        
        public void ActivateCollider(SpellPartCastId attackId, Transform body, AnimationTrajectory trajectory, float trajectoryStart, float trajectoryEnd, float speed)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));
            if (trajectory == null) throw new ArgumentNullException(nameof(trajectory));

            if (!_initialized)
                return;

            enabled = true;
            _root = transform.gameObject.GetRoot();
            
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"HitCollider Start | TimeRange:[{trajectoryStart},{trajectoryEnd}] Speed:{speed} Attacker:{body.name} Trajectory:{trajectory.name} AttackId:{attackId}").Write();
            var trajectoryTime = Mathf.Max(trajectoryStart, trajectory.Position.FirstTime);
            var trajectoryPose = trajectory.Evaluate(trajectoryTime);
            var bodyPose = new PositionRotation(body.position, body.rotation);
            var session = new Session(attackId, body, trajectory, speed, Time.frameCount)
            {
                TrajectoryTime = trajectoryTime,
                TrajectoryTimeEnd = Mathf.Min(Mathf.Max(trajectoryStart, trajectoryEnd), trajectory.Position.LastTime),
                PrevBodyPose = bodyPose,
                PrevTrajectoryPose = trajectoryPose,
                PrevColliderPose = TransformPose(trajectoryPose, bodyPose),
            };
            _sessions.Add(session);
            Collect.IfActive?.EventBgn("HitCollider.Active", (this.GetHashCode(),attackId));
            CheckHit(session, session.PrevColliderPose, session.PrevColliderPose);
        }

        public void DeactivateCollider(SpellPartCastId attackId, float trajectoryEnd)
        {
            if (!_initialized)
                return;

            bool allFinished = true;
            for (int idx = _sessions.Count - 1; idx >= 0; --idx)
            {
                var session = _sessions[idx];
                if (session.AttackId == attackId)
                {
                    _sessions.RemoveAt(idx);
                    DeactivateColliderInternal(session, trajectoryEnd);
                }
                else
                if (!session.Finished)
                    allFinished = false;
            }

            if (allFinished)
                enabled = false;
        }
        
        public HitColliderHandler Setup(HitColliderDelegate callback, Transform rootBone, DamageTypeDef damageType, GameObjectMatcher objectMatcher)
        {
            if (!HitCollider)
                HitCollider = GetComponent<Collider>();

            if (HitCollider.AssertIfNull(nameof(HitCollider)))
                return this;

            _objectMatcher = objectMatcher;
                
            enabled = false;
            HitCollider.enabled = false;
            HitCollider.isTrigger = true;
            Marker = this.TryGetMarker();

            _onAttackColliderHit = callback;
            _damageType = damageType;

            _colliderPose = new PositionRotation(
                rootBone.transform.InverseTransformPoint(transform.TransformPoint(HitCollider.GetCenter())),
                Quaternion.Inverse(rootBone.transform.rotation) * transform.rotation);
            
            _colliderWidth = Mathf.Max(Vector3.Scale(HitCollider.GetSize(), HitCollider.transform.lossyScale).MinComponent(), 0.1f);

            _initialized = true;

            return this;
        }

        public HitColliderHandler Dispose()
        {
            foreach (var session in _sessions)
                DeactivateColliderInternal(session, session.TrajectoryTimeEnd);
            _sessions.Clear();
            _onAttackColliderHit = null;
            return this;
        }
        
        private void DeactivateColliderInternal(Session session, float trajectoryEnd)
        {
            session.TrajectoryTimeEnd = Mathf.Min(Mathf.Max(session.TrajectoryTime, trajectoryEnd), session.Trajectory.Position.LastTime);
            Process(session, session.TrajectoryTimeEnd - session.TrajectoryTime, true);
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"HitCollider Finish | HitChecks:{session.HitChecksCount} Hits:{session.Hits.Count} [{string.Join(", ",session.Hits.Select(x => x.name))}] Attacker:{session.Body.name} AttackId:{session.AttackId}").Write();
            Collect.IfActive?.EventEnd((this.GetHashCode(),session.AttackId));
        }

        private void HitDetected(Session session, GameObject victimRoot, PositionRotation prevPose, PositionRotation pose, Vector3 direction)
        {
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            Transform hitObject;
            var hpa = victimRoot.GetComponentInChildren<IHitPointAdjuster>();
            if (hpa != null)
            {
                var (ho, hp) = hpa.AdjustHitPoint(pose.Position);
                hitObject = ho;
                pose = new PositionRotation(hp, pose.Rotation);
            }
            else
                hitObject = victimRoot.transform;
            // ReSharper disable Unity.PerformanceCriticalCodeInvocation
            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Hit Detected | Session:{session.AttackId} Victim:{victimRoot.transform.FullName()} Attacker:{transform.FullName()} HitObject:{hitObject.FullName()} Pose:{pose.Position} PrevPose:{prevPose.Position} Direction:{direction} Rotation:{rotation.eulerAngles}").Write();
            // ReSharper restore Unity.PerformanceCriticalCodeInvocation
            Collect.IfActive?.Event("HitCollider.HitDetected");
            var hitInfo = new HitInfo (point: pose.Position, rotation: rotation, victim: victimRoot, hitObject: hitObject, damageType: _damageType);
            _onAttackColliderHit?.Invoke(session.AttackId, hitInfo);
        }

        private void Update()
        {
            if (!_initialized)
                return;

            bool any = false;
            foreach (var session in _sessions)
            {
                if (session.StartFrame != Time.frameCount)
                    Process(session, Time.unscaledDeltaTime * session.Speed, false);
                if (!session.Finished)
                    any = true;
            }
            
            if (!any)
                enabled = false;
        }

        private void Process(Session session, float deltaTime, bool finished)
        {
            var nextTime = Mathf.Min(session.TrajectoryTime + deltaTime, session.TrajectoryTimeEnd);
            if (nextTime <= session.TrajectoryTime && !finished)
                return;
            
            float bodyTime = 0;
            var nextBodyPose = new PositionRotation(session.Body.position, session.Body.rotation);
            var prevBodySubPose = session.PrevBodyPose;
            // расстояние которое прошёл коллайдер вдоль траектории за время deltaTime разбиваем на несколько шагов, чтобы получить равномерное покрытие 
            var distanceStep = Mathf.Max(_colliderWidth * Constants.AttackConstants.HitDetectionColliderFactor, 0.05f);
            int counter = IterationsLimit;
            do
            {
                // определяем промежуток времени соответствующий перемещению длинной в один шаг 
                var subDeltaTime = Mathf.Max(session.Trajectory.Position.DeltaTimeByDistance(session.TrajectoryTime, distanceStep), 0.005f);
                // определяем перемещение тела персонажа за этот промежуток времени
                var nextBodySubPose = deltaTime > 0 ? PositionRotation.Lerp(session.PrevBodyPose, nextBodyPose, (bodyTime += subDeltaTime) / deltaTime) : nextBodyPose;
                session.TrajectoryTime = Mathf.Min(session.TrajectoryTime + subDeltaTime, nextTime);
                // определяем перемещение коллайдера вдоль траектории за текущий шаг
                var nextTrajectoryPose = session.Trajectory.Evaluate(session.TrajectoryTime);
                DrawTrajectoryStep(prevBodySubPose, session.PrevTrajectoryPose.Position, nextBodySubPose, nextTrajectoryPose.Position);
                // определяем положение коллайдера (его центра) в мире для текущего шага
                var nextColliderPose = TransformPose(nextTrajectoryPose, nextBodySubPose);
                // реальное расстояние между предыдущей и текущей позициями коллайдера может быть больше distanceStep
                // в силу движения body и неточности вычисления DeltaTimeByDistance. 
                // это расстояние так же разбиваем на шаги
                var distance = (nextColliderPose.Position - session.PrevColliderPose.Position).magnitude; 
                var steps = Mathf.Min(finished ? Mathf.CeilToInt(distance / distanceStep) : Mathf.FloorToInt(distance / distanceStep), IterationsLimit);
                var prevColliderSubPose = session.PrevColliderPose; 
                for (int s = 1; s <= steps; ++s)
                {
                    // проверяем коллизию на каждом шаге
                    var nextColliderSubPose = PositionRotation.Lerp(session.PrevColliderPose, nextColliderPose, s * distanceStep / distance);
                    CheckHit(session, prevColliderSubPose, nextColliderSubPose);
                    prevColliderSubPose = nextColliderSubPose;                    
                }                
                session.PrevColliderPose = prevColliderSubPose;
                session.PrevTrajectoryPose = nextTrajectoryPose;
                prevBodySubPose = nextBodySubPose;
            } while (session.TrajectoryTime < nextTime && --counter > 0);

            session.PrevBodyPose = nextBodyPose;
        }

        private PositionRotation TransformPose(PositionRotation trajectoryPose, PositionRotation bodyPose)
        {
            return new PositionRotation(
                bodyPose.Position + bodyPose.Rotation * (trajectoryPose.Position + trajectoryPose.Rotation * _colliderPose.Position),
                bodyPose.Rotation * trajectoryPose.Rotation * _colliderPose.Rotation
            );
        }

        private void CheckHit(Session session,PositionRotation prevPose, PositionRotation pose)
        {
            session.HitChecksCount++;
            int cols = OverlapCollider(pose.Position, pose.Rotation, CollidersBuffer);
            if (cols > 0)
            {
                for (int j = 0; j < cols; j++)
                {
                    var otherCollider = CollidersBuffer[j];
                    // ReSharper disable Unity.PerformanceCriticalCodeInvocation
                    if(Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Hit Check | Other:{otherCollider.transform.FullName()} This:{transform.FullName()} Session:{session.AttackId}").Write();
                    // ReSharper restore Unity.PerformanceCriticalCodeInvocation
                    if (!otherCollider.enabled || !otherCollider.gameObject.activeInHierarchy)
                        continue;
                    if (!_objectMatcher.IsMatch(otherCollider))
                        continue;
                    var otherRoot = otherCollider.gameObject.GetRoot();
                    if (otherRoot == _root)
                        continue;
                    if (!session.Hits.Add(otherRoot))
                        continue;

                    var direction = pose.TransformDirection(session.Trajectory.Position.Tangent(session.TrajectoryTime));
                    //var direction = pose.Position - prevPose.Position;
                    HitDetected(session, otherRoot, prevPose, pose, direction);
                    Drawer.Debug?.Collider(otherCollider, otherCollider.transform.position, otherCollider.transform.rotation, otherCollider.transform.lossyScale, Color.cyan);
                }

                Drawer.Debug?.Collider(HitCollider, pose.Position, pose.Rotation, HitCollider.transform.lossyScale, Color.red, true);
            }
            else
                Drawer.Debug?.Collider(HitCollider, pose.Position, pose.Rotation, HitCollider.transform.lossyScale, Color.cyan, true);
        }


        private int OverlapCollider(Vector3 center, Quaternion rotation, Collider[] colliders)
        {
            var scale = HitCollider.transform.lossyScale;

            switch (HitCollider)
            {
                case SphereCollider sphere:
                {
                    return Physics.OverlapSphereNonAlloc(center, sphere.radius * scale.MaxComponent(), colliders, _objectMatcher.Layers, QueryTriggers);
                }

                case CapsuleCollider capsule:
                {
                    // Попытка повторить влияние скейла на юнитёвый коллайдер:
                    // длинна меняется значением скейла по главной оси капсулы,
                    // радиус меняется максимальным значением скейла по перпендикулярным осям 
                    var radius = Vector3.Scale(capsule.GetCapsuleSides() * capsule.radius, scale).MaxComponent();
                    var axis = capsule.GetCapsuleAxis();
                    float eccentricity = Vector3.Scale(capsule.height * 0.5f * axis, scale).MaxComponent() - radius;
                    var offset = rotation * axis * eccentricity;
                    return Physics.OverlapCapsuleNonAlloc(center - offset, center + offset, radius, colliders, _objectMatcher.Layers, QueryTriggers);
                }

                case BoxCollider box:
                {
                    return Physics.OverlapBoxNonAlloc(center, Vector3.Scale(box.size * 0.5f, scale), colliders, rotation, _objectMatcher.Layers, QueryTriggers);
                }

                default:
                    throw new NotSupportedException($"{HitCollider.GetType()}");
            }
        }

        private void DrawTrajectoryStep(PositionRotation prevBodyPose, Vector3 prevPos, PositionRotation bodyPose, Vector3 pos)
        {
            if (Drawer.IsDebugEnabled)
            {
                prevPos = prevBodyPose.TransformPoint(prevPos);
                pos = bodyPose.TransformPoint(pos);
                Drawer.Debug.Line(prevPos, pos, Color.red);
                Drawer.Debug.Point(pos, Color.red, 0.05f);
            }
        }
        
        private class Session
        {
            public readonly SpellPartCastId AttackId;
            public readonly Transform Body;
            public readonly HashSet<GameObject> Hits = new HashSet<GameObject>();
            public readonly AnimationTrajectory Trajectory;
            public readonly int StartFrame;
            public readonly float Speed;
            public PositionRotation PrevColliderPose;
            public PositionRotation PrevTrajectoryPose;
            public PositionRotation PrevBodyPose;
            public float TrajectoryTime;
            public float TrajectoryTimeEnd;
            public int HitChecksCount;
            public bool Finished => TrajectoryTime >= TrajectoryTimeEnd;

            public Session(SpellPartCastId attackId, Transform body, AnimationTrajectory trajectory, float speed, int startFrame)
            {
                AttackId = attackId;
                Body = body;
                Trajectory = trajectory;
                StartFrame = startFrame;
                Speed = speed;
            }
        }
    }

    public readonly struct HitInfo
    {
        public readonly GameObject Victim; // __Корневой__ объект того, по чему ударили (обычно должен содержать EGO)
        public readonly Vector3 Point; // Точка удара в мировых координатах
        public readonly Quaternion Rotation; // Описывает направление удара
        public readonly Transform HitObject; // Под-объект CollidedObject'а ближайший к Position  
        public readonly Vector3 LocalPoint; // Точка удара в координатах HitObject'а
        public readonly Quaternion LocalRotation; // Описывает направление удара в СК HitObject
        public readonly DamageTypeDef DamageType;

        public HitInfo(GameObject victim, Transform hitObject, Vector3 point, Quaternion rotation, DamageTypeDef damageType)
        {
            Victim = victim;
            Point = point;
            Rotation = rotation;
            DamageType = damageType;
            HitObject = hitObject ? hitObject : victim.transform;
            LocalPoint = HitObject.InverseTransformPoint(point);
            LocalRotation = Quaternion.Inverse(HitObject.rotation) * rotation;
        }
    }
}