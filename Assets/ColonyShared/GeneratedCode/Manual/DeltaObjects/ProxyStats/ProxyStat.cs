using Assets.Src.Aspects.Impl.Stats;
using SharedCode.EntitySystem;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class ProxyStat
    {
        public Task OnDatabaseLoadImpl() { return Task.CompletedTask; }

        public ValueTask<float> GetValueImpl()
        {
            return new ValueTask<float>(ValueCache);
        }

        public Task ProxySubscribeImpl(PropertyAddress containerAddress) { return Task.CompletedTask; }
        public ValueTask InitializeImpl(StatDef statDef, bool resetState) { return new ValueTask(); }
        public ValueTask<bool> RecalculateCachesImpl(bool calcersOnly) { return new ValueTask<bool>(false); }

        public override string ToString()
        {
            return $"{ValueCache} [{(LimitMinCache < -100000 ? "-∞" : LimitMinCache + ""),6:F1}; {(LimitMaxCache > 100000 ? "+∞" : LimitMaxCache + ""),6:F1}]";
        }
    }
}
