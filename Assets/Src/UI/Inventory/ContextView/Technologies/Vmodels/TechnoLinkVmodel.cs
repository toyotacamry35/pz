using System.Linq;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using SharedCode.Aspects.Science;

namespace Uins
{
    public class TechnoLinkVmodel : BindingVmodel
    {
        public readonly TechnologyItem SlaveTechnologyItem;
        public readonly TechnologyItem MasterTechnologyItem;

        public ReactiveProperty<bool> IsAvailableRp = new ReactiveProperty<bool>() {Value = false};


        //=== Ctor ============================================================

        public TechnoLinkVmodel(TechnologyItem slaveTechnologyItem, TechnologyDef masterTechnologyDef, TechnoItemVmodel[] itemVmodelsOfTab)
        {
            SlaveTechnologyItem = slaveTechnologyItem;
            SlaveTechnologyItem.Technology.Target.AssertIfNull(nameof(SlaveTechnologyItem));
            if (itemVmodelsOfTab.AssertIfNull(nameof(itemVmodelsOfTab)))
                return;

            var masterTechnologyVmodel = itemVmodelsOfTab.FirstOrDefault(vm => vm.TechnologyItem.Technology == masterTechnologyDef);
            if (masterTechnologyVmodel == null)
            {
                UI.Logger.IfError()?.Message($"Not found {nameof(masterTechnologyVmodel)} by {masterTechnologyDef.____GetDebugShortName()}  -- {this}").Write();
                return;
            }

            MasterTechnologyItem = masterTechnologyVmodel.TechnologyItem;
            var isActivatedStream = masterTechnologyVmodel.IsActivatedRp
                .Zip(D, masterTechnologyVmodel.IsBlockedByMutationRp)
                .Func(D, (isActivated, isBlocked) => isActivated && !isBlocked);
            isActivatedStream.Bind(D, IsAvailableRp);
        }


        //=== Public ==========================================================

        public override string ToString()
        {
            return $"{nameof(TechnoLinkVmodel)}: Slave={SlaveTechnologyItem}, Master={MasterTechnologyItem}";
        }

        public override void Dispose()
        {
            base.Dispose();
            IsAvailableRp.Dispose();
        }
    }
}