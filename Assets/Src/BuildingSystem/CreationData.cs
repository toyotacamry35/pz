using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using System;

namespace Assets.Src.BuildingSystem
{
    public interface ICreationData
    {
        BuildType Type { get; }
        Guid PlaceId { get; }
        BuildRecipeDef BuildRecipeDef { get; }
        CreateBuildElementData Data { get; }
    }

    public class CreationData : ICreationData
    {
        public BuildType Type { get; private set; }

        public Guid PlaceId { get; private set; }

        public BuildRecipeDef BuildRecipeDef { get; private set; }

        public CreateBuildElementData Data { get; private set; }

        public CreationData(BuildType type, Guid placeId, BuildRecipeDef bildRecipeDef, CreateBuildElementData data)
        {
            Type = type;
            PlaceId = placeId;
            BuildRecipeDef = bildRecipeDef;
            Data = data;
        }
    }
}
