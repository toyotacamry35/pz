using SharedCode.DeltaObjects.Building;
using SharedCode.Entities;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public class BuildingElementPositionHistory : IPositionHistory
    {
        private readonly IPositionedBuild _pos;

        public BuildingElementPositionHistory(IPositionedBuild pos)
        {
            _pos = pos;
        }

        public Transform GetTransformAt(long timestamp) => new Transform(_pos.Position, _pos.Rotation, Vector3.one);
    }
}