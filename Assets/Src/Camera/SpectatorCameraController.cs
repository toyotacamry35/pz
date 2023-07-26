using System;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

namespace Assets.Src.Camera
{
    [SaveDuringPlay]
    public class SpectatorCameraController : MonoBehaviour
    {
        public CinemachineMixingCamera MixingCamera;
        public CinemachineVirtualCameraBase PovCamera;
        public CinemachineVirtualCameraBase OrbitalCamera;
        public CinemachineVirtualCameraBase TrackedPovCamera;
        public KeyCode PovModeKey = KeyCode.F6;
        public KeyCode OrbitalModeKey = KeyCode.F7;
        public KeyCode TrackedPovModeKey = KeyCode.F8;
        
        private static readonly KeyCode[] NumKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };
       
        enum Mode { POV, Orbital, TrackedPOV }

        private SpectatorFreeFlyTransposer PovCameraTransposer => PovCamera.GetComponentInChildren<SpectatorFreeFlyTransposer>();
        
        private SpectatorFreeLookComposer PovCameraComposer => PovCamera.GetComponentInChildren<SpectatorFreeLookComposer>();
        
        private SpectatorSphericalTransposer OrbitalCameraTransposer => OrbitalCamera.GetComponentInChildren<SpectatorSphericalTransposer>();
        
        private CinemachineComposer OrbitalCameraComposer => OrbitalCamera.GetComponentInChildren<CinemachineComposer>();

        private CinemachineTrackedDolly TrackedPovCameraTransposer => TrackedPovCamera.GetComponentInChildren<CinemachineTrackedDolly>();

        private SpectatorFreeLookComposer TrackedPovCameraComposer => TrackedPovCamera.GetComponentInChildren<SpectatorFreeLookComposer>();

        private Vector3 CurentCameraPosition => GameCamera.Camera?.transform.position ?? Vector3.zero;

        private Quaternion CurentCameraOrientation => GameCamera.Camera?.transform.rotation ?? Quaternion.identity;
        
        private Mode _mode = Mode.POV;
        private CinemachinePathBase[] _tracks;
        private CinemachinePathBase _lastTrack;
        private Transform _lastTarget;
        private float _lastTrackPosition;
 
        public Vector3 Position
        {
            set
            {
                switch (_mode)
                {
                    case Mode.POV:
                        PovCameraTransposer.WorldPosition = value;
                        break;
                    case Mode.Orbital:
                        OrbitalCameraTransposer.WorldPosition = value;
                        break;
                    case Mode.TrackedPOV:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public Vector2 Rotation
        {
            set
            {
                switch (_mode)
                {
                    case Mode.POV:
                    {
                        var ctrl = PovCameraComposer;
                        ctrl.Yaw = value.y;
                        ctrl.Pitch = value.x;
                    }
                        break;
                    case Mode.Orbital:
                        break;
                    case Mode.TrackedPOV:
                    {
                        var ctrl = TrackedPovCameraComposer;
                        ctrl.Yaw = value.y;
                        ctrl.Pitch = value.x;
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        void Update()
        {
            var newMode = _mode;

            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            var modifiers = shift || ctrl || alt;
            if (!modifiers)
            {
                if (Input.GetKeyDown(PovModeKey))
                    newMode = Mode.POV;

                if (Input.GetKeyDown(OrbitalModeKey))
                    newMode = Mode.Orbital;

                if (Input.GetKeyDown(TrackedPovModeKey))
                    newMode = Mode.TrackedPOV;
            }
            SwitchMode(newMode);

            for (int i = 0; i < NumKeys.Length; ++i)
                if (Input.GetKeyDown(NumKeys[i]))
                {
                    if (alt)
                        SelectSpeed(i);
                    else if (_mode == Mode.TrackedPOV && _tracks != null && _tracks.Length > 0)
                    {
                        var tackIdx = i < _tracks.Length ? i : _tracks.Length - 1; 
                        SetPath(TrackedPovCameraTransposer, _lastTrack = _tracks[tackIdx], _lastTrackPosition = 0);
                    }
                }
        }

        private void SwitchMode(Mode newMode)
        {
            if (newMode != _mode)
            {
                switch (_mode)
                {
                    case Mode.POV:
                        SwitchFromPovMode();
                        break;
                    case Mode.Orbital:
                        SwitchFromOrbitalMode();
                        break;
                    case Mode.TrackedPOV:
                        SwitchFromTrackedPovMode();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (newMode)
                {
                    case Mode.POV:
                        SwitchToPovMode();
                        break;
                    case Mode.Orbital:
                        SwitchToOrbitalMode();
                        break;
                    case Mode.TrackedPOV:
                        SwitchToTrackedPovMode();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void SwitchToPovMode()
        {
            PovCameraTransposer.WorldPosition = CurentCameraPosition;
            var currOrientation = CurentCameraOrientation;
            PovCameraComposer.Yaw = currOrientation.eulerAngles.y;
            PovCameraComposer.Pitch = currOrientation.eulerAngles.x;
            ActivateCamera(PovCamera);
            _mode = Mode.POV;
        }

        private void SwitchFromPovMode()
        {
        }

        private void SwitchToOrbitalMode()
        {
            var currPosition = CurentCameraPosition;
            var currOrientation = CurentCameraOrientation;
            var forward = new Ray( currPosition, currOrientation * Vector3.forward * OrbitalCameraTransposer.MaxDistance);
            RaycastHit hit;
            if (Physics.Raycast(forward, out hit))
            {
                var offset = hit.transform.InverseTransformPoint(hit.point);
                OrbitalCamera.Follow = OrbitalCamera.LookAt = hit.transform;
                OrbitalCameraComposer.m_TrackedObjectOffset = offset;
                OrbitalCameraTransposer.m_FollowOffset = offset;
                OrbitalCameraTransposer.WorldPosition = currPosition;
                ActivateCamera(OrbitalCamera);
                _mode = Mode.Orbital;
            }
            else if(_lastTarget)
            {
                OrbitalCamera.Follow = OrbitalCamera.LookAt = _lastTarget;
                ActivateCamera(OrbitalCamera);
                _mode = Mode.Orbital;
            }
        }

        private void SwitchFromOrbitalMode()
        {
            _lastTarget = OrbitalCamera.LookAt; 
            OrbitalCamera.Follow = null;
            OrbitalCamera.LookAt = null;
        }

        private void SwitchToTrackedPovMode()
        {
            _tracks =  FindObjectsOfType<CinemachinePathBase>().Where(x => x.isActiveAndEnabled).OrderBy(x => x.name).ToArray();
            if (_lastTrack == null)
            {
                _lastTrack = _tracks.FirstOrDefault();
                _lastTrackPosition = 0;
            }
            if (_lastTrack != null)
            {
                var currOrientation = CurentCameraOrientation;
                TrackedPovCameraComposer.Yaw = currOrientation.eulerAngles.y;
                TrackedPovCameraComposer.Pitch = currOrientation.eulerAngles.x;
                SetPath(TrackedPovCameraTransposer, _lastTrack, _lastTrackPosition);
                ActivateCamera(TrackedPovCamera);
                _mode = Mode.TrackedPOV;
            }
        }

        private void SwitchFromTrackedPovMode()
        {
            _lastTrack = TrackedPovCameraTransposer.m_Path;
            _lastTrackPosition = TrackedPovCameraTransposer.m_PathPosition;
        }

        private void ActivateCamera(CinemachineVirtualCameraBase cam)
        {
            var idx = MixingCamera.ChildCameras.IndexOf(cam);
            Assert.AreNotEqual(idx, -1);
            for (int i = 0; i < MixingCamera.ChildCameras.Length; ++i)
            {
                MixingCamera.SetWeight(i, i == idx ? 1 : 0);
             //   MixingCamera.ChildCameras[i].enabled = i == idx;
            }
        }

        private static void SetPath(CinemachineTrackedDolly transposer, CinemachinePathBase path, float position)
        {
            transposer.m_Path = path;
            transposer.m_PathPosition = position;
            var stats = transposer.VirtualCamera.State;
            transposer.MutateCameraState(ref stats, -1);
        }
        
        private void SelectSpeed(int speedIndex)
        {
            switch (_mode)
            {
                case Mode.POV:
                    PovCameraTransposer.SelectSpeed(speedIndex);
                    break;
                case Mode.Orbital:
                    break;
                case Mode.TrackedPOV:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
