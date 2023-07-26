using System;
using System.Threading.Tasks;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using System.Collections.Generic;
using SharedCode.Entities.Service;
using System.Reflection;

namespace GeneratedCode.DeltaObjects
{
    public partial class BuildPlace : IBuildPlaceImplementRemoteMethods
    {
        public static IPositionedBuild CreatePositionedBuild(BuildType type, BuildRecipeDef buildRecipeDef, CreateBuildElementData data, OuterRef<IEntity> owner)
        {
            BuildUtils.Debug?.Report(true, $"type: {type}, buildRecipeDef: {buildRecipeDef.____GetDebugAddress()}, data: {data}, owner: {owner.Guid}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            if (type == BuildType.BuildingElement)
            {
                var buildingElementDef = buildRecipeDef.ElementDef.Target as BuildingElementDef;
                var createBuildingElementData = data as CreateBuildingElementData;
                if ((buildingElementDef != null) && (createBuildingElementData != null))
                {
                    var build = new PositionedBuildingElement();
                    //
                    build.Id = Guid.NewGuid();
                    build.RecipeDef = buildRecipeDef;
                    //
                    build.Owner = owner;
                    //
                    build.Position = createBuildingElementData.Position;
                    build.Rotation = createBuildingElementData.Rotation;
                    build.Health = createBuildingElementData.Health;
                    build.Visual = createBuildingElementData.Visual;
                    build.Interaction = createBuildingElementData.Interaction;
                    build.Block = createBuildingElementData.Block;
                    build.Type = createBuildingElementData.Type;
                    build.Face = createBuildingElementData.Face;
                    build.Side = createBuildingElementData.Side;
                    //
                    build.Depth = -1;
                    //
                    return build;
                }
            }
            else if (type == BuildType.FenceElement)
            {
                var fenceElementDef = buildRecipeDef.ElementDef.Target as FenceElementDef;
                var createFenceElementData = data as CreateFenceElementData;
                if ((fenceElementDef != null) && (createFenceElementData != null))
                {
                    var build = new PositionedFenceElement();
                    //
                    build.Id = Guid.NewGuid();
                    build.RecipeDef = buildRecipeDef;
                    //
                    build.Owner = owner;
                    //
                    build.Position = createFenceElementData.Position;
                    build.Rotation = createFenceElementData.Rotation;
                    build.Health = createFenceElementData.Health;
                    build.Visual = createFenceElementData.Visual;
                    build.Interaction = createFenceElementData.Interaction;
                    //
                    build.Depth = -1;
                    //
                    return build;
                }
            }
            else if ((type == BuildType.BuildingAttachment) || (type == BuildType.FenceAttachment))
            {
                var attachmentDef = buildRecipeDef.ElementDef.Target as AttachmentDef;
                var createAttachmentData = data as CreateAttachmentData;
                if ((attachmentDef != null) && (createAttachmentData != null))
                {
                    var build = new PositionedAttachment();
                    //
                    build.Id = Guid.NewGuid();
                    build.RecipeDef = buildRecipeDef;
                    //
                    build.Owner = owner;
                    build.ParentElementId = createAttachmentData.ParentElementId;
                    //
                    build.Position = createAttachmentData.Position;
                    build.Rotation = createAttachmentData.Rotation;
                    build.Health = createAttachmentData.Health;
                    build.Visual = createAttachmentData.Visual;
                    build.Interaction = createAttachmentData.Interaction;
                    //
                    build.Depth = -1;
                    //
                    return build;
                }
            }
            return null;
        }

        private async Task<List<KeyValuePair<BuildRecipeDef, float>>> CreateBuildsToRemove(List<KeyValuePair<BuildType, Guid>> elementsToRemove)
        {
            BuildUtils.Debug?.Report(true, $"placeId: {parentEntity.Id}, elementsToRemove: {elementsToRemove.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var buildsToRemove = new List<KeyValuePair<BuildRecipeDef, float>>();

            var place = parentEntity as IBuildCollection;
            if (place == null)
            {
                BuildUtils.Error?.Report($"place is null or not a IBuildCollection", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return buildsToRemove;
            }

            foreach (var elementToRemove in elementsToRemove)
            {
                var buildToRemove = await place.TryGetValue(elementToRemove.Key, elementToRemove.Value);
                if (buildToRemove != null)
                {
                    var factor = 1.0f;
                    if (buildToRemove.BuildToken != null)
                    {
                        factor = (SyncTime.Now - buildToRemove.BuildTimestamp - SyncTime.FromSeconds(BuildUtils.BuildParamsDef.ReclaimGap)) * 1.0f / SyncTime.FromSeconds(buildToRemove.BuildTime - BuildUtils.BuildParamsDef.ReclaimGap);
                    }
                    buildsToRemove.Add(new KeyValuePair<BuildRecipeDef, float>(buildToRemove.RecipeDef, factor));
                }
            }
            return buildsToRemove;
        }

        public async Task<bool> RemoveDelayImpl(List<KeyValuePair<BuildType, Guid>> elements)
        {
            BuildUtils.Debug?.Report(true, $"placeId: {parentEntity.Id}, elements: {elements.Count}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var place = parentEntity as IBuildCollection;
            if (place == null)
            {
                BuildUtils.Error?.Report($"place is null or not a IBuildCollection", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            var result = await place.RemoveElements(elements);
            if (!result)
            {
                BuildUtils.Error?.Report($"message: can't remove elements", MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
            return result;
        }

        public async Task<bool> CheckImpl(BuildType type, IPositionedBuildWrapper buildWrapper)
        {
            BuildUtils.Debug?.Report(true, $"placeId: {parentEntity.Id}, type: {type}, elementId: {buildWrapper.PositionedBuild.Id}, state: {buildWrapper.PositionedBuild.State}, buildRecipeDef: {buildWrapper.PositionedBuild.RecipeDef.____GetDebugAddress()}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var build = buildWrapper?.PositionedBuild;
            if (build == null)
            {
                BuildUtils.Error?.Report($"type: {type}, element is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            var place = parentEntity as IBuildCollection;
            if (place == null)
            {
                BuildUtils.Error?.Report($"place is null or not a IBuildCollection", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            if (await place.ContainsKey(type, build.Id))
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, element already built", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            var buildRecipeDef = build.RecipeDef;
            if (buildRecipeDef == null)
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, element has null or not a buildRecipeDef", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            if (!await place.CheckStructure(type, build))
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, check element error", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            return true;
        }

        public async Task<bool> StartImpl(BuildType type, IPositionedBuildWrapper buildWrapper)
        {
            BuildUtils.Debug?.Report(true, $"placeId: {parentEntity.Id}, type: {type}, elementId: {buildWrapper.PositionedBuild.Id}, state: {buildWrapper.PositionedBuild.State}, buildRecipeDef: {buildWrapper.PositionedBuild.RecipeDef.____GetDebugAddress()}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var place = parentEntity as IBuildCollection;
            if (place == null)
            {
                BuildUtils.Error?.Report($"type: {type}, place is null or not a IBuildCollection", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            var build = buildWrapper?.PositionedBuild;
            if (build == null)
            {
                BuildUtils.Error?.Report($"type: {type}, element is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            if (await place.ContainsKey(type, build.Id))
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, element is already built", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            var buildRecipeDef = build.RecipeDef;
            if (buildRecipeDef == null)
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, element has no buildRecipeDef", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            if (!await place.AddStructure(type, build))
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, add structure error", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            if (!await place.AddElement(type, build))
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, add element error", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }

            //TODO Building: to constructor
            //buildingPosition.BuildingContainer.Items.OnItemAddedOrUpdated += async (int arg1, ISlotItem arg2) =>
            //{
            //    await Items_OnItemAddedOrUpdatedForElement(arg1, arg2, buildingPosition.Element.Id);
            //};

            build.State = BuildState.InProgress;

            build.BuildTimestamp = SyncTime.Now;
            build.BuildTime = BuildUtils.BuildParamsDef.ReclaimGap + buildRecipeDef.TimerDef.Target.BuildTime;
            build.BuildToken = await place.StartChain(type, buildRecipeDef.TimerDef.Target.BuildTick, (int)build.BuildTime + 2, build.Id);
            if (build.BuildToken == null)
            {
                BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, can't start chain", MethodBase.GetCurrentMethod().DeclaringType.Name);
                build.State = BuildState.Canceled;
                build.Depth = 0;
                var elementsToremove = await place.RemoveStructure(type, build.Id);
                if (!await place.RemoveChain(elementsToremove))
                {
                    BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, can't remove elements", MethodBase.GetCurrentMethod().DeclaringType.Name);
                }
                return false;
            }

            return true;
        }

        public async Task<bool> OnProgressImpl(BuildType type, Guid elementId)
        {
            //BuildUtils.Debug?.Report(true, $"placeId: {placeId}, type: {type}, elementId: {elementId}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var place = parentEntity as IBuildCollection;
            if (place == null)
            {
                BuildUtils.Error?.Report($"Type: {type}, Id: {elementId}, place is not a IBuildCollection", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return false;
            }
            IPositionedBuild build = await place.TryGetValue(type, elementId);
            if (build == null)
            {
                BuildUtils.Error?.Report($"Type: {type}, Id: {elementId}, element is not found, probably already removed", MethodBase.GetCurrentMethod().DeclaringType.Name);
                //TODO building: TerminateCurrentChain realization
                //ChainHelper.TerminateCurrentChain();
                return false;
            }

            if (build.BuildToken != null)
            {
                if ((SyncTime.Now - build.BuildTimestamp) >= SyncTime.FromSeconds(build.BuildTime))
                {
                    if (await place.CancelChain(type, build.BuildToken))
                    {
                        build.BuildToken = null;
                        build.State = BuildState.Completed;
                        build.BuildTimestamp = 0;
                        build.BuildTime = 0;
                        return true;
                    }
                    else
                    {
                        BuildUtils.Error?.Report($"Type: {type}, Id: {elementId}, can't finish chain", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<OperationResultEx> OperateImpl(BuildType type, Guid callerId, Guid elementId, OperationData data)
        {
            BuildUtils.Debug?.Report(true, $"placeId: {parentEntity.Id}, type: {type}, callerId: {callerId}, elementId: {elementId}, data: {data}", MethodBase.GetCurrentMethod().DeclaringType.Name);

            var place = parentEntity as IBuildCollection;
            if (place == null)
            {
                BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, place is not IBuildCollection", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_InvalidPlace, ElementId = elementId, OperationData = null };
            }
            if (data == null)
            {
                BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, data is null", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_InvalidOperationData, ElementId = elementId, OperationData = null };
            }

            IPositionedBuild build = await place.TryGetValue(type, elementId);
            if (build == null)
            {
                BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, element is not found", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_InvalidElement, ElementId = elementId, OperationData = null };
            }
            switch (data.Type)
            {
                case OperationType.Damage:
                {
                    float damage = 0.0f;
                    if (BuildUtils.CheatDamageEnable)
                    {
                        damage = BuildUtils.CheatDamageValue;
                    }
                    else
                    {
                        var damageData = data as DamageData;
                        if (damageData == null)
                        {
                            BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                            return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_InvalidOperationData, ElementId = elementId, OperationData = null };
                        }
                        damage = damageData.DestructionPower > build.RecipeDef.DestructionPowerRequired ? damageData.Damage : 0;
                        if (BuildUtils.BuildParamsDef.RestrictHits)
                        {
                            damage = Math.Max(BuildUtils.BuildParamsDef.MinHit, Math.Min(BuildUtils.BuildParamsDef.MaxHit, damage));
                        }
                    }
                    build.Health = Math.Max(0.0f, build.Health - damage);
                    var operationDataWithRecipe = new OperationDataWithRecipe { Type = OperationType.WithRecipe, Recipe = build.RecipeDef};
                    if (build.Health > 0.0f)
                    {
                        return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Success, ElementId = elementId, OperationData = operationDataWithRecipe };
                    }
                    build.State = BuildState.Destroyed;
                    build.Depth = 0;
                    var elementsToRemove = await place.RemoveStructure(type, build.Id);
                    var buildsToRemove = await CreateBuildsToRemove(elementsToRemove);
                    if (buildsToRemove.Count > 0)
                    {
                        var resourceOperationResult = await BuildResourceManager.SpawnResources(place as IEntity, build.Position, build.Rotation, buildsToRemove, ((IHasWorldSpaced)place).WorldSpaced.OwnWorldSpace.Guid);
                        if (resourceOperationResult.Result != ResourceOperationResultCode.Success)
                        {
                            BuildUtils.Error?.Report($"BuildResourceManager.SpawnResources() call, place: {place.GetType()}, owner: {build.Owner.Guid}, count: {buildsToRemove.Count}, result: {resourceOperationResult.Message}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        }
                    }
                    if (!await place.RemoveChain(elementsToRemove))
                    {
                        BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, can't remove elements", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    }
                    return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Success, ElementId = elementId, OperationData = operationDataWithRecipe };
                }
                case OperationType.Interact:
                {
                    var interactData = data as InteractData;
                    if (interactData == null)
                    {
                        BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_InvalidOperationData, ElementId = elementId, OperationData = null };
                    }
                    if (callerId != build.Owner.Guid)
                    {
                        BuildUtils.Message?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, invalid caller", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Success, ElementId = elementId, OperationData = null };
                    }
                    build.Interaction = interactData.Interaction;
                    return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Success, ElementId = elementId, OperationData = null };
                }
                case OperationType.Remove:
                {
                    var removeData = data as RemoveData;
                    if (removeData == null)
                    {
                        BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, invalid operation data", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_InvalidOperationData, ElementId = elementId, OperationData = null };
                    }
                    if (callerId != build.Owner.Guid)
                    {
                        BuildUtils.Message?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, invalid caller", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Success, ElementId = elementId, OperationData = null };
                    }
                    build.State = BuildState.Removed;
                    build.Depth = 0;
                    var elementsToRemove = await place.RemoveStructure(type, build.Id);
                    var buildsToRemove = await CreateBuildsToRemove(elementsToRemove);
                    if (buildsToRemove.Count > 0)
                    {
                        var resourceOperationResult = await BuildResourceManager.ReclaimResources(place as IEntity, build.Owner, buildsToRemove);
                        if (resourceOperationResult.Result != ResourceOperationResultCode.Success)
                        {
                            BuildUtils.Error?.Report($"BuildResourceManager.ReclaimResources() call, place: {place.GetType()}, owner: {build.Owner.Guid}, count: {buildsToRemove.Count}, result: {resourceOperationResult.Message}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                        }
                    }
                    if (!await place.RemoveChain(elementsToRemove))
                    {
                        BuildUtils.Error?.Report($"type: {type}, Id: {build.Id}, can't remove elements", MethodBase.GetCurrentMethod().DeclaringType.Name);
                    }
                    return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Success, ElementId = elementId, OperationData = null };
                }
            }
            BuildUtils.Error?.Report($"type: {type}, callerId: {callerId}, Id: {elementId}, operation: {data.Type}, unknown operation", MethodBase.GetCurrentMethod().DeclaringType.Name);
            return new OperationResultEx { Type = data.Type, BuildType = type, Result = ErrorCode.Error_UnknownOperationType, ElementId = elementId, OperationData = null };
        }
    }
}