using Assets.Src.Aspects.Impl;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using Assets.Src.WorldSpace;
using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using System;
using System.Threading.Tasks;
using Assets.Src.Camera;
using Src.Aspects.Impl;
using Uins;
using Uins.Slots;
using UnityEngine;
using Assets.ColonyShared.SharedCode.Shared;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using SharedCode.Serializers;

// костры, верстаки
namespace Assets.Src.Aspects.Building
{
    public class BuildingCreator : EntityGameObjectComponent
    {
        [SerializeField, Range(3f, 50f)]
        private float BuildingDistance = 20f;

        private SlotViewModel _slotViewModel;

        private GameObject _activeGameObject;
        private BuildingEditor _buildingEditor;

        private RaycastHit _hit;
        private Vector3 _buildPoint;
        private UnityEngine.Camera _camera;
        private readonly Vector3 _pointView = new Vector3(0.5f, 0.5f, 0);

        private Action<bool> _onMountingEnd;
        private bool _isMounting;


        //=== Unity ===========================================================

        private void Update()
        {
            if (!_isMounting)
                return;

            if (_activeGameObject.AssertIfNull(nameof(_activeGameObject)))
                return;

            var ray = _camera.ViewportPointToRay(_pointView);
            if (Physics.Raycast(ray, out _hit, PhysicsChecker.CheckDistance(BuildingDistance, "BuildingCreator"), PhysicsLayers.BuildMask, QueryTriggerInteraction.Ignore) &&
                _hit.distance >= (_hit.point - gameObject.transform.position).magnitude)
            {
                _buildPoint = _hit.point;
                _activeGameObject.transform.position = _buildingEditor.GetPosition(_buildPoint);
                _activeGameObject.transform.rotation = Quaternion.Lerp(_buildingEditor.GetRotation(_buildPoint),
                    _activeGameObject.transform.rotation, 0.5f);
                var sceneId = NodeAccessor.Repository.GetSceneForEntity(new OuterRef<IEntity>(
         GameState.Instance.CharacterRuntimeData.CharacterId, WorldCharacter.StaticTypeId));
                IEntityObjectDef entityObjectDef = (_slotViewModel?.ItemResource as ItemResource)?.MountingData?.EntityObjectDef.Target;
                _buildingEditor.SetAvaliable(_buildingEditor.CanBuildHere(_buildPoint, _activeGameObject.transform.rotation, sceneId, entityObjectDef));
            }
        }

        protected override void DestroyInternal()
        {
            if (_isMounting)
                Cancel();
        }


        //=== Public ==========================================================

        public void MountingStart(SlotViewModel slotViewModel, Action<bool> onMountingEnd)
        {
            if (slotViewModel.AssertIfNull(nameof(slotViewModel)))
                return;

            _isMounting = true;
            _onMountingEnd = onMountingEnd;
            SetActivePrototype(slotViewModel);
        }

        public async Task MountingAccept()
        {
            if (!_isMounting)
            {
                UI. Logger.IfError()?.Message("Attempt to accept mounting while mounting isn't happening").Write();;
                return;
            }

            if (!_buildingEditor.IsAvaliable) //не пытаемся ставить, если нельзя
                return;

            SharedCode.Utils.Vector3 position = UnityWorldSpace.ToVector3(_activeGameObject.transform.position);
            SharedCode.Utils.Vector3 scale = UnityWorldSpace.ToVector3(_activeGameObject.transform.localScale);
            SharedCode.Utils.Quaternion rotation = UnityWorldSpace.ToQuaternion(_activeGameObject.transform.rotation);

            var address = _slotViewModel.SlotsCollectionApi.CollectionPropertyAddress;
            var slotId = _slotViewModel.SlotId;
            var itemId = _slotViewModel.SelfSlotItem.ItemGuid;
            var engineId = Ego.EntityId;

            var isBuilded = await AsyncUtils.RunAsyncTask(() => BuildAsync(address, slotId, position, rotation, engineId));

            _onMountingEnd?.Invoke(isBuilded);
            ClearState();
        }

        public void Cancel()
        {
            if (!_isMounting)
            {
                UI. Logger.IfError()?.Message("Attempt to cancel mounting while mounting isn't happening").Write();;
                return;
            }

            _onMountingEnd?.Invoke(false);
            ClearState();
        }

        public void Rotate(float angle)
        {
            if (!_isMounting || _activeGameObject.AssertIfNull(nameof(_activeGameObject)))
            {
                UI. Logger.IfError()?.Message("Attempt to rotate object while mounting isn't happening").Write();;
                return;
            }

            _buildingEditor.RotationAngle += angle;
        }


        //=== Private =========================================================

        private void ClearState()
        {
            _isMounting = false;
            _onMountingEnd = null;
            if (_activeGameObject != null)
            {
                DestroyImmediate(_activeGameObject);
                _buildingEditor = null;
                _slotViewModel = null;
                _camera = null;
            }
        }

        private void SetActivePrototype(SlotViewModel slotViewModel)
        {
            _slotViewModel = slotViewModel;
            var prefab = (_slotViewModel.ItemResource as ItemResource).MountingData.EntityObjectDef.Target.Prefab?.Target;
            if ((prefab?.GetComponent<BuildingEditor>()).AssertIfNull("visualGo.BuildingEditor"))
            {
                _onMountingEnd?.Invoke(false);
                return;
            }

            _camera = GameCamera.Camera;
            _activeGameObject = Instantiate(prefab);
            _activeGameObject.SetActive(true);
            _activeGameObject.name = prefab.name + "_tempo";

            _buildingEditor = _activeGameObject.GetComponent<BuildingEditor>();
            _buildingEditor.SetVisibility(true);
            _buildingEditor.SetCollidersEnable(false);
            _buildingEditor.RotationAngle = prefab.transform.rotation.eulerAngles.y;
        }

        private async Task<bool> BuildAsync(PropertyAddress address, int slodIds, SharedCode.Utils.Vector3 position, SharedCode.Utils.Quaternion rotation, Guid entityId)
        {
            if (address.AssertIfNull(nameof(PropertyAddress)))
                return false;

            using (var wrapper2 = await ClusterCommands.ClientRepository.Get<IBuildingEngineClientFull>(entityId))
            {
                var buildingEngine = wrapper2.Get<IBuildingEngineClientFull>(entityId);
                var result = await buildingEngine.Build(address, slodIds, position, rotation);
                return !result.Is(BuildOperationResult.Error);
            }
        }
    }
}