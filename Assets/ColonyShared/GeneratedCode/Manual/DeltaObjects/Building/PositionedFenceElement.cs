using SharedCode.Utils;
using System.Reflection;

namespace GeneratedCode.DeltaObjects
{
    public partial class PositionedFenceElement
    {
//        private IPositionHistory _positionHistory;
//        public Task<IPositionHistory> GetPositionHistory() => Task.FromResult(_positionHistory ?? (_positionHistory = new BuildingElementPositionHistory(this)));

        protected override void constructor()
        {
            BuildUtils.Debug?.Report(true, $"", MethodBase.GetCurrentMethod().DeclaringType.Name);

            PositionHistory = new BuildingElementPositionHistory(this);
        }
    }
}
