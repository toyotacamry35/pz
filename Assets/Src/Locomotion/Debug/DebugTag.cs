using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Src.Locomotion
{
    public enum DebugTag
    {
        TimeStamp,

        VarsPosition,
        VarsVelocity,
        VarsOrientation,
        VarsAngularVelocity,

        BodyPosition,
        BodyVelocity,
        BodyOrientation,
        
        RealVelocity,
        
        Airborne,
        SlopeFactor,
        SlopeFactorAlongVelocity,
        AnimationState,
        NextAnimationState,
        Shift,
        IsFalling,
        FallHeight,
        DistanceToGround,
        IsDirectControl,
        Sticking,
        
        MoveAxes,
        GuideAxes,
        
        CollisionDetection,
        Depenetration,
        ColliderPosition,
        
        MovementFlags,
        StateMachineStateName,
        
        CheatMode,
        
        RelevanceLevel,
        NetworkSentPosition,
        NetworkSentVelocity,
        NetworkSentOrientation,
        NetworkSentReason,
        NetworkSentFrameId,
        // NetworkReceived:
        NetworkReceivedPosition,
        NetworkReceivedVelocity,
        NetworkReceivedOrientation,
        NetworkReceivedFrameId,
        NetworkPredictedPosition,
        NetworkPredictedVelocity,
        NetworkPredictedOrientation,

        // DamperTrail:
        DamperTrailBeforeDamp, //Target pos (fed into `MovePosition(..)`) w/o(before) damping
        DamperTrailDamped, //Same but damped

        DamperPositionDelta,
        DamperLinearVelocity,
        DamperOrientationDelta,
        DamperAngularSpeed,

        Destucker,
        
        NavMeshPosition,
        NavMeshOffLink,
        
        GroundRaycastStart,
        
        Contact0,
        Contact1,
        Contact2,
        Contact3,
        Contact4,
        Contact5,
        Contact6,
        Contact7,
        Contact8,
        Contact9,
        
        DrawColor,
        DrawSize,
        DrawPoint,
        DrawSphere,
        DrawBox,
    }

    public static class DebugTags
    {
        public static int Count = Enum.GetValues(typeof(DebugTag)).Cast<int>().Max();
        
        public static string[] Names = Enum.GetNames(typeof(DebugTag));

        public static DebugTag[] Contacts = Enumerable.Range((int)DebugTag.Contact0, DebugTag.Contact9 - DebugTag.Contact0).Cast<DebugTag>().ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Idx(this DebugTag tag) => (int) tag;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static DebugTag FromIdx(int idx) => (DebugTag) idx;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsContact(this DebugTag tag) => tag >= DebugTag.Contact0 && tag <= DebugTag.Contact9;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int ContactIdx(this DebugTag tag) => tag - DebugTag.Contact0;
    }
    
}