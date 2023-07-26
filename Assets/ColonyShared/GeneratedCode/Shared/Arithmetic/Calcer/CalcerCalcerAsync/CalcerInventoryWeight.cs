using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Stats;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using NLog;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerInventoryWeight : ICalcerBinding<CalcerInventoryWeightDef,float>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask<float> Calc(CalcerInventoryWeightDef def, CalcerContext ctx)
        {
            var character = ctx.TryGetEntity<IWorldCharacterClientFull>(ReplicationLevel.ClientFull);
            if (character != null)
            {
                if (character.CraftEngine == null)
                {
                    Logger.IfError()?.Message("character.CraftEngine == null characterId {0}", character.Id).Write();
                    return 0.0f;
                }

                using (var container2 = await ctx.Repository.Get<ICraftEngineClientFull>(character.CraftEngine.Id))
                {
                    var craftEngine = container2?.Get<ICraftEngineClientFull>(character.CraftEngine.Id);
                    var res = (await (character?.Inventory?.GetTotalWeight() ?? Task.FromResult(0.0f))) +
                              (await (character?.Doll?.GetTotalWeight() ?? Task.FromResult(0.0f))) +
                              (await (craftEngine?.IntermediateCraftContainer?.GetTotalWeight() ?? Task.FromResult(0.0f)));
                    return res;
                }
            }
            return 0;
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerInventoryWeightDef calcer) { yield return null; }
    }
}
