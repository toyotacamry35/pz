using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Camera;
using Assets.Src.Tools;
using Cinemachine;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    class CharacterBuildBehaviour : MonoBehaviour, ICharacterBuildInterface
    {
        private bool resourcesIsEnough = true;
        private Vector3 lastPlayerPosition = Vector3.zero;

        // ICharacterBuilderInterface -------------------------------------------------------------
        public BuildMode ActiveMode { get; private set; } = BuildMode.None;
        public BuildType ActiveType { get; private set; } = BuildType.None;
        public bool IsBuildingPlaceActive => true;
        public bool IsBuildingPlaceInRange => true;
        public bool HasSelectedElement => lastSelectedData?.Selected ?? false;

        public CanPlaceData CanPlace
        {
            get
            {
                var placeholderData = GetPlaceholderData(ActiveType);
                if (placeholderData != null)
                {
                    return placeholderData.CanPlace;
                }
                else
                {
                    return null;
                }
            }
        }

        private bool CheckResources()
        {
            bool result = resourcesIsEnough;
            if (BuildSystem.Builder.ClaimResourcesEnableCheat)
            {
                if (!BuildSystem.Builder.ClaimResourcesValueCheat)
                {
                    return true;
                }
            }
            return result;
        }

        private bool CanBuildHere()
        {
            var placeholderData = GetPlaceholderData(ActiveType);
            if (placeholderData != null)
            {
                return placeholderData.CanBuildHere();
            }
            return true;
        }

        public bool Activate(bool activate)
        {
            ActiveMode = activate ? BuildMode.PickExisted : BuildMode.None;
            if (ActiveMode == BuildMode.None)
            {
                if (buildSystemCallackRegistered)
                {
                    BuildSystem.Builder.BuildingPlaceRegistered -= OnBuildingPlaceChanged;
                    BuildSystem.Builder.BuildingPlaceUnregistered -= OnBuildingPlaceChanged;
                    BuildSystem.Builder.BuildingPlaceChanged -= OnBuildingPlaceChanged;
                    DeactivateBuildingPlace();
                    ClearAllClientObjects();
                    buildSystemCallackRegistered = false;
                }
                if ((CinemachineCore.CameraUpdatedEvent != null) && cameraCallackRegistered)
                {
                    CinemachineCore.CameraUpdatedEvent.RemoveListener(CameraUpdatedCallback);
                    cameraCallackRegistered = false;
                }
            }
            else
            {
                if (!buildSystemCallackRegistered)
                {
                    BuildSystem.Builder.BuildingPlaceRegistered += OnBuildingPlaceChanged;
                    BuildSystem.Builder.BuildingPlaceUnregistered += OnBuildingPlaceChanged;
                    BuildSystem.Builder.BuildingPlaceChanged += OnBuildingPlaceChanged;
                    BuildSystem.Builder.UpdateCheats(NodeAccessor.Repository); //TODO building переделать на сервисную энтити
                    ActivateBuildingPlace(true);
                    buildSystemCallackRegistered = true;
                }
                if ((CinemachineCore.CameraUpdatedEvent != null) && !cameraCallackRegistered)
                {
                    CinemachineCore.CameraUpdatedEvent.AddListener(CameraUpdatedCallback);
                    cameraCallackRegistered = true;
                }
            }
            return true;
        }

        private void OnBuildingPlaceChanged(object sender, BuildingPlaceEventArgs e)
        {
            ActivateBuildingPlace(true);
        }

        public bool ActivatePlaceholder(BuildRecipeDef buildRecipeDef)
        {
            var buildingElementDef = buildRecipeDef.ElementDef.Target as BuildingElementDef;
            if (buildingElementDef != null)
            {
                ActiveMode = BuildMode.PlaceNew;
                ActiveType = BuildType.BuildingElement;
                buildingPlaceholderData.BuildRecipeDef = buildRecipeDef;
                buildingPlaceholderData.PlaceholderType = buildingElementDef.Type;
                return true;
            }
            else
            {
                var fenceElementDef = buildRecipeDef.ElementDef.Target as FenceElementDef;
                if (fenceElementDef != null)
                {
                    ActiveMode = BuildMode.PlaceNew;
                    ActiveType = BuildType.FenceElement;
                    fencePlaceholderData.BuildRecipeDef = buildRecipeDef;
                    return true;
                }
            }
            return false;
        }

        public bool SetResourcesIsEnough(bool _resourcesIsEnough)
        {
            resourcesIsEnough = _resourcesIsEnough;
            return true;
        }

        public bool CreateElement()
        {
            if (ActiveMode == BuildMode.PlaceNew)
            {
                var placeholderData = GetPlaceholderData(ActiveType);
                if ((placeholderData != null) && CheckResources() && placeholderData.CanPlace.Result && placeholderData.Visible && !operationTimer.IsInProgress())
                {
                    ShowPlaceholder(false);
                    BuildSystem.Builder.CreateElement(NodeAccessor.Repository, placeholderData);
                    operationTimer.Set(operationDelay);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveElement()
        {
            if ((ActiveMode == BuildMode.PickExisted) && (lastSelectedData != null) && !operationTimer.IsInProgress())
            {
                //GameState.Instance.ClientClusterNode
                BuildSystem.Builder.RemoveElement(NodeAccessor.Repository, ActiveType, lastSelectedData.PlaceId, lastSelectedData.ElementId);
                operationTimer.Set(operationDelay);
                return true;
            }
            return false;
        }

        public bool CyclePlaceholderRotation(bool positive)
        {
            if (ActiveType == BuildType.BuildingElement)
            {
                buildingPlaceholderData.ChangeRotation(positive);
                return true;
            }
            else if (ActiveType == BuildType.FenceElement)
            {
                fencePlaceholderData.ChangeRotation(positive);
                return true;
            }
            return false;
        }

        public bool CyclePlaceholderShift(bool positive, PlaceholderShiftType shiftType)
        {
            if (ActiveType == BuildType.BuildingElement)
            {
                buildingPlaceholderData.ChangeShift(positive, shiftType);
                return true;
            }
            else if (ActiveType == BuildType.FenceElement)
            {
                fencePlaceholderData.ChangeShift(positive, shiftType);
                return true;
            }
            return false;
        }

        private bool buildSystemCallackRegistered = false;
        private bool cameraCallackRegistered = false;

        private bool IsEnabled => (ActiveMode != BuildMode.None);

        private UnityEngine.Camera raycastCamera = null;
        private Vector3 centerViewportPoint = new Vector3(0.5f, 0.5f, 0.0f);

        private BuildingPlaceholderData buildingPlaceholderData = new BuildingPlaceholderData();
        private FencePlaceholderData fencePlaceholderData = new FencePlaceholderData();
        //TODO building, make real attachmentPlaceholderData
        private FencePlaceholderData attachmentPlaceholderData = new FencePlaceholderData();

        private ElementData lastSelectedData = null;

        private const float operationDelay = 1.0f;
        private DelayTimer operationTimer = new DelayTimer();

        // common methods -------------------------------------------------------------------------
        private void DeactivateBuildingPlace()
        {
            BuildSystem.Builder.DeactiveBuildingPlace(NodeAccessor.Repository);
        }

        private void ActivateBuildingPlace(bool force)
        {
            if (ActiveMode != BuildMode.None)
            {
                var playerPosition = GetPlayerPosition(false);
                if (force ||
                    (((playerPosition.x - lastPlayerPosition.x) * (playerPosition.x - lastPlayerPosition.x) +
                      (playerPosition.z - lastPlayerPosition.z) * (playerPosition.z - lastPlayerPosition.z)) > SharedCode.Utils.BuildUtils.BuildParamsDef.BuildingPlacePositionThreshold2))
                {
                    lastPlayerPosition = playerPosition;
                    BuildSystem.Builder.ActivateBuildingPlace(NodeAccessor.Repository, playerPosition);
                }
            }
        }

        private Vector3 GetPlayerPosition(bool addCharacterCenterPoint)
        {
            if (addCharacterCenterPoint)
            {
                return gameObject.transform.position + (Vector3)(SharedCode.Utils.BuildUtils.BuildParamsDef.CharacterCenterPoint);
            }
            else
            {
                return gameObject.transform.position;
            }
        }

        private Quaternion GetPlayerRotation()
        {
            return raycastCamera.transform.rotation;
        }

        private PlaceholderData GetPlaceholderData(BuildType type)
        {
            if (type == BuildType.BuildingElement) { return buildingPlaceholderData; }
            else if (type == BuildType.FenceElement) { return fencePlaceholderData; }
            else if ((type == BuildType.BuildingAttachment) || (type == BuildType.FenceAttachment)) { return attachmentPlaceholderData; }
            else { return null; }
        }

        private void GameCameraNotifier_OnCameraCreated(UnityEngine.Camera camera)
        {
            SetRaycastCamera(camera);
        }

        private void GameCameraNotifier_OnCameraDestroyed()
        {
            SetRaycastCamera(null);
        }

        private void SetRaycastCamera(UnityEngine.Camera camera)
        {
            raycastCamera = camera;
        }

        private bool UpdatePlaceholderInterfaceData()
        {
            if (ActiveType == BuildType.BuildingElement)
            {
                return UpdateBuildingPlaceholderInterfaceData();
            }
            else if (ActiveType == BuildType.FenceElement)
            {
                return UpdateFencePlaceholderInterfaceData();
            }
            return false;
        }

        private void ClearAllClientObjects()
        {
            if (lastSelectedData != null)
            {
                lastSelectedData.Selected = false;
                lastSelectedData = null;
            }
            ShowPlaceholder(false);
        }

        private void UpdateFromCamera()
        {
            if (raycastCamera != null)
            {
                BuildSystemHelper.ElementDataHitInfo hitInfo = null;
                if (ActiveMode == BuildMode.PickExisted)
                {
                    var ray = raycastCamera.ViewportPointToRay(centerViewportPoint);
                    ray.origin = GetPlayerPosition(true);
                    hitInfo = BuildSystemHelper.RayCast(ray);
                }
                if ((hitInfo != null) && (hitInfo.Data != null))
                {
                    if (lastSelectedData != hitInfo.Data)
                    {
                        if (lastSelectedData != null)
                        {
                            lastSelectedData.Selected = false;
                        }
                        else
                        {
                            ShowPlaceholder(false);
                        }
                        ActiveType = hitInfo.BuildType;
                        lastSelectedData = hitInfo.Data;
                        lastSelectedData.Selected = true;
                    }
                }
                else
                {
                    if (lastSelectedData != null)
                    {
                        lastSelectedData.Selected = false;
                        lastSelectedData = null;
                    }
                    if ((ActiveMode == BuildMode.PlaceNew) && UpdatePlaceholderInterfaceData())
                    {
                        ShowPlaceholder(true);
                    }
                    else
                    {
                        ShowPlaceholder(false);
                    }
                }
            }
        }

        private void ShowPlaceholder(bool show)
        {
            if (ActiveType != BuildType.None)
            {
                var placeholderData = GetPlaceholderData(ActiveType);
                if (placeholderData != null)
                {
                    if ((!show || (ActiveMode != BuildMode.PlaceNew)) && placeholderData.Visible)
                    {
                        BuildSystem.Builder.HidePlaceholder(placeholderData);
                        placeholderData.Cache(false);
                    }
                    else if (show && (ActiveMode == BuildMode.PlaceNew))
                    {
                        BuildSystem.Builder.CalculatePlaceholder(placeholderData);
                        placeholderData.CanPlace.Switch(!CheckResources(), CanPlaceData.REASON_NO_RESOURCES);
                        placeholderData.CanPlace.Switch(!CanBuildHere(), CanPlaceData.REASON_PROHIBITED_POSITION);
                        placeholderData.CanPlace.Switch(operationTimer.IsInProgress(), CanPlaceData.REASON_TIMER);

                        if (placeholderData.IsCacheInvalid())
                        {
                            placeholderData.Cache(true);
                            BuildSystem.Builder.ShowPlaceholder(placeholderData);
                        }
                    }
                }
            }
        }

        // building specific ----------------------------------------------------------------------
        private bool UpdateBuildingPlaceholderInterfaceData()
        {
            buildingPlaceholderData.InterfacePosition = GetPlayerPosition(false);
            buildingPlaceholderData.InterfaceRotation = GetPlayerRotation();
            return true;
        }

        // fence specific -------------------------------------------------------------------------
        private bool UpdateFencePlaceholderInterfaceData()
        {
            fencePlaceholderData.InterfacePosition = GetPlayerPosition(false);
            fencePlaceholderData.InterfaceRotation = GetPlayerRotation();
            return true;
        }

        private void CameraUpdatedCallback(CinemachineBrain brain)
        {
            if (BuildSystem.Builder.IsEnabled && IsEnabled)
            {
                UpdateFromCamera();
            }
        }

        // Unity methods --------------------------------------------------------------------------
        void Awake()
        {
            SetRaycastCamera(GameCamera.Camera);
            GameCamera.OnCameraCreated += GameCameraNotifier_OnCameraCreated;
            GameCamera.OnCameraDestroyed += GameCameraNotifier_OnCameraDestroyed;
        }

        void OnDestroy()
        {
            GameCamera.OnCameraCreated -= GameCameraNotifier_OnCameraCreated;
            GameCamera.OnCameraDestroyed -= GameCameraNotifier_OnCameraDestroyed;
            SetRaycastCamera(null);
        }

        void FixedUpdate()
        {
            ActivateBuildingPlace(false);
        }
    }
}

