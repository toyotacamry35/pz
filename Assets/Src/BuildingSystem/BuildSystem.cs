using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using Assets.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SharedCode.Serializers;

namespace Assets.Src.BuildingSystem
{
    public class BuildSystem : IBuildInterface
    {
        private bool isEnabled = true;
        private bool isSimpleMode = true;

        private bool damageEnableCheat = false;
        private float damageValueCheat = 0.0f;

        private bool claimResourcesEnableCheat = false;
        private bool claimResourcesValueCheat = true;

        private bool visualEnableCheat = false;

        private bool canPlaceAnchor = false;
        private BuildingPlaceData activeBuildingPlace = null;
        private BuildingPlaceData simpleBuildingPlace = new BuildingPlaceData(); // empty building place for initial element placeholder calculating
        private FencePlaceData activeFencePlace = new FencePlaceData(); // fake fence place for placeholder calculating

        private ElementData activeBuildingPlaceholder = null;
        private ElementData activeFencePlaceholder = null;
        private ElementData activeAttachmentPlaceholder = null;

        private Dictionary<Guid,PlaceData> buildingPlaces = new Dictionary<Guid,PlaceData>();
        private Dictionary<Guid,PlaceData> fencePlaces = new Dictionary<Guid,PlaceData>();

        private PlaceData GetActivePlace(BuildType type) // only for placeholders
        {
            if (type == BuildType.BuildingElement) { return (activeBuildingPlace != null) ? activeBuildingPlace : (isSimpleMode ? simpleBuildingPlace : null); }
            else if (type == BuildType.BuildingAttachment) { return activeBuildingPlace; }
            else if (type == BuildType.FenceElement) { return activeFencePlace; }
            else if (type == BuildType.FenceAttachment) { return activeFencePlace; }
            else { return null; }
        }

        private ElementData GetActivePlaceholder(BuildType type) // only for placeholders
        {
            if (type == BuildType.BuildingElement) { return activeBuildingPlaceholder; }
            else if (type == BuildType.FenceElement) { return activeFencePlaceholder; }
            else if ((type == BuildType.BuildingAttachment) || (type == BuildType.FenceAttachment)) { return activeAttachmentPlaceholder; }
            else { return null; }
        }

        private void SetActivePlaceholder(BuildType type, ElementData placeholder) // only for placeholders
        {
            if (type == BuildType.BuildingElement) { activeBuildingPlaceholder = placeholder; }
            else if (type == BuildType.FenceElement) { activeFencePlaceholder = placeholder; }
            else if ((type == BuildType.BuildingAttachment) || (type == BuildType.FenceAttachment)) { activeAttachmentPlaceholder = placeholder; }
        }

        private Dictionary<Guid, PlaceData> GetPlaces(PlaceType type)
        {
            if (type == PlaceType.BuildingPlace) { return buildingPlaces; }
            else if (type == PlaceType.FencePlace) { return fencePlaces; }
            else { return null; }
        }

        public PlaceData CreatePlace(PlaceType type)
        {
            if (type == PlaceType.BuildingPlace) { return new BuildingPlaceData(); }
            else if (type == PlaceType.FencePlace) { return new FencePlaceData(); }
            else { return null; }
        }

        public static IBuildInterface Builder => GameState.Instance ? GameState.Instance.BuildSystem : null;

        public BuildSystem()
        {
            PropertyDataHelper.CreateCopyList(new KeyValuePair<Type, Type>(typeof(FencePlaceAlways), typeof(FencePlaceData)));
            PropertyDataHelper.CreateCopyList(new KeyValuePair<Type, Type>(typeof(PositionedFenceElementAlways), typeof(FenceElementData)));
            PropertyDataHelper.CreateCopyList(new KeyValuePair<Type, Type>(typeof(BuildingPlaceAlways), typeof(BuildingPlaceData)));
            PropertyDataHelper.CreateCopyList(new KeyValuePair<Type, Type>(typeof(PositionedBuildingElementAlways), typeof(BuildingElementData)));
        }

        // IBuilderInterface, unity thread only ---------------------------------------------------
        public event EventHandler<BuildingPlaceEventArgs> BuildingPlaceRegistered;
        public event EventHandler<BuildingPlaceEventArgs> BuildingPlaceUnregistered;
        public event EventHandler<BuildingPlaceEventArgs> BuildingPlaceChanged;

        public bool IsEnabled { get { return isEnabled; } }
        public bool IsSimpleMode { get { return isSimpleMode; } }

        public bool DamageEnableCheat { get { return damageEnableCheat; } }
        public float DamageValueCheat { get { return damageValueCheat; } }

        public bool ClaimResourcesEnableCheat { get { return claimResourcesEnableCheat; } }
        public bool ClaimResourcesValueCheat { get { return claimResourcesValueCheat; } }

        public bool CheatVisualEnable { get { return visualEnableCheat; } }

        public PlaceData RegisterPlace(IEntitiesRepository entitiesRepository, PlaceType type, EventHandler OnBindFinished, EventHandler OnUnbindFinished, EventHandler<PropertyData.PropertyArgs> OnBindPropertyChanged, Guid placeId)
        {
            BuildUtils.Debug?.Report(true, $"type {type}, placeId: {placeId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var places = GetPlaces(type);
            if (places == null)
            {
                BuildUtils.Error?.Report($"can't find places of {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return null;
            }
            if (places.ContainsKey(placeId))
            {
                BuildUtils.Error?.Report($"place already registered {placeId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return null;
            }
            var place = CreatePlace(type);
            if (place == null)
            {
                BuildUtils.Error?.Report($"can't create place", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return null;
            }
            place.SetVisualCheat(visualEnableCheat);
            places.Add(placeId, place);
            if (OnBindFinished != null) { place.BindFinished += OnBindFinished; }
            if (OnUnbindFinished != null) { place.UnbindFinished += OnUnbindFinished; }
            if (OnBindPropertyChanged != null) { place.BindPropertyChanged += OnBindPropertyChanged; }
            place.Bind(entitiesRepository, placeId);
            BuildingPlaceRegistered?.Invoke(this, new BuildingPlaceEventArgs());
            return place;
        }

        public bool UnregisterPlace(IEntitiesRepository entitiesRepository, PlaceType type, Guid placeId)
        {
            BuildUtils.Debug?.Report(true, $"type {type}, placeId: {placeId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var places = GetPlaces(type);
            if (places == null)
            {
                BuildUtils.Error?.Report($"can't find places of {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            PlaceData place = null;
            if (!places.TryGetValue(placeId, out place))
            {
                BuildUtils.Error?.Report($"place already registered {placeId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            place.Unbind(entitiesRepository);
            places.Remove(placeId);

            if ((type == PlaceType.BuildingPlace) && (activeBuildingPlace != null) && (activeBuildingPlace.PlaceId == placeId))
            {
                activeBuildingPlace = null;
            }
            BuildingPlaceUnregistered?.Invoke(this, new BuildingPlaceEventArgs());
            return true;
        }

        public void InvokeBuildingPlaceChanged()
        {
            BuildUtils.Debug?.Report(true, $"", MethodBase.GetCurrentMethod().DeclaringType.Name);

            BuildingPlaceChanged?.Invoke(this, new BuildingPlaceEventArgs());
        }

        public void ActivateBuildingPlace(IEntitiesRepository entitiesRepository, UnityEngine.Vector3 position)
        {
            BuildUtils.Debug?.Report(true, $"position: {position}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            activeBuildingPlace = null;
            var buildingPlaceDistance2 = 0.0f;
            foreach (var placeData in buildingPlaces)
            {
                var buildingPlaceData = placeData.Value as BuildingPlaceData;
                if ((buildingPlaceData != null) && (buildingPlaceData.Owner.Guid == GameState.Instance.CharacterRuntimeData.CharacterId))
                {
                    var distance2 = (buildingPlaceData.Position.x - position.x) * (buildingPlaceData.Position.x - position.x) +
                                    (buildingPlaceData.Position.z - position.z) * (buildingPlaceData.Position.z - position.z);
                    if (activeBuildingPlace == null)
                    {
                        activeBuildingPlace = buildingPlaceData;
                        buildingPlaceDistance2 = distance2;
                    }
                    else if (distance2 < buildingPlaceDistance2)
                    {
                        activeBuildingPlace = buildingPlaceData;
                        buildingPlaceDistance2 = distance2;
                    }
                }
            }
        }

        public void DeactiveBuildingPlace(IEntitiesRepository entitiesRepository)
        {
            BuildUtils.Debug?.Report(true, $"", MethodBase.GetCurrentMethod().DeclaringType.Name);

            activeBuildingPlace = null;
        }

        public void CreateElement(IEntitiesRepository entitiesRepository, PlaceholderData data)
        {
            BuildUtils.Debug?.Report(true, $"type: {data.BuildType}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (data != null) && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                var creationData = data.GetCreationData(GetActivePlace(data.BuildType));
                if (creationData != null)
                {
                    //TODO building: lock поставить (одновременно спроверкой на наличие)
                    Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                    var _type = creationData.Type;
                    var _placeId = creationData.PlaceId;
                    var _buildRecipeDef = creationData.BuildRecipeDef;
                    var _data = creationData.Data;
                    AsyncUtils.RunAsyncTask(() => CreateBuildElementAsync(entitiesRepository, _characterId, _type, _placeId, _buildRecipeDef, _data), entitiesRepository);
                }
            }
        }

        public void RemoveElement(IEntitiesRepository entitiesRepository, BuildType type, Guid placeId, Guid elementId)
        {
            BuildUtils.Debug?.Report(true, $"type: {type}, placeId: {placeId}, elementId: {elementId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled&& (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                //TODO building: lock поставить (одновременно спроверкой на наличие)
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var _type = type;
                var _placeId = placeId;
                var _elementId = elementId;
                var _data = new RemoveData();
                _data.Type = OperationType.Remove;
                AsyncUtils.RunAsyncTask(() => OperateBuildElementAsync(entitiesRepository, _characterId, _type, _placeId, _elementId, _data), entitiesRepository);
            }
        }

        public void DamageElement(IEntitiesRepository entitiesRepository, BuildType type, Guid placeId, Guid elementId, float damage)
        {
            BuildUtils.Debug?.Report(true, $"type: {type}, placeId: {placeId}, elementId: {elementId}, damage: {damage}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                //TODO building: lock поставить (одновременно спроверкой на наличие)
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var _type = type;
                var _placeId = placeId;
                var _elementId = elementId;
                var _data = new DamageData();
                _data.Type = OperationType.Damage;
                _data.Damage = damage;
                AsyncUtils.RunAsyncTask(() => OperateBuildElementAsync(entitiesRepository, _characterId, _type, _placeId, _elementId, _data), entitiesRepository);
            }
        }

        public void InteractElement(IEntitiesRepository entitiesRepository, BuildType type, Guid placeId, Guid elementId, int interaction)
        {
            BuildUtils.Debug?.Report(true, $"type: {type}, placeId: {placeId}, elementId: {elementId}, interaction: {interaction}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                //TODO building: lock поставить (одновременно спроверкой на наличие)
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var _type = type;
                var _placeId = placeId;
                var _elementId = elementId;
                var _data = new InteractData();
                _data.Type = OperationType.Interact;
                _data.Interaction = interaction;
                AsyncUtils.RunAsyncTask(() => OperateBuildElementAsync(entitiesRepository, _characterId, _type, _placeId, _elementId, _data), entitiesRepository);
            }
        }

        public bool CalculatePlaceholder(PlaceholderData data)
        {
            //BuildUtils.Debug?.Report(true, $"type: {data.BuildType}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (data != null))
            {
                var place = GetActivePlace(data.BuildType);
                if (place != null)
                {
                    return place.CalculatePlaceholder(data);
                }
            }
            return false;
        }

        public bool ShowPlaceholder(PlaceholderData data)
        {
            //BuildUtils.Debug?.Report(true, $"type: {data.BuildType}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (data != null))
            {
                var place = GetActivePlace(data.BuildType);
                if (place != null)
                {
                    data.Placeholder = place.ShowPlaceholder(GetActivePlaceholder(data.BuildType), data);
                    SetActivePlaceholder(data.BuildType, data.Placeholder);
                    return (data.Placeholder != null);
                }
            }
            return false;
        }

        public bool HidePlaceholder(PlaceholderData data)
        {
            //BuildUtils.Debug?.Report(true, $"type: {data.BuildType}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled)
            {
                var place = GetActivePlace(data.BuildType);
                if (place != null)
                {
                    if (place.HidePlaceholder(GetActivePlaceholder(data.BuildType)))
                    {
                        data.Placeholder = null;
                        SetActivePlaceholder(data.BuildType, data.Placeholder);
                        return true;
                    }
                }
            }
            return false;
        }

        // Cheats ---------------------------------------------------------------------------------
        public void UpdateCheats(IEntitiesRepository entitiesRepository)
        {
            BuildUtils.Debug?.Report(true, $"", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                AsyncUtils.RunAsyncTask(() => UpdateCheatsAsync(entitiesRepository, _characterId), entitiesRepository);
            }
        }

        public void SetDamageCheat(IEntitiesRepository entitiesRepository, bool enable, float damage)
        {
            BuildUtils.Debug?.Report(true, $"enable: {enable}, damage: {damage}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var _data = new CheatDamageData();
                _data.Type = OperationType.CheatDamage;
                _data.Enable = enable;
                _data.Value = damage;
                AsyncUtils.RunAsyncTask(() => SetCheatAsync(entitiesRepository, _characterId, _data, true), entitiesRepository);
            }
        }

        public void SetClaimResourceCheat(IEntitiesRepository entitiesRepository, bool enable, bool claim)
        {
            BuildUtils.Debug?.Report(true, $"enable: {enable}, claim: {claim}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (IsEnabled && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var _data = new CheatClaimResourcesData();
                _data.Type = OperationType.CheatClaimResources;
                _data.Enable = enable;
                _data.Value = claim;
                AsyncUtils.RunAsyncTask(() => SetCheatAsync(entitiesRepository, _characterId, _data, true), entitiesRepository);
            }
        }

        public void SetVisualCheat(IEntitiesRepository entitiesRepository, bool enable)
        {
            BuildUtils.Debug?.Report(true, $"enable: {enable}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (visualEnableCheat != enable)
            {
                visualEnableCheat = enable;
                foreach (var buildingPlace in buildingPlaces)
                {
                    buildingPlace.Value.SetVisualCheat(enable);
                }
                foreach (var fencePlace in fencePlaces)
                {
                    fencePlace.Value.SetVisualCheat(enable);
                }
            }
        }

        public void SetDebugCheat(IEntitiesRepository entitiesRepository, bool enable, bool verboose)
        {
            BuildUtils.Debug?.Report(true, $"enable: {enable}, verboose: {verboose}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            BuildUtils.SetCheatDebug(enable, verboose);

            if (IsEnabled && (GameState.Instance != null) && (GameState.Instance.CharacterRuntimeData != null))
            {
                Guid _characterId = GameState.Instance.CharacterRuntimeData.CharacterId;
                var _data = new CheatDebugData();
                _data.Type = OperationType.CheatDebug;
                _data.Enable = enable;
                _data.Verboose = verboose;
                AsyncUtils.RunAsyncTask(() => SetCheatAsync(entitiesRepository, _characterId, _data, false), entitiesRepository);
            }
        }

        // Threadpool methods ---------------------------------------------------------------------
        private async Task<bool> CreateBuildElementAsync(IEntitiesRepository entitiesRepository, Guid characterId, BuildType type, Guid placeId, BuildRecipeDef buildRecipeDef, CreateBuildElementData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {characterId}, type: {type}, placeId: {placeId}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            using (var wrapper = await entitiesRepository.Get<IWorldCharacterClientFull>(characterId))
            {
                var character = wrapper.Get<IWorldCharacterClientFull>(characterId);
                if (character == null)
                {
                    BuildUtils.Error?.Report($"invalid character: {characterId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
                var result = await character.CreateBuildElement(type, placeId, buildRecipeDef, data);
                if (result.Result != ErrorCode.Success)
                {
                    BuildUtils.Error?.Report($"error: {result.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> OperateBuildElementAsync(IEntitiesRepository entitiesRepository, Guid characterId, BuildType type, Guid placeId, Guid elementId, OperationData data)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {characterId}, type: {type}, placeId: {placeId}, elementId: {elementId}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            using (var wrapper = await entitiesRepository.Get<IWorldCharacterClientFull>(characterId))
            {
                var character = wrapper.Get<IWorldCharacterClientFull>(characterId);
                if (character == null)
                {
                    BuildUtils.Error?.Report($"invalid character: {characterId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
                var result = await character.OperateBuildElement(type, placeId, elementId, data);
                if (result.Result != ErrorCode.Success)
                {
                    BuildUtils.Error?.Report($"error: {result.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> SetCheatAsync(IEntitiesRepository entitiesRepository, Guid characterId, OperationData data, bool update)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {characterId}, data: {data}, update: {update}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            using (var wrapper = await entitiesRepository.Get<IWorldCharacterClientFull>(characterId))
            {
                var character = wrapper.Get<IWorldCharacterClientFull>(characterId);
                if (character == null)
                {
                    BuildUtils.Error?.Report($"invalid character: {characterId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
                var result = await character.SetBuildCheat(data);
                if (result.Result != ErrorCode.Success)
                {
                    BuildUtils.Error?.Report($"error: {result.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
            }
            if (update)
            {
                await UpdateCheatsAsync(entitiesRepository, characterId);
            }
            return true;
        }

        private async Task<bool> UpdateCheatsAsync(IEntitiesRepository entitiesRepository, Guid characterId)
        {
            BuildUtils.Debug?.Report(true, $"characterId: {characterId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            OperationResultEx cheatDamageResult = null;
            OperationResultEx cheatClaimResourcesResult = null;
            OperationResultEx cheatDebugResult = null;
            using (var wrapper = await entitiesRepository.Get<IWorldCharacterClientFull>(characterId))
            {
                var character = wrapper.Get<IWorldCharacterClientFull>(characterId);
                if (character == null)
                {
                    BuildUtils.Error?.Report($"invalid character: {characterId}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }

                CheatDamageData cheatDamageData = new CheatDamageData();
                cheatDamageData.Type = OperationType.CheatDamage;
                cheatDamageResult = await character.GetBuildCheat(cheatDamageData);

                CheatClaimResourcesData cheatClaimResourcesData = new CheatClaimResourcesData();
                cheatClaimResourcesData.Type = OperationType.CheatClaimResources;
                cheatClaimResourcesResult = await character.GetBuildCheat(cheatClaimResourcesData);

                CheatDebugData cheatDebugData = new CheatDebugData();
                cheatDebugData.Type = OperationType.CheatDebug;
                cheatDebugResult = await character.GetBuildCheat(cheatDebugData);

                if ((cheatDamageResult.Result != ErrorCode.Success) || (cheatClaimResourcesResult.Result != ErrorCode.Success) || (cheatDebugResult.Result != ErrorCode.Success))
                {
                    BuildUtils.Error?.Report($"error: damage: {cheatDamageResult.Result}, claimResources: {cheatClaimResourcesResult.Result}, debug: {cheatDebugResult.Result}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return false;
                }
            }
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if ((cheatDamageResult != null) && (cheatDamageResult.Result == ErrorCode.Success) && (cheatDamageResult.OperationData != null))
                {
                    var cheatDamageData = cheatDamageResult.OperationData as CheatDamageData;
                    if (cheatDamageData != null)
                    {
                        damageEnableCheat = cheatDamageData.Enable;
                        damageValueCheat = cheatDamageData.Value;
                    }
                }
                if ((cheatClaimResourcesResult != null) && (cheatClaimResourcesResult.Result == ErrorCode.Success) && (cheatClaimResourcesResult.OperationData != null))
                {
                    var cheatClaimResourcesData = cheatClaimResourcesResult.OperationData as CheatClaimResourcesData;
                    if (cheatClaimResourcesData != null)
                    {
                        claimResourcesEnableCheat = cheatClaimResourcesData.Enable;
                        claimResourcesValueCheat = cheatClaimResourcesData.Value;
                    }
                }
                if ((cheatDebugResult != null) && (cheatDebugResult.Result == ErrorCode.Success) && (cheatDebugResult.OperationData != null))
                {
                    var cheatDebugData = cheatDebugResult.OperationData as CheatDebugData;
                    if (cheatDebugData != null)
                    {
                        BuildUtils.SetCheatDebug(cheatDebugData.Enable, cheatDebugData.Verboose);
                    }
                }
            });
            return true;
        }
    }
}