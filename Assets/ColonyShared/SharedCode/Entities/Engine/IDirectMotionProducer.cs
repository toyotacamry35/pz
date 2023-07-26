using System;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using SharedCode.Utils;

namespace SharedCode.Entities.Engine
{
    public interface IRotator {}

    public interface IMover {}
    
    public interface IDirectMotionProducer
    {
        IMover NoMovement();
        IMover AnimatorMovement(float factor);
        IMover CurveMovement(UnityRef<Curve> curve, Vector2 dir, float duration, float velocityFactor);
        IMover CurveMovement(UnityRef<Curve> curve, UnityRef<Curve> verticalCurve, Vector2 dir, float duration, float velocityFactor);
        IRotator NoRotation();
        IRotator DirectionFixed(Func<Vector3?> directionFn);
        IRotator DirectionWithSpeed(Func<Vector3?> directionFn, float speed);
        IRotator DirectionWithTime(Func<Vector3?> directionFn, float time);
        IRotator LookAtFixed(Func<Vector3?> targetFn);
        IRotator LookAtWithSpeed(Func<Vector3?> targetFn, float speed);
        IRotator LookAtWithTime(Func<Vector3?> targetFn, float time);
        void AddOrder(object causer, IMover mover, IRotator rotator);
        void RemoveOrder(object causer);
    }
}