using GeneratedCode.DeltaObjects.Chain;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.MovementSync;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    struct FencePlaceGridSyncType : IPointSpatialHashType
    {
        public int StaticCellSize => 300;

        public int StaticHeightSize => 400;

        public int StaticReplicationRadius => 300;

        public bool ShouldCheckForRadius => true;

        public IEntityObjectDef Def { get; set; }
    }

    public partial class FencePlace : IHookOnInit, IHookOnDatabaseLoad, IHookOnReplicationLevelChanged
    {
        protected override void constructor()
        {
            BuildUtils.Debug?.Report(true, $"", MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        public Task OnInit()
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            return Task.CompletedTask;
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            MovementSync.GridSyncType = typeof(FencePlaceGridSyncType);
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
                case BuildType.FenceElement:
                {
                    return Task.FromResult(Elements?.ContainsKey(elementId) ?? false);
                }
                case BuildType.FenceAttachment:
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
                case BuildType.FenceElement:
                {
                    if (Elements != null)
                    {
                        IPositionedFenceElement positionedFenceElement;
                        if (Elements.TryGetValue(elementId, out positionedFenceElement))
                        {
                            return Task.FromResult<IPositionedBuild>(positionedFenceElement);
                        }
                    }
                    break;
                }
                case BuildType.FenceAttachment:
                {
                    if (Attachments != null)
                    {
                        IPositionedAttachment positionedAttachment;
                        if (Attachments.TryGetValue(elementId, out positionedAttachment))
                        {
                            return Task.FromResult < IPositionedBuild > (positionedAttachment);
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

            return Task.FromResult(true);
        }

        public Task<bool> AddStructureImpl(BuildType type, IPositionedBuild build)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {build.Id}, state: {build.State}, buildRecipeDef: {build.RecipeDef.____GetDebugAddress()}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            return Task.FromResult(true);
        }

        public async Task<List<KeyValuePair<BuildType, Guid>>> RemoveStructureImpl(BuildType type, Guid elementId)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {elementId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var result = new List<KeyValuePair<BuildType, Guid>>();
            var build = await TryGetValueImpl(type, elementId);
            switch (build)
            {
                case IPositionedFenceElement _:
                    result.Add(new KeyValuePair<BuildType, Guid>(BuildType.FenceElement, elementId));
                    break;
                case null:
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
                case BuildType.FenceElement:
                {
                    var positionedFenceElement = build as IPositionedFenceElement;
                    if ((Elements != null) && (positionedFenceElement != null))
                    {
                        Elements.Add(build.Id, positionedFenceElement);
                        return Task.FromResult(true);
                    }
                    break;
                }
                case BuildType.FenceAttachment:
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

        public Task<bool> RemoveElementsImpl(List<KeyValuePair<BuildType, Guid>> elements)
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
                            case BuildType.FenceElement:
                            {
                                Elements.Remove(keyValuePair.Value);
                                break;
                            }
                            case BuildType.FenceAttachment:
                            {
                                Attachments.Remove(keyValuePair.Value);
                                break;
                            }
                            default:
                                BuildUtils.Error?.Report($"invalid BuildType: {keyValuePair.Key}, placeId:{Id}, elementId:{keyValuePair.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                                return Task.FromResult(false);
                        }
                    }
                    else
                    {
                        BuildUtils.Error?.Report($"element is not found, placeId:{Id}, elementType: {keyValuePair.Key}, elementId: {keyValuePair.Value}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return Task.FromResult(false);
                    }
                }
            }
            return Task.FromResult(true);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Task<ChainCancellationToken> StartChainImpl(BuildType type, float tick, int count, Guid elementId)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, type: {type}, elementId: {elementId}, tick: {tick}, count: {count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            switch (type)
            {
                case BuildType.Any:
                case BuildType.FenceElement:
                case BuildType.FenceAttachment:
                {
                    return Task.FromResult(this.Chain().DelayCount(tick, count).OnProgress(type, elementId).Run() ?? null);
                }
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
                    case BuildType.FenceElement:
                    case BuildType.FenceAttachment:
                    {
                        token.Cancel(EntitiesRepository);
                        return Task.FromResult(true);
                    }
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> RemoveChainImpl(List<KeyValuePair<BuildType, Guid>> elements)
        {
            BuildUtils.Debug?.Report(true, $"place: {this.Id}, elements: {elements.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            this.Chain().RemoveDelay(elements).Run();
            return Task.FromResult(true);
        }

        public Task<bool> OnProgressImpl(BuildType type, Guid elementId) => BuildPlace.OnProgress(type, elementId);

        public Task<bool> RemoveDelayImpl(List<KeyValuePair<BuildType, Guid>> elements) => BuildPlace.RemoveDelay(elements);


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
    }
}