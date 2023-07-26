using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.Chain;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.MovementSync;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    struct BuildingPlaceGridSyncType : IPointSpatialHashType
    {
        public int StaticCellSize => 300;

        public int StaticHeightSize => 400;

        public int StaticReplicationRadius => 300;

        public bool ShouldCheckForRadius => true;

        public IEntityObjectDef Def { get; set; }
    }

    public partial class BuildingPlace: IHookOnInit, IHookOnDatabaseLoad, IHookOnReplicationLevelChanged
    {
        private class ServerBuildingStructure : BuildingStructure
        {
            protected class ServerBlock : Block
            {
                protected override void OnBeginSwitchEdges() { }
                protected override void OnEndSwitchEdges() { }
                protected override void OnUpdateEdges() { }
                protected override void OnUpdateFaces() { }
                public ServerBlock(Vector3Int blockCount, int x, int y, int z) : base(blockCount, x, y, z) { }
            }

            protected override Block CreateBlock(Vector3Int blockCount, int x, int y, int z)
            {
                return new ServerBlock(blockCount, x, y, z);
            }

            protected override void OnCreate(int size, int height, Vector3 position, Quaternion rotation, float blockSize) { }
            protected override void OnDestroy() { }
            protected override uint OnSet(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side) { return CanPlaceData.REASON_OK; }
            protected override void OnClear(Guid id, int x, int y, int z, BuildRecipeDef buildRecipeDef, BuildingElementFace face, BuildingElementSide side) { }
        }

        private ServerBuildingStructure serverBuildingStructure = null;

        protected override void constructor()
        {
            BuildUtils.Debug?.Report(true, $"", MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        public Task OnInit()
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var buildingPlaceDef = Def as BuildingPlaceDef;
            if (buildingPlaceDef == null)
            {
                BuildUtils.Error?.Report($"Def: {Def} is null or not a BuildingPlaceDef", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.CompletedTask;
            }

            serverBuildingStructure = new ServerBuildingStructure();
            serverBuildingStructure.Create(buildingPlaceDef.Size, buildingPlaceDef.Height, MovementSync.Position, MovementSync.Rotation, buildingPlaceDef.BlockSize);

            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var buildingPlaceDef = Def as BuildingPlaceDef;
            if (buildingPlaceDef == null)
            {
                BuildUtils.Error?.Report($"Def: {Def} is null or not a BuildingPlaceDef", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.CompletedTask;
            }

            serverBuildingStructure = new ServerBuildingStructure();
            serverBuildingStructure.Create(buildingPlaceDef.Size, buildingPlaceDef.Height, MovementSync.Position, MovementSync.Rotation, buildingPlaceDef.BlockSize);

            foreach (var element in Elements)
            {
                var positionedBuildingElement = element.Value;
                if (positionedBuildingElement != null)
                {
                    var result = serverBuildingStructure.Set(positionedBuildingElement.Id,
                                                             positionedBuildingElement.Block.x,
                                                             positionedBuildingElement.Block.y,
                                                             positionedBuildingElement.Block.z,
                                                             positionedBuildingElement.RecipeDef,
                                                             positionedBuildingElement.Face,
                                                             positionedBuildingElement.Side);
                }
            }

            return Task.CompletedTask;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<bool> ContainsKeyImpl(BuildType type, Guid elementId)
        {
            //BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {elementId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            switch (type)
            {
                case BuildType.Any:
                {
                    return Task.FromResult(Elements?.ContainsKey(elementId) ?? Attachments?.ContainsKey(elementId) ?? false);
                }
                case BuildType.BuildingElement:
                {
                    return Task.FromResult(Elements?.ContainsKey(elementId) ?? false);
                }
                case BuildType.BuildingAttachment:
                {
                    return Task.FromResult(Attachments?.ContainsKey(elementId) ?? false);
                }
            }
            return Task.FromResult(false);
        }

        public Task<IPositionedBuild> TryGetValueImpl(BuildType type, Guid elementId)
        {
            //BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {elementId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            switch (type)
            {
                case BuildType.BuildingElement:
                {
                    if (Elements != null)
                    {
                        IPositionedBuildingElement positionedBuildingElement;
                        if (Elements.TryGetValue(elementId, out positionedBuildingElement))
                        {
                            return Task.FromResult<IPositionedBuild>(positionedBuildingElement);
                        }
                    }
                    break;
                }
                case BuildType.BuildingAttachment:
                {
                    if (Attachments != null)
                    {
                        IPositionedAttachment positionedAttachment;
                        if (Attachments.TryGetValue(elementId, out positionedAttachment))
                        {
                            return Task.FromResult<IPositionedBuild>(positionedAttachment);
                        }
                    }
                    break;
                }
                case BuildType.Any:
                {
                    if (Elements != null && Elements.TryGetValue(elementId, out var positionedBuildingElement))
                        return Task.FromResult<IPositionedBuild>(positionedBuildingElement);
                    if (Attachments != null && Attachments.TryGetValue(elementId, out var positionedAttachment))
                        return Task.FromResult<IPositionedBuild>(positionedAttachment);
                }
                    break;
            }
            return Task.FromResult<IPositionedBuild>(null);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<bool> CheckStructureImpl(BuildType type, IPositionedBuild build)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {build.Id}, state: {build.State}, buildRecipeDef: {build.RecipeDef.____GetDebugAddress()}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (serverBuildingStructure == null)
            {
                BuildUtils.Error?.Report($"serverBuildingStructure is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(false);
            }

            switch (build)
            {
                case IPositionedBuildingElement positionedBuildingElement:
                    if (Elements != null)
                    {
                        if (positionedBuildingElement.RecipeDef != null)
                        {
                            var result = serverBuildingStructure.Check(positionedBuildingElement.Block.x,
                                positionedBuildingElement.Block.y,
                                positionedBuildingElement.Block.z,
                                positionedBuildingElement.RecipeDef,
                                positionedBuildingElement.Face,
                                positionedBuildingElement.Side,
                                false,
                                false);
                            if (result.Reason == CanPlaceData.REASON_OK)
                            {
                                return Task.FromResult(true);
                            }
                            BuildUtils.Error?.Report($"Error reason: {result.Reason}|{result.ReasonDescription}, block: [{positionedBuildingElement.Block.x}, {positionedBuildingElement.Block.y}, {positionedBuildingElement.Block.z}], face: {positionedBuildingElement.Face}, side: {positionedBuildingElement.Side}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        }
                        else
                        {
                            BuildUtils.Error?.Report($"RecipeDef is null, block: [{positionedBuildingElement.Block.x}, {positionedBuildingElement.Block.y}, {positionedBuildingElement.Block.z}], face: {positionedBuildingElement.Face}, side: {positionedBuildingElement.Side}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        }
                    }
                    return Task.FromResult(false);
                case IPositionedAttachment positionedAttachment:
                    return Task.FromResult(true);
                case null:
                    BuildUtils.Error?.Report($"build is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
                default:
                    BuildUtils.Error?.Report($"invalid BuildType: {build.GetType()}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
            }
            return Task.FromResult(false);
        }

        public Task<bool> AddStructureImpl(BuildType type, IPositionedBuild build)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {build.Id}, state: {build.State}, buildRecipeDef: {build.RecipeDef.____GetDebugAddress()}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (serverBuildingStructure == null)
            {
                BuildUtils.Error?.Report($"serverBuildingStructure is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return Task.FromResult(false);
            }
            switch (type)
            {
                case BuildType.BuildingElement:
                {
                    var positionedBuildingElement = build as IPositionedBuildingElement;
                    if (positionedBuildingElement != null)
                    {
                        var result = serverBuildingStructure.Set(positionedBuildingElement.Id,
                                                                 positionedBuildingElement.Block.x,
                                                                 positionedBuildingElement.Block.y,
                                                                 positionedBuildingElement.Block.z,
                                                                 positionedBuildingElement.RecipeDef,
                                                                 positionedBuildingElement.Face,
                                                                 positionedBuildingElement.Side);
                        return Task.FromResult(result == CanPlaceData.REASON_OK);
                    }
                    return Task.FromResult(false);
                }
                case BuildType.BuildingAttachment:
                {
                    return Task.FromResult(true);
                }
                default:
                    BuildUtils.Error?.Report($"invalid BuildType: {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
            }
            return Task.FromResult(false);
        }

        public async Task<List<KeyValuePair<BuildType, Guid>>> RemoveStructureImpl(BuildType type, Guid elementId)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {elementId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new List<KeyValuePair<BuildType, Guid>>();
            if (serverBuildingStructure == null)
            {
                BuildUtils.Error?.Report($"serverBuildingStructure is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return result;
            }
            var build = await TryGetValueImpl(type, elementId);
            switch (build)
            {
                case IPositionedBuildingElement positionedBuildingElement:
                {
                    //TODO building, attachment suppport
                    var typeToRemove = BuildType.BuildingElement;
                    var elementsToRemove = serverBuildingStructure.Clear(positionedBuildingElement.Id,
                                                                         positionedBuildingElement.Block.x,
                                                                         positionedBuildingElement.Block.y,
                                                                         positionedBuildingElement.Block.z,
                                                                         positionedBuildingElement.RecipeDef,
                                                                         positionedBuildingElement.Face,
                                                                         positionedBuildingElement.Side,
                                                                         true);
                    foreach (var elementToRemove in elementsToRemove)
                    {
                        var buildToRemove = TryGetValueImpl(typeToRemove, elementToRemove.Id);
                        if (buildToRemove.Result != null)
                        {
                            var positionedBuildingElementToRemove = buildToRemove.Result as IPositionedBuildingElement;
                            if (positionedBuildingElementToRemove != null)
                            {
                                positionedBuildingElementToRemove.State = positionedBuildingElement.State;
                                positionedBuildingElementToRemove.BuildTimestamp = 0;
                                positionedBuildingElementToRemove.BuildTime = 0;
                                positionedBuildingElementToRemove.Depth = elementToRemove.Depth;
                                result.Add(new KeyValuePair<BuildType, Guid>(typeToRemove, elementToRemove.Id));
                            }
                        }
                    }
                }
                    break;
                
                case IPositionedAttachment _:
                    result.Add(new KeyValuePair<BuildType, Guid>(BuildType.BuildingAttachment, elementId));
                    break;
                
                case null:
                    BuildUtils.Error?.Report($"build is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
                default:
                    BuildUtils.Error?.Report($"invalid BuildType: {build.GetType()}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
            }
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<bool> AddElementImpl(BuildType type, IPositionedBuild build)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {build.Id}, state: {build.State}, buildRecipeDef: {build.RecipeDef.____GetDebugAddress()}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            switch (type)
            {
                case BuildType.BuildingElement:
                {
                    var positionedBuildingElement = build as IPositionedBuildingElement;
                    if ((Elements != null) && (positionedBuildingElement != null))
                    {
                        Elements.Add(build.Id, positionedBuildingElement);
                        return Task.FromResult(true);
                    }
                    break;
                }
                case BuildType.BuildingAttachment:
                {
                    var positionedAttachment = build as IPositionedAttachment;
                    if ((Attachments != null) && (positionedAttachment != null))
                    {
                        Attachments.Add(build.Id, positionedAttachment);
                        return Task.FromResult(true);
                    }
                    break;
                }
                default:
                    BuildUtils.Error?.Report($"invalid BuildType: {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
            }
            return Task.FromResult(false);
        }

        public async Task<bool> RemoveElementsImpl(List<KeyValuePair<BuildType, Guid>> elements)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, elements: {elements.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if ((elements != null) && (elements.Count > 0))
            {
                foreach (var keyValuePair in elements)
                {
                    var element = TryGetValueImpl(keyValuePair.Key, keyValuePair.Value).Result;
                    if (element != null)
                    {
                        BuildUtils.Debug?.Report(true, $"Remove element, place: {this.Id}, buildType: {keyValuePair.Key}, elementId: {keyValuePair.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        if (element.BuildToken != null)
                        {
                            element.BuildToken.Cancel(EntitiesRepository);
                            element.BuildToken = null;
                        }
                        switch (keyValuePair.Key)
                        {
                            case BuildType.BuildingElement:
                            {
                                Elements.Remove(keyValuePair.Value);
                                break;
                            }
                            case BuildType.BuildingAttachment:
                            {
                                Attachments.Remove(keyValuePair.Value);
                                break;
                            }
                            default:
                                BuildUtils.Error?.Report($"invalid BuildType: {keyValuePair.Key}, placeId: {Id}, elementId: {keyValuePair.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                                return false;
                        }
                    }
                    else
                    {
                        BuildUtils.Error?.Report($"element is not found, placeId: {Id}, elementType: {keyValuePair.Key}, elementId: {keyValuePair.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return false;
                    }
                }
            }
            var isEmpty = (Elements.Count == 0) && (Attachments.Count == 0);
            if (isEmpty && RemoveIfEmpty)
            {
                var repository = EntitiesRepository;
                var id = Id;
                BuildUtils.Debug?.Report(true, $"Destroy IBuildingPlace, place: {id}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                AsyncUtils.RunAsyncTask(async () => await EntitiesRepository.Destroy<IBuildingPlace>(id), repository);
            }
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<ChainCancellationToken> StartChainImpl(BuildType type, float tick, int count, Guid elementId)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {elementId}, tick: {tick}, count: {count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            switch (type)
            {
                case BuildType.Any:
                case BuildType.BuildingElement:
                case BuildType.BuildingAttachment:
                {
                    return Task.FromResult(this.Chain().DelayCount(tick, count).OnProgress(type, elementId).Run() ?? null);
                }
                default:
                    BuildUtils.Error?.Report($"invalid BuildType: {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    break;
            }
            return Task.FromResult<ChainCancellationToken>(null);
        }

        public Task<bool> CancelChainImpl(BuildType type, ChainCancellationToken token)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (token != null)
            {
                switch (type)
                {
                    case BuildType.Any:
                    case BuildType.BuildingElement:
                    case BuildType.BuildingAttachment:
                    {
                        token.Cancel(EntitiesRepository);
                        return Task.FromResult(true);
                    }
                    default:
                        BuildUtils.Error?.Report($"invalid BuildType: {type}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        break;
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> OnProgressImpl(BuildType type, Guid elementId) => BuildPlace.OnProgress(type, elementId);

        public Task<bool> RemoveDelayImpl(List<KeyValuePair<BuildType, Guid>> elements) => BuildPlace.RemoveDelay(elements);

        public Task<bool> RemoveChainImpl(List<KeyValuePair<BuildType, Guid>> elements)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, elements: {elements.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            this.Chain().RemoveDelay(elements).Run();
            return Task.FromResult(true);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private Task Items_OnItemAddedOrUpdated(int arg1, SharedCode.DeltaObjects.ISlotItem arg2)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, arg1: {arg1}, arg2: {arg2}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (BuildToken != null)
            {
                //TODO Building: Cancellation Token resume
                //if (State == BuildState.Paused)
                //{
                //    BuildToken.Resume();
                //}
            }
            return Task.CompletedTask;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<bool> StartPlaceImpl(bool instant)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, instant: {instant}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            RemoveIfEmpty = instant;
            if (instant)
            {
                BuildTimestamp = 0;
                BuildTime = 0;
                State = BuildState.Completed;
                return Task.FromResult(true);
            }
            else
            {
                //TODO Building: Cancellation Token must be null at start
                //if (BuildToken != null)
                //{
                //    Logger.IfError()?.Message($"BuildingPlace.StartPlace(), message: BuildToken already in use").Write();
                //    return Task.FromResult(false);
                //}


                var buildingPlaceDef = Def as BuildingPlaceDef;
                if (buildingPlaceDef == null)
                {
                    BuildUtils.Error?.Report($"Def is null or not a BuildingPlaceDef", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    return Task.FromResult(false);
                }

                //TODO Building: Cancellation Token resume
                //Resources.Items.OnItemAddedOrUpdated += Items_OnItemAddedOrUpdated;

                State = BuildState.InProgress;
                BuildTimestamp = SyncTime.Now;
                BuildTime = buildingPlaceDef.TimerDef.Target.BuildTime;
                BuildToken = this.Chain().DelayCount(buildingPlaceDef.TimerDef.Target.BuildTick, (int)(BuildTime) + 2).OnProgressPlace().Run();

                return Task.FromResult(true);
            }
        }

        public async Task<bool> CancelPlaceImpl()
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (BuildToken == null)
            {
                BuildUtils.Error?.Report($"BuildToken is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            BuildToken.Cancel(EntitiesRepository);
            BuildToken = null;
            await FinishPlaceImpl(BuildResult.Cancel);

            return true;
        }

        public async Task<bool> OnProgressPlaceImpl()
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (BuildToken == null)
            {
                BuildUtils.Error?.Report($"BuildToken is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            //TODO building: списываем ресурсы
            //if (не списали)
            //{
            //    BuildToken.Pause();
            //}

            if ((SyncTime.Now - BuildTimestamp) >= SyncTime.FromSeconds(BuildTime))
            {
                BuildToken.Cancel(EntitiesRepository);
                BuildToken = null;
                await FinishPlaceImpl(BuildResult.Success);
            }

            return true;
        }

        public Task FinishPlaceImpl(BuildResult result)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, result: {result}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (result == BuildResult.Success)
            {
                BuildTimestamp = 0;
                BuildTime = 0;
                State = BuildState.Completed;
            }
            else if (result == BuildResult.Cancel)
            {
                //TODO building: вернуть ресурсы, Building Place
                BuildTimestamp = 0;
                BuildTime = 0;
                State = BuildState.Canceled;

                var repository = EntitiesRepository;
                var id = Id;
                BuildUtils.Debug?.Report(true, $"Destroy IBuildingPlace, place: {id}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                AsyncUtils.RunAsyncTask(async () => { await repository.Destroy<IBuildingPlace>(id); }, repository);
            }
            return Task.CompletedTask;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<bool> NameSetImpl(string name)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, name: {name}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            Name = name;
            return Task.FromResult(true);
        }

        public Task<bool> PrefabSetImpl(string prefab)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, prefab: {prefab}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            Prefab = prefab;
            return Task.FromResult(true);
        }

        public Task<bool> StateSetImpl(BuildState state)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, state: {state}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            State = state;
            return Task.FromResult(true);
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            MovementSync.GridSyncType = typeof(BuildingPlaceGridSyncType);
        }
    }
}