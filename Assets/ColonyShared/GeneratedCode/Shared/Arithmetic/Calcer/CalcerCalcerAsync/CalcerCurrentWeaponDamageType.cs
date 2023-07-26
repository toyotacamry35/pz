using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using ResourceSystem.Arithmetic.Templates.Calcers;
using SharedCode.EntitySystem;

namespace Assets.Src.Arithmetic
{
    [UsedImplicitly]
    public class CalcerCurrentWeaponDamageType : ICalcerBinding<CalcerCurrentWeaponDamageTypeDef,string>
    {
        public ValueTask<string> Calc(CalcerCurrentWeaponDamageTypeDef def, CalcerContext ctx)
        {
            var specificStats = ctx.TryGetEntity<IHasSpecificStatsClientFull>(ReplicationLevel.ClientFull);
            var itemsStatAccumulator = ctx.TryGetEntity<IHasItemsStatsAccumulatorClientFull>(ReplicationLevel.ClientFull);
            return new ValueTask<string>((DamagePipelineHelper.GetCurrentWeaponDamageType(itemsStatAccumulator?.ItemsStatsAccumulator, specificStats))?.ToString());
        }

        public IEnumerable<StatResource> CollectStatNotifiers(CalcerCurrentWeaponDamageTypeDef def) { yield return null; }
    }
}