using System.Threading.Tasks;
using SharedCode.Logging;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;

namespace GeneratedCode.DeltaObjects
{
    public partial class LogableEntity
    {
    // --- Implements: ----------------------------------------------------------
    #region Implements

        public Task SetCurveLoggerEnableImpl(bool val)
        {
            IsCurveLoggerEnable = val;
            Log.Logger.IfInfo()?.Message(ParentEntityId, $"Curve logger {(val ? "enabled" : "disabled")}").Write();
            return Task.CompletedTask;
        }

    #endregion Implements

    }
}
