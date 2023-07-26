using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Cinemachine;
using Cinemachine.Utility;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Camera;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Src.Camera
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Cinemachine/Base Camera Rig")]
    public class BaseCameraRig : CinemachineVirtualCameraBase, ICameraRig, ICameraWithFollow, ICameraWithLookAt
    {        
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(Camera));
        
        [HideInInspector, NoSaveDuringPlay] 
        public ChildCamera[] Cameras;
        [CinemachineBlendDefinitionProperty] 
        public CinemachineBlendDefinition DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);
        public CinemachineBlenderSettings CustomBlends;
        public bool ShowDebugText;

        private readonly List<KeyValuePair<object, CameraDef>> _stack = new List<KeyValuePair<object, CameraDef>>();
        private CameraState _state = CameraState.Default;
        private CinemachineBlend _activeBlend;
        private Transform _follow;
        private Transform _lookAt;

        public CameraDef ActiveCamera { get; private set; } 
        
        public void ActivateCamera(CameraDef tag, object causer)
        {
            if(!Array.Exists(Cameras, x => x.Tag.Target == tag))
                Logger.IfError()?.Message($"No camera {tag.____GetDebugAddress()} on {gameObject}").Write();
            _stack.Add(new KeyValuePair<object, CameraDef>(causer, tag));
        }

        public void DeactivateCamera(CameraDef tag, object causer)
        {
            var idx = _stack.FindLastIndex(x => x.Key == causer && x.Value == tag);
            if (idx != -1)
                _stack.RemoveAt(idx);
        }

        public ICinemachineCamera LiveChild { get; private set; }

        public CinemachineVirtualCameraBase[] ChildCameras => Cameras.Select(x => x.Camera).ToArray();

        public override CameraState State => _state;

        public override Transform LookAt { get { return null; } set { SetupLookAt(value); } }

        public override Transform Follow { get { return null; } set { SetupFollow(value); } }

        public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
        {
            return vcam == LiveChild || _activeBlend != null && (vcam == _activeBlend.CamA || vcam == _activeBlend.CamB);
        }

        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            foreach (var childCamera in Cameras)
                childCamera.Camera.OnTargetObjectWarped(target, positionDelta);
            base.OnTargetObjectWarped(target, positionDelta);
        }

        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            if (!PreviousStateIsValid)
                deltaTime = -1f;
            var child = ChooseCurrentCamera();
            var childCamera = child?.Camera; 
            if (childCamera != null && !childCamera.gameObject.activeInHierarchy)
            {
                childCamera.gameObject.SetActive(true);
                childCamera.UpdateCameraState(worldUp, deltaTime);
            }

            var liveChild = LiveChild;
            LiveChild = childCamera;
            ActiveCamera = child?.Tag.Target;

            if (LiveChild != null && liveChild != LiveChild)
            {
                _activeBlend = CreateBlend(liveChild, LiveChild, LookupBlend(liveChild, LiveChild), _activeBlend);
                LiveChild.OnTransitionFromCamera(liveChild, worldUp, deltaTime);
                CinemachineCore.Instance.GenerateCameraActivationEvent(LiveChild, liveChild);
                if (_activeBlend == null)
                    CinemachineCore.Instance.GenerateCameraCutEvent(LiveChild);
            }

            if (_activeBlend != null)
            {
                _activeBlend.TimeInBlend += (double) deltaTime < 0.0 ? _activeBlend.Duration : deltaTime;
                if (_activeBlend.IsComplete)
                    _activeBlend = null;
            }

            if (_activeBlend != null)
            {
                _activeBlend.UpdateCameraState(worldUp, deltaTime);
                _state = _activeBlend.State;
            }
            else if (LiveChild != null)
            {
                _state = LiveChild.State;
            }

            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref _state, deltaTime);
            PreviousStateIsValid = true;
        }

        public override string Description
        {
            get
            {
                if (_activeBlend != null)
                    return _activeBlend.Description;
                var liveChild = LiveChild;
                if (liveChild == null)
                    return "(none)";
                var sb = CinemachineDebug.SBFromPool().Append("[").Append(liveChild.Name).Append("]");
                var str = sb.ToString();
                CinemachineDebug.ReturnToPool(sb);
                return str;
            }
        }

        private ChildCamera ChooseCurrentCamera()
        {
            if (Cameras != null && Cameras.Length > 0)
            {
                if (_stack.Count > 0)
                {
                    var topTag = _stack[_stack.Count - 1].Value;
                    foreach (var element in Cameras)
                        if (element.Tag.Target == topTag)
                            return element;
                }
                return Cameras[0];
            }
            return null;
        }

        private CinemachineBlendDefinition LookupBlend(ICinemachineCamera fromKey, ICinemachineCamera toKey)
        {
            var defaultBlend = DefaultBlend;
            if (CustomBlends != null)
                defaultBlend = CustomBlends.GetBlendForVirtualCameras(fromKey == null ? string.Empty : fromKey.Name,
                    toKey == null ? string.Empty : toKey.Name, defaultBlend);
            return defaultBlend;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _activeBlend = null;
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
            CinemachineDebug.OnGUIHandlers += OnGuiHandler;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
        }

        public void OnTransformChildrenChanged()
        {
            UpdateCamerasList();
        }

        private void OnGuiHandler()
        {
            if (!ShowDebugText)
            {
                CinemachineDebug.ReleaseScreenPos(this);
            }
            else
            {
                var sb = CinemachineDebug.SBFromPool();
                sb.Append(Name);
                sb.Append(": ");
                sb.Append(Description);
                var text = sb.ToString();
                GUI.Label(CinemachineDebug.GetScreenPos(this, text, GUI.skin.box), text, GUI.skin.box);
                CinemachineDebug.ReturnToPool(sb);
            }
        }

        [ContextMenu("Update Cameras List")]
        private void UpdateCamerasList()
        {
            var virtualCameraBaseList = GetComponentsInChildren<CinemachineVirtualCameraBase>(true)
                .Where(x => x.transform.parent == transform)
                .ToArray();
            var oldCameras = Cameras;
            Cameras = new ChildCamera[virtualCameraBaseList.Length];
            for (int i = 0; i < virtualCameraBaseList.Length; ++i)
            {
                var camera = virtualCameraBaseList[i];
                var oldCameraEntry = oldCameras.FirstOrDefault(x => ReferenceEquals(x.Camera, camera));
                Cameras[i] = oldCameraEntry ?? new ChildCamera(camera);
            }
            if (!Array.Exists(Cameras, x => ReferenceEquals(x.Camera, LiveChild)))
                LiveChild = null;
            SetupFollow(_follow);
            SetupLookAt(_lookAt);
            SetupExtra();
        }

        private void SetupFollow(Transform follow)
        {
            _follow = follow;
            foreach (var element in Cameras)
                element.Camera.Follow = CameraAnchorFollow.Find(_follow, element.Tag.Target) ?? _follow;
        }

        private void SetupLookAt(Transform lookAt)
        {
            _lookAt = lookAt;
            foreach (var element in Cameras)
                element.Camera.LookAt = CameraAnchorLookAt.Find(_lookAt, element.Tag.Target) ?? _lookAt;
        }

        protected virtual void SetupExtra() {}

        [Serializable]
        public class ChildCamera
        {
            public CinemachineVirtualCameraBase Camera;
            public CameraRef Tag;

            public ChildCamera(CinemachineVirtualCameraBase camera)
            {
                Assert.IsNotNull(camera);
                Camera = camera;
                Tag = null;
            }
        }
    }
}