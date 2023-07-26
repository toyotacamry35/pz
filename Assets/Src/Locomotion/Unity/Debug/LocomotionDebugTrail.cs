using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using ColonyShared.SharedCode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using static Src.Locomotion.DebugTag;

namespace Src.Locomotion.Unity
{
    // #To_add_new_trail_user see all comments here with "##toAddNewTrail_X".
    // #HowToUse: Ctrl+PgUp/PgDown, Ctrl+"+"/"-"
    public class LocomotionDebugTrail : MonoBehaviour, ILocomotionDebugAgent
    {
        public static readonly int ModesCount = (int) Enum.GetValues(typeof(TrailModes)).Cast<TrailModes>().Max() + 1;

        [SerializeField, FormerlySerializedAs("Drawing")] public bool _drawing;
        [SerializeField, FormerlySerializedAs("Recording")] public bool _recording;
        [SerializeField] public TrailModes TrailMode = TrailModes.BodySpeed;
        [SerializeField] private int _historyLength = 100;
        [SerializeField] private float _minStep = 0.05f;
        private float _minStepSqrd;
        [SerializeField] private Material _material;
        [SerializeField] private Color _colorLow = Color.red;
        [SerializeField] private Color _colorHigh = Color.green;
        [SerializeField] private RangeGradient _speedGradient;
        [SerializeField] private RangeGradient _slopeGradient;
        [SerializeField] private RangeGradient _shiftGradient;
        [SerializeField] private RangeGradient _distanceToGroundGradient;
        [SerializeField] private StateColor[] _stateColor;
        [SerializeField] private StateColor[] _animatorStateColors;
        [SerializeField] private CameraEvent _placeInDrawQueue = CameraEvent.AfterEverything;

        private static StringBuilder _stringBuilder = new StringBuilder();
        private bool _initialized;
        private RingBuffer<Frame> _history;
        private Frame _currentFrame;
        private bool _updateTrail;
        private ComputeBuffer _computeBuffer;
        private CommandBuffer _commandBuffer;
        private bool _prevDrawing;
        private TrailModes _prevTrailMode;
        private Func<Camera> _gameCamera;
        private int _skipFrames;

        private void Awake()
        {
            _minStepSqrd = SharedHelpers.Sqr(_minStep);
            _recording = false;
            _drawing = false;
            enabled = false;
        }

        public void Initialize(Func<Camera> gameCamera)
        {
            _gameCamera = gameCamera;
            _history = null;
            _prevTrailMode = TrailMode;
            _initialized = true;
        }

        public bool Drawing
        {
            get => _drawing;
            set
            {
                _drawing = value;
                if (value)
                    enabled = true;
            }
        }

        public bool Recording
        {
            get => _recording;
            set => _recording = value;
        }
        
        public bool IsActive => Recording;

        public void Dispose()
        {
            _gameCamera = null;
            _initialized = false;
            _history = null;
        }

        public void SwitchToNextMode()
        {
            TrailMode = (TrailModes) (((int) TrailMode + 1) % ModesCount);
        }

        public void SwitchToPrevMode()
        {
            TrailMode = (TrailModes) (((int) TrailMode + ModesCount - 1) % ModesCount);
        }

        public void Clear()
        {
            _history = null;
            _updateTrail = true;
        }

        public int SkipFrames
        {
            get { return _skipFrames; }
            set { 
                value = Mathf.Clamp(value, 0, _historyLength - 1);
                if (_skipFrames != value)
                {
                    _skipFrames = value;
                    _updateTrail = true;
                }
            }
        }
        
        public string GetLastFrameString(TrailModes mode) => GetFrameString(GetLastFrame(), mode);

        public string GetLastFrameString()
        {
            var frame = GetLastFrame();
            _stringBuilder.Clear();
            for (var i = 0; i < ModesCount; ++i)
            {
                var mode = (TrailModes) i;
                _stringBuilder.Append(mode).Append(':').Append(GetFrameString(frame, mode)).Append(' ');
            }
            return _stringBuilder.ToString();
        }

        public DateTime GetLastFrameTimestamp() => GetLastFrame().Timestamp;
        
        public TimeSpan GetSkipedTime()
        {
            if (_history == null || _history.IsEmpty) return TimeSpan.Zero;
            var timestamp1 = _history.Front().Timestamp;
            var timestamp2 = GetLastFrame().Timestamp;
            return timestamp1 - timestamp2;
        }

        private Frame GetLastFrame()
        {
            return _history == null || _history.IsEmpty ? new Frame() : _skipFrames < _history.Count ? _history[_skipFrames] : _history.Back();
        }

        // ##toAddNewTrail_8 - Add 
        public void Add(DebugTag id, Value entry)
        {
            if (!_initialized || !Recording)
                return;

            switch (id)
            {
                case BodyPosition:
                    _currentFrame.BodyPosition = entry.Vector3.ToUnity();
                    break;
                case VarsPosition:
                    _currentFrame.VarsPosition = entry.Vector3.ToUnity();
                    break;
                case BodyVelocity:
                {
                    var vel = entry.LocomotionVector();
                    _currentFrame.BodySpeed = vel.Magnitude;
                    _currentFrame.BodyHorizontalSpeed = vel.Horizontal.magnitude;
                }
                    break;
                case VarsVelocity:
                {
                    var vel = entry.LocomotionVector();
                    _currentFrame.VarsSpeed = vel.Magnitude;
                    _currentFrame.VarsHorizontalSpeed = vel.Horizontal.magnitude;
                }
                    break;
                case RealVelocity:
                    _currentFrame.RealSpeed = entry.LocomotionVector().Magnitude;
                    _currentFrame.RealHorizontalSpeed = entry.LocomotionVector().Horizontal.magnitude;
                    break;
                case NavMeshPosition:
                    _currentFrame.NavMeshPosition = entry.Vector3.ToUnity();
                    break;
                case Airborne:
                    _currentFrame.Airborne = entry.Bool;
                    break;
                case GroundRaycastStart:
                    _currentFrame.GroundRaycastStart = entry.Vector3.ToUnity();
                    break;
                case SlopeFactor:
                    _currentFrame.Slope = entry.Float;
                    break;
                case StateMachineStateName:
                    _currentFrame.State = entry.String;
                    break;
                case DebugTag.AnimationState:
                    _currentFrame.AnimationState = entry.Int;
                    break;
                case NextAnimationState:
                    _currentFrame.NextAnimationState = entry.Int;
                    break;
                case Shift:
                    _currentFrame.Shift = entry.Vector3.ToUnity();
                    _currentFrame.HasShift = !_currentFrame.Shift.ApproximatelyEqual(Vector3.zero);
                    break;
                case IsFalling:
                    _currentFrame.IsFalling = entry.Bool;
                    break;
                case NetworkSentPosition:
                    _currentFrame.NetworkSent = true;
                    _currentFrame.NetworkSentPosition = entry.Vector3.ToUnity();
                    break;
                case NetworkReceivedPosition:
                    _currentFrame.NetworkReceived = true;
                    _currentFrame.NetworkReceivedPosition = entry.Vector3.ToUnity();
                    break;
                case NetworkPredictedPosition:
                    _currentFrame.NetworkPredictedPosition = entry.Vector3.ToUnity();
                    break;
                case ColliderPosition:
                    _currentFrame.ColliderPosition = entry.Vector3.ToUnity();
                    break;
                case NetworkSentFrameId: // Ok. Propagetion into next case is intended
                case NetworkReceivedFrameId:
                    _currentFrame.NetworkFrameId = entry.Long;
                    break;
                case MovementFlags:
                    _currentFrame.Flags = (LocomotionFlags)entry.Int;
                    break;
                case DistanceToGround:
                    _currentFrame.DistanceToGround = entry.Float;
                    break;
                case NavMeshOffLink:
                    _currentFrame.NavMeshOffLink = entry.Bool;
                    break;
                //case LocoConsts.DamperTrail:
                //    _currentFrame.OffsetToPosWithoutDamping = entry.Vector3.ToUnity();
                //    break;
                case DamperTrailBeforeDamp:
                    _currentFrame.TargetPosBeforeDamper = entry.Vector3.ToUnity();
                    break;
                case DamperTrailDamped:
                    _currentFrame.TargetPosDamped= entry.Vector3.ToUnity();
                    break;
                case IsDirectControl:
                    _currentFrame.IsDirectControl = entry.Bool;
                    break;
                case CollisionDetection:
                    _currentFrame.CollisionDetection = entry.Vector3.ToUnity();
                    break;
                case Depenetration:
                    _currentFrame.Depenetration = entry.Vector3.ToUnity();
                    break;
                case Sticking:
                    _currentFrame.Sticiking = entry.Bool;
                    break;
            }

            if (id.IsContact())
            {
                Array.Resize(ref _currentFrame.Contacts, _currentFrame.Contacts?.Length + 1 ?? 1);
                _currentFrame.Contacts[_currentFrame.Contacts.Length - 1] = entry.Vector3.ToUnity();
            }
        }

        public void BeginOfFrame()
        {
        }

        public void EndOfFrame()
        {
            if (_initialized && Recording)
            {
                if (_history == null)
                    _history = new RingBuffer<Frame>(_historyLength);

                if (_history.IsEmpty || !FrameEquals(_currentFrame,_history.Front()))
                {
                    _currentFrame.Timestamp = DateTime.UtcNow;
                    _history.PushFront(_currentFrame);
                    _updateTrail = true;
                }
                _currentFrame = new Frame();
            }
        }
        
        private void Update()
        {
            if (!_initialized)
                return;
            
            if (TrailMode != _prevTrailMode)
            {
                _prevTrailMode = TrailMode;
                _updateTrail = true;
            }

            if (Drawing != _prevDrawing)
            {
                _prevDrawing = Drawing;
                if(Drawing)
                    SetupDrawing();
                else
                    DisposeDrawing();
            }
                      
            if (_updateTrail)
            {
                _updateTrail = false;
                RebuildPointsBuffer();
                if (Drawing)
                    SetupDrawing();
            }

            if (!Drawing)
                enabled = false;
        }

        private void OnDestroy()
        {
            DisposeDrawing();
        }

        private void RebuildPointsBuffer()
        {
            if (_computeBuffer != null)
            {
                _computeBuffer.Dispose();
                _computeBuffer = null;
            }

            if (_history == null)
                return;
            
            var count = 0;
            for (int i = _skipFrames; i < _history.Count; ++i)
                count += GetFramePointsCount(_history[i], TrailMode);

            var points = new Point[count];
            for (int i = _skipFrames, k = 0; i < _history.Count; ++i)
            {
                var frame = _history[i];
                for (int j = 0, cnt = GetFramePointsCount(frame, TrailMode); j < cnt; ++j)
                {
                    points[k++] = new Point
                    {
                        Vertex = GetFramePosition(frame, TrailMode, j),
                        Color = GetFrameColor(frame, TrailMode, j)
                    };
                }
            }

            if (points.Length > 0)
            {
                _computeBuffer = new ComputeBuffer(points.Length, Marshal.SizeOf(typeof(Point)), ComputeBufferType.Default);
                _computeBuffer.SetData(points);
            }
        }

        private void SetupDrawing()
        {
            DisposeDrawing();
            var camera = _gameCamera?.Invoke();
            if (_computeBuffer != null && camera != null)
            {
                _material.SetBuffer("points", _computeBuffer);
                _commandBuffer = new CommandBuffer();
                _commandBuffer.DrawProcedural(Matrix4x4.identity, _material, 0, MeshTopology.Points, _computeBuffer.count);
                _commandBuffer.DrawProcedural(Matrix4x4.identity, _material, 1, MeshTopology.Points, _computeBuffer.count);
                camera.AddCommandBuffer(_placeInDrawQueue, _commandBuffer);
            }
        }

        private void DisposeDrawing()
        {
            if (_commandBuffer != null)
            {
                _gameCamera?.Invoke()?.RemoveCommandBuffer(_placeInDrawQueue, _commandBuffer);
                _commandBuffer.Dispose();
                _commandBuffer = null;
            }
        }

        // ##toAddNewTrail_7 - Set color of trail-point(s)
        private Color GetFrameColor(in Frame frame, TrailModes trailMode, int j)
        {
            switch (trailMode)
            {
                case TrailModes.BodySpeed:
                    return _speedGradient.Evaluate(frame.BodySpeed);
                case TrailModes.BodyHorizontalSpeed:
                    return _speedGradient.Evaluate(frame.BodyHorizontalSpeed);
                case TrailModes.VarsSpeed:
                    return _speedGradient.Evaluate(frame.VarsSpeed);
                case TrailModes.VarsHorizontalSpeed:
                    return _speedGradient.Evaluate(frame.VarsHorizontalSpeed);
                case TrailModes.RealSpeed:
                    return _speedGradient.Evaluate(frame.RealSpeed);
                case TrailModes.RealHorizontalSpeed:
                    return _speedGradient.Evaluate(frame.RealHorizontalSpeed);
                case TrailModes.Airborne:
                    return frame.Airborne ? _colorHigh : _colorLow;
                case TrailModes.GroundRaycast:
                    return frame.DistanceToGround < float.MaxValue ? _colorHigh : _colorLow;
                case TrailModes.Slope:
                    return frame.Slope > 0 ? _slopeGradient.Evaluate(frame.Slope) : Color.clear;
                case TrailModes.DistanceToGround:
                    return _distanceToGroundGradient.Evaluate(frame.DistanceToGround);
                case TrailModes.State:
                    foreach (var x in _stateColor)
                        if (frame.State != null && frame.State.EndsWith(x.State, StringComparison.OrdinalIgnoreCase))
                            return x.Color;
                    return Color.clear;
                case TrailModes.AnimatorState:
                    foreach (var x in _animatorStateColors)
                        if (x.State != null && Animator.StringToHash(x.State) == (j==0 ? frame.AnimationState : frame.NextAnimationState))
                            return x.Color;
                    return Color.clear;
                case TrailModes.Shift:
                    return j == 1 ? _shiftGradient.Evaluate(frame.Shift.magnitude) : new Color();
                case TrailModes.NetworkMaster:
                    return frame.NetworkSent ? _colorHigh : Color.clear;
                case TrailModes.NetworkSlave:
                    return j == 1 ? (frame.NetworkReceived ? _colorHigh : Color.clear) : new Color(_colorLow.r,_colorLow.g,_colorLow.b,0.5f);
                case TrailModes.IsFalling:
                    return frame.IsFalling ? _colorHigh : Color.clear;
                case TrailModes.Flags:
                    var val = (long)frame.Flags;
                    var ival = (val * 0xFFFFFF) / 0xFFFF;
                    return new Color32((byte)(ival & 0xFF), (byte)((ival>>8) & 0xFF), (byte)((ival>>16) & 0xFF), 0xFF);
                case TrailModes.NavMeshOffLink:
                    return frame.NavMeshOffLink ? _colorHigh : _colorLow;
                case TrailModes.Damper:
                    return j == 1 ? _colorHigh : _colorLow;
                case TrailModes.IsDirectControl:
                    return frame.IsDirectControl ? _colorHigh : _colorLow;
                case TrailModes.CollisionDetection:
                    return frame.CollisionDetection.sqrMagnitude > 0 ? (j == 0 ? _colorHigh : _colorLow) : Color.clear;
                case TrailModes.Depenetration:
                    return frame.Depenetration.sqrMagnitude > 0 ? (j == 0 ? _colorHigh : _colorLow) : Color.clear;
                case TrailModes.Sticking:
                    return frame.Sticiking ? _colorHigh : Color.clear;
                case TrailModes.InvSticking:
                    return !frame.Sticiking ? Color.clear : _colorHigh;
                case TrailModes.Contacts:
                    return j == 0 ? Color.gray : _colorHigh;
                default:
                    return _colorLow;
            }
        }

        // ##toAddNewTrail_6 - String trail data interpretation (is seen at Loco debug view)
        private String GetFrameString(in Frame frame, TrailModes trailMode)
        {
            switch (trailMode)
            {
                case TrailModes.VarsPosition:
                    return frame.VarsPosition.ToString("F2");
                case TrailModes.VarsSpeed:
                    return frame.VarsSpeed.ToString("F2");
                case TrailModes.VarsHorizontalSpeed:
                    return frame.VarsHorizontalSpeed.ToString("F2");
                case TrailModes.BodyPosition:
                    return frame.BodyPosition.ToString("F2");
                case TrailModes.BodySpeed:
                    return frame.BodySpeed.ToString("F2");
                case TrailModes.BodyHorizontalSpeed:
                    return frame.BodyHorizontalSpeed.ToString("F2");
                case TrailModes.RealSpeed:
                    return frame.RealSpeed.ToString("F2");
                case TrailModes.RealHorizontalSpeed:
                    return frame.RealHorizontalSpeed.ToString("F2");
                case TrailModes.Airborne:
                    return frame.Airborne.ToString();
                case TrailModes.GroundRaycast:
                    return frame.GroundRaycastStart.ToString();
                case TrailModes.Slope:
                    return !frame.Airborne && frame.Slope > 0 ? frame.Slope.ToString("F2") : string.Empty;
                case TrailModes.State:
                    return frame.State;
//                case TrailModes.AnimatorState:
//                    return $"{_animatorStateColors.FirstOrDefault(x => Animator.StringToHash(x.State) == frame.AnimationState).State} {_animatorStateColors.FirstOrDefault(x => Animator.StringToHash(x.State) == frame.NextAnimationState).State}";
                case TrailModes.Shift:
                    return frame.Shift.magnitude.ToString("F2");
                case TrailModes.NetworkMaster:
                    return frame.NetworkSent.ToString();
                case TrailModes.NetworkSlave:
                    return frame.NetworkReceived.ToString();
                case TrailModes.NetworkFrameId:
                    return frame.NetworkFrameId != 0 ? "#" + frame.NetworkFrameId : string.Empty;
                case TrailModes.IsFalling:
                    return frame.IsFalling.ToString();
                case TrailModes.Flags:
                    return frame.Flags.ToString();
                case TrailModes.DistanceToGround:
                    return frame.DistanceToGround.ToString("F2");
                case TrailModes.NavMeshPosition:
                    return frame.NavMeshPosition.ToString("F2");
                case TrailModes.NavMeshOffLink:
                    return frame.NavMeshOffLink.ToString();
                //case TrailModes.Damper:
                //    return frame.OffsetToPosWithoutDamping.magnitude.ToString("F2");
                case TrailModes.Damper:
                    //return frame.OffsetToPosWithoutDamping.magnitude.ToString("F2");
                    return (frame.TargetPosDamped - frame.TargetPosDamped).magnitude.ToString("F2");
                case TrailModes.IsDirectControl:
                    return frame.IsDirectControl.ToString();
                case TrailModes.CollisionDetection:
                    return frame.CollisionDetection.ToString("F2");
                case TrailModes.Depenetration:
                    return frame.Depenetration.ToString("F2");
                case TrailModes.Sticking:
                    return frame.Sticiking.ToString();
                default:
                    return string.Empty;
            }
        }

        // ##toAddNewTrail_5 - return num. of points curr. mode should draw every step
        private int GetFramePointsCount(in Frame frame, TrailModes trailMode)
        {
            switch (trailMode)
            {
                case TrailModes.Contacts:
                    return (frame.Contacts?.Length ?? 0) + 1;
                case TrailModes.Shift:
                    return frame.HasShift ? 2 : 1;
//                case TrailModes.AnimatorState:
//                    return 2;
                case TrailModes.NetworkMaster:
                    return 1;
                case TrailModes.NetworkSlave:
                    return 2;
                case TrailModes.Damper:
                    return 2;
                case TrailModes.CollisionDetection:
                    return frame.CollisionDetection.sqrMagnitude > 0 ? 2 : 1;
                case TrailModes.Depenetration:
                    return frame.Depenetration.sqrMagnitude > 0 ? 2 : 1;
                default:
                    return 1;
            }
        }

        // ##toAddNewTrail_4 - Getter for trail-point(s) position
        private Vector3 GetFramePosition(in Frame frame, TrailModes trailMode, int i)
        {
            switch (trailMode)
            {
                case TrailModes.Contacts:
                    return i == 0 ? frame.ColliderPosition : frame.Contacts[i - 1];
                case TrailModes.Shift:
                    return i == 1 ? frame.BodyPosition : frame.BodyPosition - frame.Shift;
//                case TrailModes.AnimatorState:
//                    return i == 0 ? frame.Position : frame.Position + Vector3.up * 0.05f;
                case TrailModes.NetworkMaster:
                    return frame.NetworkSent ? frame.NetworkSentPosition : frame.BodyPosition;
                case TrailModes.NetworkSlave:
                    return i == 1 ? (frame.NetworkReceived ? frame.NetworkReceivedPosition : frame.BodyPosition) : frame.NetworkPredictedPosition;
                case TrailModes.DistanceToGround:
                case TrailModes.ColliderPosition:
                    return frame.ColliderPosition;
                case TrailModes.GroundRaycast:
                    return frame.GroundRaycastStart;
                case TrailModes.NavMeshPosition:
                    return frame.NavMeshPosition;
                case TrailModes.VarsPosition:
                    return frame.VarsPosition;
                case TrailModes.VarsSpeed:
                case TrailModes.VarsHorizontalSpeed:
                    return frame.VarsPosition;
                case TrailModes.Damper:
                    //return i == 1 ? frame.Position : frame.Position + frame.OffsetToPosWithoutDamping;
                    return i == 1 ? frame.TargetPosDamped : frame.TargetPosBeforeDamper;
                case TrailModes.CollisionDetection:
                    return i == 0 ? frame.BodyPosition : frame.BodyPosition + frame.CollisionDetection;
                case TrailModes.Depenetration:
                    return i == 0 ? frame.BodyPosition : frame.BodyPosition + frame.Depenetration;
                default:
                    return frame.BodyPosition;
            }
        }


        // ##toAddNewTrail_3 - Add equals to previous frame check (to do not write same data twice)
        private bool FrameEquals(in Frame f1, in Frame f2)
        {
            return
                (f1.BodyPosition - f2.BodyPosition).sqrMagnitude < _minStepSqrd &&
                f1.BodySpeed.ApproximatelyEqual(f2.BodySpeed, 0.01f) &&
                f1.Airborne == f2.Airborne &&
                f1.GroundRaycastStart == f2.GroundRaycastStart &&
                f1.Shift.ApproximatelyEqual(f2.Shift, 0.01f) &&
                f1.Slope.ApproximatelyEqual(f2.Slope, 0.01f) &&
                f1.State == f2.State &&
                f1.AnimationState == f2.AnimationState &&
                f1.NextAnimationState == f2.NextAnimationState &&
                f1.NetworkSent == f2.NetworkSent &&
                f1.NetworkReceived == f2.NetworkReceived && 
                f1.Flags == f2.Flags &&
                f1.NavMeshOffLink == f2.NavMeshOffLink &&
                //(f1.OffsetToPosWithoutDamping == Vector3.zero || (f1.OffsetToPosWithoutDamping - f2.OffsetToPosWithoutDamping).sqrMagnitude < _minStepSqrd)
                (f1.TargetPosBeforeDamper - f2.TargetPosBeforeDamper).sqrMagnitude < _minStepSqrd &&
                ((f1.TargetPosDamped - f2.TargetPosDamped).sqrMagnitude < _minStepSqrd) &&
                f1.IsDirectControl == f2.IsDirectControl &&
                f1.CollisionDetection.ApproximatelyEqual(f2.CollisionDetection, 0.001f) &&
                f1.Depenetration.ApproximatelyEqual(f2.Depenetration, 0.001f) &&
                f1.Sticiking == f2.Sticiking 
              //  (f1.NetworkPredictedPosition - f2.NetworkPredictedPosition).sqrMagnitude < _minStep * _minStep 
                ;
        }

        // ##toAddNewTrail_2 - Add needed data to this trail-struct
        private struct Frame
        {
            public DateTime Timestamp;
            public Vector3 BodyPosition;
            public Vector3 VarsPosition;
            public Vector3 NavMeshPosition;
            public float BodySpeed;
            public float BodyHorizontalSpeed;
            public float VarsSpeed;
            public float VarsHorizontalSpeed;
            public float RealSpeed;
            public float RealHorizontalSpeed;
            public bool Airborne;
            public Vector3 GroundRaycastStart;
            public bool HasShift;
            public Vector3 Shift;
            public Vector3[] Contacts;
            public float Slope;
            public float DistanceToGround;
            public string State;
            public int AnimationState;
            public int NextAnimationState;
            public bool NetworkSent;
            public Vector3 NetworkSentPosition;
            public bool NetworkReceived;
            public Vector3 NetworkReceivedPosition;
            public Vector3 NetworkPredictedPosition;
            public Vector3 ColliderPosition;
            public bool IsFalling;
            public long NetworkFrameId;
            public LocomotionFlags Flags;
            public bool NavMeshOffLink;
            //public Vector3 OffsetToPosWithoutDamping;
            public Vector3 TargetPosBeforeDamper;
            public Vector3 TargetPosDamped;
            public bool IsDirectControl;
            public Vector3 CollisionDetection;
            public Vector3 Depenetration;
            public bool Sticiking;
        }

        // ##toAddNewTrail_1 - Add mode 
        public enum TrailModes
        {
            BodyPosition,
            VarsPosition,
            BodySpeed,
            BodyHorizontalSpeed,
            VarsSpeed,
            VarsHorizontalSpeed,
            RealSpeed,
            RealHorizontalSpeed,
            State,
            Flags,
            Airborne,
            DistanceToGround,
            Slope,
            GroundRaycast,
            Contacts,
            AnimatorState,
            Shift,
            NetworkMaster,
            NetworkSlave,
            IsFalling,
            ColliderPosition,
            NavMeshPosition,
            NetworkFrameId,
            NavMeshOffLink,
            Damper,
            IsDirectControl,
            CollisionDetection,
            Depenetration,
            Sticking,
            InvSticking
        }

        [Serializable]
        private struct RangeGradient
        {
            public Gradient Gradient;
            public float Min;
            public float Max;

            public Color Evaluate(float v)
            {
                return Gradient.Evaluate(Mathf.InverseLerp(Min, Max, v));
            }
        }

        [Serializable]
        private struct StateColor
        {
            public string State;
            public Color Color;
        }
        
        private struct Point
        {
            [UsedImplicitly] public Vector3 Vertex;
            [UsedImplicitly] public Color Color;
        }
   }
}