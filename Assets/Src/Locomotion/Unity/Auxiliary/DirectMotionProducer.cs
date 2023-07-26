using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.Engine;
using SharedCode.Utils.DebugCollector;
using UnityEngine;
using UnityEngine.Assertions;
using SVector3 = SharedCode.Utils.Vector3;
using SVector2 = SharedCode.Utils.Vector2;

namespace Src.Locomotion.Unity
{
    public class DirectMotionProducer : MonoBehaviour, ILocomotionUpdateable, IDirectMotionProducer
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();    
        
        private List<(OrderOperation Operation, Order Order, object Causer)> _operations = new List<(OrderOperation,Order,object)>();
        private List<(OrderOperation Operation, Order Order, object Causer)> _operationsBack = new List<(OrderOperation,Order,object)>();
        private readonly object _lock = new object();
        private readonly List<Order> _orders = new List<Order>();
        private readonly List<OrderState> _states = new List<OrderState>();
        Animator anim;
        private Animator _animator
        {
            get
            {
                if (_animatorGetter != null)
                {
                    anim = _animatorGetter(); 
                    if (anim != null) 
                        _animatorGetter = null;
                } 
                return anim;
            }
            set => anim = value;
        }
        private Func<Animator> _animatorGetter;
        private ILocomotionInputReceiver _receiver;
        private ILocomotionBody _body;
        private float _time;
        private Guid _entityId;

        public void Init(Func<Animator> animatorGetter, ILocomotionBody body, ILocomotionInputReceiver receiver, Guid entityId)
        {
            Init((Animator)null, body, receiver, entityId); 
            _animatorGetter = animatorGetter;
        }
        public void Init(Animator animator, ILocomotionBody body, ILocomotionInputReceiver receiver, Guid entityId)
        {
            Assert.IsNotNull(body, nameof(body));
            Assert.IsNotNull(receiver, nameof(receiver));

            Clean();
            _receiver = receiver;
            _body = body;
            _animator = animator;
            _entityId = entityId;
        }

        public void Clean()
        {
            lock (_lock)
            {
                _operations.Clear();
                _operationsBack.Clear();
            }
            _orders.Clear();
            _states.Clear();
        }

        public static DirectMotionProducer Create(Transform parent, Animator animator, ILocomotionBody body, ILocomotionInputReceiver receiver, Guid entityId)
        {
            var directRotationProducer = parent.gameObject.AddComponent<DirectMotionProducer>(); ///PZ-2020.05.OPTIMIZ: иметь бы компонент сразу, НО это только для AuthCl. Так что по-другому нельзя
            directRotationProducer.Init(animator, body, receiver, entityId);                     /// + посмотреть с т.зр., что initServer теперь каждые 5 сек. зовётся и на клиенте, то, может что-то ещё оптимизировать
            return directRotationProducer;
        }
        
        public static void RemoveFrom(Transform parent)
        {
            var directRotationProducer = parent.gameObject.GetComponent<DirectMotionProducer>();
            GameObject.Destroy(directRotationProducer);
        }

        public IMover NoMovement() => new NullMover();

        public IMover AnimatorMovement(float factor) => new AnimatorMover(factor);

        public IMover CurveMovement(UnityRef<Curve> curve, SVector2 dir, float timeFactor, float velocityFactor) => new CurveMover(curve, dir, timeFactor, velocityFactor);

        public IMover CurveMovement(UnityRef<Curve> curve, UnityRef<Curve> verticalCurve, SVector2 dir, float timeFactor, float velocityFactor) => new CurveMover(curve, verticalCurve, dir, timeFactor, velocityFactor);
        
        public IRotator NoRotation() => new NullRotator();

        public IRotator DirectionFixed(Func<SVector3?> directionFn) => new DirectionRotator(directionFn, new NullRotationInterpolator());
        
        public IRotator DirectionWithSpeed(Func<SVector3?> directionFn, float speed) => new DirectionRotator(directionFn, new FixedSpeedRotationInterpolator(speed));

        public IRotator DirectionWithTime(Func<SVector3?> directionFn, float time) => new DirectionRotator(directionFn, new FixedTimeRotationInterpolator(time));

        public IRotator LookAtFixed(Func<SVector3?> targetFn) => new LookAtRotator(targetFn, new NullRotationInterpolator());

        public IRotator LookAtWithSpeed(Func<SVector3?> targetFn, float speed) => new LookAtRotator(targetFn, new FixedSpeedRotationInterpolator(speed));

        public IRotator LookAtWithTime(Func<SVector3?> targetFn, float time) => new LookAtRotator(targetFn, new FixedTimeRotationInterpolator(time));
        
        public void AddOrder(object causer, IMover mover, IRotator rotator)
        {
            Assert.IsNotNull(causer, nameof(causer));
            lock (_lock)
                _operations.Add((OrderOperation.Add, new Order(causer, (IExecutableMover)mover, (IExecutableRotator)rotator), causer));
        }

        public void RemoveOrder(object causer)
        {
            Assert.IsNotNull(causer, nameof(causer));
            lock (_lock)
                _operations.Add((OrderOperation.Remove, default, causer));
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        void ILocomotionUpdateable.Update(float dt)
        {
            if (!_animator)
                return;
            
            lock (_lock)
            {
                (_operations, _operationsBack) = (_operationsBack, _operations);
                _operations.Clear();
            }

            foreach (var (operation, order, causer) in _operationsBack)
            {
                switch (operation)
                {
                    case OrderOperation.Add:
                        if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add Order | Cuser:{causer} Mover:{order.Mover} Rotator:{order.Rotator}").Write();
                        Collect.IfActive?.EventBgn("DirectMotionProducer", _entityId, causer);
                        _orders.Add(order);
                        _states.Add(new OrderState(0));
                        break;
                    case OrderOperation.Remove:
                        int idx;
                        for(idx = _orders.Count - 1; idx >= 0; --idx)
                            if (_orders[idx].Causer.Equals(causer))
                                break;
                        if (idx != -1)
                        {
                            if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Remove Order | Cuser:{causer} Mover:{_orders[idx].Mover} Rotator:{_orders[idx].Rotator}").Write();
                            Collect.IfActive?.EventEnd(causer);
                            _orders.RemoveAt(idx);
                            _states.RemoveAt(idx);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            var baseCtx = new OrderContext
            {
                Body = _body,
                Animator = _animator,
                DeltaTime = dt
            };

            _time += dt;
            var needApplyVelocity = false;
            var needApplyOrientation = false;
            var finalVelocity = LocomotionVector.Zero;
            var finalOrientation = float.NaN;
            for (int i = _orders.Count - 1;  i >= 0; --i)
            {
                var order = _orders[i];
                var state = _states[i]; 
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (state.TimeAtStart == -1)
                {
                    state.TimeAtStart = _time;
                    state.OrientationAtStart = _body.Orientation;
                    _states[i] = state;
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"OrientationAtStart:{_body.Orientation}").Write();
                }
                var ctx = baseCtx;
                ctx.Time = _time - state.TimeAtStart;
                ctx.OrientationAtStart = state.OrientationAtStart;
                var moverIsAlive = false;
                if(order.Mover != null)
                {
                    moverIsAlive = order.Mover.Execute(out var velocity, ctx);
                    if (!needApplyVelocity && (moverIsAlive || !ReferenceEquals(order.Causer, null)))
                    {
                        finalVelocity = velocity;
                        needApplyVelocity = true;
                    }
                }
                var rotatorIsAlive = false;
                if (order.Rotator != null)
                {
                    rotatorIsAlive = order.Rotator.Execute(out float orientation, ctx);
                    if (!needApplyOrientation && (rotatorIsAlive || !ReferenceEquals(order.Causer, null)))
                    {
                        if(Logger.IsDebugEnabled && Mathf.Abs(orientation) < 0.01f) Logger.IfDebug()?.Message($"Orientation is Zero | Rotator:{order.Rotator} Time:{ctx.Time} OrientationAtStat:{ctx.OrientationAtStart}").Write();
                        finalOrientation = orientation;
                        needApplyOrientation = true;
                    }
                }

                if (!moverIsAlive && !rotatorIsAlive && ReferenceEquals(order.Causer, null))
                {
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Order Expired | Mover:{order.Mover} Rotator:{order.Rotator}").Write();
                    Collect.IfActive?.EventEnd(order.Causer);
                    _orders.RemoveAt(i);
                    _states.RemoveAt(i);
                }
            }

            if (needApplyVelocity || needApplyOrientation)
            {
                _receiver.SetInput(CommonInputs.Direct, true);
                if(needApplyVelocity)
                {
                	_receiver.SetInput(CommonInputs.DirectVelocity, finalVelocity.Horizontal);
               		_receiver.SetInput(CommonInputs.DirectVelocityVertical, finalVelocity.Vertical);
                }   
                if(needApplyOrientation)
                {
                	_receiver.SetInput(CommonInputs.DirectOrientation, finalOrientation);
                }
                else
                {
                    _receiver.SetInput(CommonInputs.DirectOrientation, _body.Orientation);
                    if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"No Rotator | Orientation:{_body.Orientation}").Write();
                }
            }
        }

        private interface IExecutableMover : IMover
        {
            bool Execute(out LocomotionVector velocity, OrderContext ctx);
        }

        private interface IExecutableRotator : IRotator
        {
            bool Execute(out float orientation, OrderContext ctx);
        }
        
        private interface IRotationInterpolator
        {
            bool Interpolate(ref float currentOrientation, float startOrientation, float targetOrientation, float time, float dt);
        }

        private class NullMover : IExecutableMover
        {
            public bool Execute(out LocomotionVector velocity, OrderContext ctx)
            {
                velocity = LocomotionVector.Zero;
                return false;
            }
            
            public override string ToString() => $"{nameof(NullMover)}";
        }

        private class AnimatorMover : IExecutableMover
        {
            private readonly float _velocityFactor;

            public AnimatorMover(float velocityFactor)
            {
                _velocityFactor = velocityFactor;
            }

            public bool Execute(out LocomotionVector velocity, OrderContext ctx)
            {
                velocity = LocomotionHelpers.WorldToLocomotionVector(ctx.Animator.velocity) * _velocityFactor;
                return false;
            }
            
            public override string ToString() => $"{nameof(AnimatorMover)} VelocityFactor:{_velocityFactor}";
        }

        private class CurveMover : IExecutableMover
        {
            private readonly UnityRef<Curve> _curveRef;
            private readonly UnityRef<Curve> _verticalCurveRef;
            private readonly SharedCode.Utils.Vector2 _dir;
            private readonly float _duration;
            private readonly float _velocityFactor;
            private Curve _curve;
            private Curve _verticalCurve;
            private float _timeFactor = 1;
            private float _verticalTimeFactor = 1;
            private bool _firstTime = true;

            public CurveMover(UnityRef<Curve> curve, SharedCode.Utils.Vector2 dir, float duration, float velocityFactor)
            {
                _curveRef = curve;
                _dir = dir;
                _duration = duration;
                _velocityFactor = velocityFactor;
            }

            public CurveMover(UnityRef<Curve> curve, UnityRef<Curve> verticalCurve, SharedCode.Utils.Vector2 dir, float duration, float velocityFactor)
            {
                _curveRef = curve;
                _verticalCurveRef = verticalCurve;
                _dir = dir;
                _duration = duration;
                _velocityFactor = velocityFactor;
            }
            
            public bool Execute(out LocomotionVector velocity, OrderContext ctx)
            {
                if (_firstTime)
                {
                    _firstTime = false;
                    _curve = _curveRef?.Target;
                    if (!_curve) 
                        throw new NullReferenceException("_curveRef is null");
                    if (_curve && _duration > 0)
                        _timeFactor = _curve.LastTime / Math.Max(_duration, 0.001f);
                    _verticalCurve = _verticalCurveRef?.Target;
                    if (_verticalCurve && _duration > 0)
                        _verticalTimeFactor = _verticalCurve.LastTime / Math.Max(_duration, 0.001f);
                }

                SVector2 velocityHorizontal = SVector2.zero;
                if (_curve)
                    velocityHorizontal = LocomotionHelpers.TransformMovementInputAxes(_dir, ctx.Body.Forward) * _curve.Evaluate(ctx.Time * _timeFactor) * _timeFactor * _velocityFactor;
                float velocityVertical = 0;
                if (_verticalCurve)
                    velocityVertical = _verticalCurve.Evaluate(ctx.Time * _verticalTimeFactor) * _verticalTimeFactor * _velocityFactor;
                velocity = new LocomotionVector(velocityHorizontal, velocityVertical);
                return false;
            }

            public override string ToString() => $"{nameof(CurveMover)}:[ Curve:{_curveRef} Dir:{_dir} TimeFactor:{_timeFactor} VelocityFactor:{_velocityFactor}]";
        }
        
        private class NullRotator : IExecutableRotator
        {
            public bool Execute(out float orientation, OrderContext ctx)
            {
                orientation = ctx.Body.Orientation;
                return false;
            }
            
            public override string ToString() => $"{nameof(NullRotator)}";
        }
        
        private class LookAtRotator : IExecutableRotator
        {
            private readonly Func<SVector3?> _targetFn;
            private readonly IRotationInterpolator _interpolator;

            public LookAtRotator(Func<SVector3?> targetFn, IRotationInterpolator interpolator)
            {
                _targetFn = targetFn;
                _interpolator = interpolator;
            }

            public bool Execute(out float orientation, OrderContext ctx)
            {
                orientation = ctx.Body.Orientation;
                var target = _targetFn?.Invoke();
                if (!target.HasValue)
                    return false;
                var point = LocomotionHelpers.WorldToLocomotionVector(target.Value);
                var dir = point - ctx.Body.Position;
                var targetOrientation = Mathf.Atan2(dir.Horizontal.y, dir.Horizontal.x);
                return _interpolator.Interpolate(ref orientation, ctx.OrientationAtStart, targetOrientation, ctx.Time, ctx.DeltaTime);
            }
            
            public override string ToString() => $"{nameof(LookAtRotator)}:[ Target:{_targetFn} ]";
        }
        
        private class DirectionRotator : IExecutableRotator
        {
            private readonly Func<SVector3?> _directionFn;
            private readonly IRotationInterpolator _rotationInterpolator;

            public DirectionRotator(Func<SVector3?> directionFn, IRotationInterpolator rotationInterpolator)
            {
                _directionFn = directionFn;
                _rotationInterpolator = rotationInterpolator;
            }

            public bool Execute(out float orientation, OrderContext ctx)
            {
                orientation = ctx.Body.Orientation;
                var direction = _directionFn?.Invoke(); 
                if (direction == null)
                    return _directionFn == null;
                var dir = LocomotionHelpers.WorldToLocomotionVector(direction.Value);
                var targetOrientation = Mathf.Atan2(dir.Horizontal.y, dir.Horizontal.x);
                return _rotationInterpolator.Interpolate(ref orientation, ctx.OrientationAtStart, targetOrientation, ctx.Time, ctx.DeltaTime);
            }
        }
        
        private class FixedSpeedRotationInterpolator : IRotationInterpolator
        {
            private readonly float _speed;
            
            public FixedSpeedRotationInterpolator(float speed)
            {
                _speed = speed;
            }
            
            public bool Interpolate(ref float currentOrientation, float startOrientation, float targetOrientation, float time, float dt)
            {
                currentOrientation = currentOrientation.MoveTowardsAngleRad(targetOrientation, _speed * dt);
                return !Mathf.Approximately(currentOrientation, targetOrientation);
            }
        }

        private class FixedTimeRotationInterpolator : IRotationInterpolator
        {
            private readonly float _duration;
            
            public FixedTimeRotationInterpolator(float duration)
            {
                _duration = duration;
            }
            
            // ReSharper disable once RedundantAssignment
            public bool Interpolate(ref float currentOrientation, float startOrientation, float targetOrientation, float time, float dt)
            {
                var t = Mathf.Approximately(_duration, 0) ? time / _duration : 1;
                currentOrientation = SharedHelpers.LerpAngle(startOrientation, targetOrientation, t);
                return _duration >= 0 ? t < 1 : t > 0;
            }
        }
        
        private class NullRotationInterpolator : IRotationInterpolator
        {
            // ReSharper disable once RedundantAssignment
            public bool Interpolate(ref float currentOrientation, float startOrientation, float targetOrientation, float time, float dt)
            {
                currentOrientation = targetOrientation;
                return false;
            }
        }
        
        private readonly struct Order
        {
            public readonly object Causer;
            public readonly IExecutableMover Mover;
            public readonly IExecutableRotator Rotator;
            
            public Order(object causer, IExecutableMover mover, IExecutableRotator rotator)
            {
                Causer = causer;
                Mover = mover;
                Rotator = rotator;
            }
        }

        private struct OrderState
        {
            public float OrientationAtStart;
            public float TimeAtStart;
            // ReSharper disable once UnusedParameter.Local
            public OrderState(int _) 
            {
                TimeAtStart = -1;
                OrientationAtStart = 0;
            }
        }

        private struct OrderContext
        {
            public ILocomotionBody Body;
            public Animator Animator;
            public float OrientationAtStart;
            public float Time; // Время от начала order'а
            public float DeltaTime;
        }

        private enum OrderOperation
        {
            Add, Remove
        }
    }
}