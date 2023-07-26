using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using System;
using System.Collections.Generic;

namespace Assets.Src.BuildingSystem
{
    public class PropertyReplica
    {
        public List<string> PropertyNames { get; } = new List<string>();
    }

    public class PlaceReplica : PropertyReplica
    {
        public BuildState State { get; set; } = BuildState.None;
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public Vector3 Scale { get; set; } = Vector3.one;
        public IEntityObjectDef Def { get; set; } = null;
    }

    public class ElementReplica : PropertyReplica
    {
        public BuildState State { get; set; } = BuildState.None;
        public long BuildTimestamp { get; set; } = 0;
        public float BuildTime { get; set; } = 0.0f;
        public float Health { get; set; } = 0.0f;
        public int Visual { get; set; } = -1;
        public int Interaction { get; set; } = -1;

        public BuildRecipeDef RecipeDef { get; set; } = null;
        public Guid OwnerId { get; set; } = Guid.Empty;
        public Guid ElementId { get; set; } = Guid.Empty;

        public int Depth { get; set; } = -1;
    }

    public class BuildingPlaceAlwaysReplica : PlaceReplica
    {
        public long BuildTimestamp { get; set; } = 0;
        public float BuildTime { get; set; } = 0.0f;
        public int Visual { get; set; } = -1;
        public OuterRef<IEntity> Owner { get; set; } = OuterRef<IEntity>.Invalid;

        public Dictionary<Guid, KeyValuePair<PositionedBuildingElementAlwaysReplica, IPositionedBuildingElementAlways>> Elements { get; } = new Dictionary<Guid, KeyValuePair<PositionedBuildingElementAlwaysReplica, IPositionedBuildingElementAlways>>();
    }

    public class FencePlaceAlwaysReplica : PlaceReplica
    {
        public Dictionary<Guid, KeyValuePair<PositionedFenceElementAlwaysReplica, IPositionedFenceElementAlways>> Elements { get; } = new Dictionary<Guid, KeyValuePair<PositionedFenceElementAlwaysReplica, IPositionedFenceElementAlways>>();
    }

    public class PositionedBuildingElementAlwaysReplica : ElementReplica
    {
        public Vector3Int Block { get; set; } = Vector3Int.zero;
        public BuildingElementType Type { get; set; } = BuildingElementType.Unknown;
        public BuildingElementFace Face { get; set; } = BuildingElementFace.Unknown;
        public BuildingElementSide Side { get; set; } = BuildingElementSide.Unknown;
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
    }

    public class PositionedFenceElementAlwaysReplica : ElementReplica
    {
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
    }

    public static class ReplicaFactory
    {
        public static PropertyReplica Get(IDeltaObject _bind)
        {
            {
                var bind = _bind as IBuildingPlaceAlways;
                if (bind != null)
                {
                    var replica = new BuildingPlaceAlwaysReplica();
                    replica.PropertyNames.Add(nameof(bind.State));
                    replica.PropertyNames.Add(nameof(bind.Def));

                    replica.PropertyNames.Add(nameof(bind.BuildTimestamp));
                    replica.PropertyNames.Add(nameof(bind.BuildTime));
                    replica.PropertyNames.Add(nameof(bind.Visual));

                    replica.State = bind.State;
                    replica.Position = bind.MovementSync.Position;
                    replica.Rotation = bind.MovementSync.Rotation;
                    replica.Scale = bind.MovementSync.Scale;
                    replica.Def = bind.Def;

                    replica.BuildTimestamp = bind.BuildTimestamp;
                    replica.BuildTime = bind.BuildTime;
                    replica.Visual = bind.Visual;
                    replica.Owner = bind.OwnerInformation.Owner;

                    foreach( var element in bind.Elements)
                    {
                        var elementReplica = Get(element.Value) as PositionedBuildingElementAlwaysReplica;
                        if (elementReplica != null)
                        {
                            replica.Elements.Add(element.Key, new KeyValuePair<PositionedBuildingElementAlwaysReplica, IPositionedBuildingElementAlways>(elementReplica, element.Value));
                        }
                    }

                    return replica;
                }
            }
            {
                var bind = _bind as IFencePlaceAlways;
                if (bind != null)
                {
                    var replica = new FencePlaceAlwaysReplica();
                    replica.PropertyNames.Add(nameof(bind.State));
                    replica.PropertyNames.Add(nameof(bind.Def));

                    replica.State = bind.State;
                    replica.Position = bind.MovementSync.Position;
                    replica.Rotation = bind.MovementSync.Rotation;
                    replica.Scale = bind.MovementSync.Scale;
                    replica.Def = bind.Def;

                    foreach (var element in bind.Elements)
                    {
                        var elementReplica = Get(element.Value) as PositionedFenceElementAlwaysReplica;
                        if (elementReplica != null)
                        {
                            replica.Elements.Add(element.Key, new KeyValuePair<PositionedFenceElementAlwaysReplica, IPositionedFenceElementAlways>(elementReplica, element.Value));
                        }
                    }
                    return replica;
                }
            }
            {
                var bind = _bind as IPositionedBuildingElementAlways;
                if (bind != null)
                {
                    var replica = new PositionedBuildingElementAlwaysReplica();
                    replica.PropertyNames.Add(nameof(bind.State));
                    replica.PropertyNames.Add(nameof(bind.BuildTimestamp));
                    replica.PropertyNames.Add(nameof(bind.BuildTime));
                    replica.PropertyNames.Add(nameof(bind.Health));
                    replica.PropertyNames.Add(nameof(bind.Visual));
                    replica.PropertyNames.Add(nameof(bind.Interaction));

                    replica.PropertyNames.Add(nameof(bind.Block));
                    replica.PropertyNames.Add(nameof(bind.Type));
                    replica.PropertyNames.Add(nameof(bind.Face));
                    replica.PropertyNames.Add(nameof(bind.Side));
                    replica.PropertyNames.Add(nameof(bind.Position));
                    replica.PropertyNames.Add(nameof(bind.Rotation));
                    //
                    replica.PropertyNames.Add(nameof(bind.Depth));

                    replica.State = bind.State;
                    replica.BuildTimestamp = bind.BuildTimestamp;
                    replica.BuildTime = bind.BuildTime;
                    replica.Health = bind.Health;
                    replica.Visual = bind.Visual;
                    replica.Interaction = bind.Interaction;

                    replica.RecipeDef = bind.RecipeDef;
                    replica.OwnerId = bind.Owner.Guid;
                    replica.ElementId = bind.Id;

                    replica.Block = bind.Block;
                    replica.Type = bind.Type;
                    replica.Face = bind.Face;
                    replica.Side = bind.Side;
                    replica.Position = bind.Position;
                    replica.Rotation = bind.Rotation;

                    replica.Depth = bind.Depth;

                    return replica;
                }
            }
            {
                var bind = _bind as IPositionedFenceElementAlways;
                if (bind != null)
                {
                    var replica = new PositionedFenceElementAlwaysReplica();
                    replica.PropertyNames.Add(nameof(bind.State));
                    replica.PropertyNames.Add(nameof(bind.BuildTimestamp));
                    replica.PropertyNames.Add(nameof(bind.BuildTime));
                    replica.PropertyNames.Add(nameof(bind.Health));
                    replica.PropertyNames.Add(nameof(bind.Visual));
                    replica.PropertyNames.Add(nameof(bind.Interaction));

                    replica.PropertyNames.Add(nameof(bind.Position));
                    replica.PropertyNames.Add(nameof(bind.Rotation));

                    replica.PropertyNames.Add(nameof(bind.Depth));

                    replica.State = bind.State;
                    replica.BuildTimestamp = bind.BuildTimestamp;
                    replica.BuildTime = bind.BuildTime;
                    replica.Health = bind.Health;
                    replica.Visual = bind.Visual;
                    replica.Interaction = bind.Interaction;

                    replica.RecipeDef = bind.RecipeDef;
                    replica.OwnerId = bind.Owner.Guid;
                    replica.ElementId = bind.Id;

                    replica.Position = bind.Position;
                    replica.Rotation = bind.Rotation;

                    replica.Depth = bind.Depth;

                    return replica;
                }
            }
            return null;
        }
    }
}