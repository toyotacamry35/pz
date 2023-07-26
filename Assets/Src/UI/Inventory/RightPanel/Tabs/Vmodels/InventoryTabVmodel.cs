using ReactivePropsNs;

namespace Uins.Inventory
{
    public class InventoryTabVmodel : BindingVmodel
    {
        //=== Props ===========================================================

        public IContextViewTargetWithParams ContextSingleTarget { get; } //м.б. null

        public InventoryTabType TabType { get; }

        public ContextViewWithParamsVmodel ContextViewWithParamsVmodel { get; private set; }

        public ReactiveProperty<bool> IsOpenTabRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<InventoryNode.WindowMode> InventoryModeRp { get; } = new ReactiveProperty<InventoryNode.WindowMode>()
            {Value = InventoryNode.WindowMode.Normal};

        public ReactiveProperty<bool> IsOpenableRp { get; } = new ReactiveProperty<bool>();


        //=== Public ==============================================================

        public InventoryTabVmodel(InventoryTabType tabType, IContextViewTargetWithParams contextSingleTarget)
        {
            ContextSingleTarget = contextSingleTarget;
            TabType = tabType;
            InventoryModeRp.Func(D, mode => TabType != InventoryTabType.Machine || mode == InventoryNode.WindowMode.Machine).Bind(D, IsOpenableRp);
        }

        public void Init(ContextViewWithParamsVmodel cvwpVmodel)
        {
            ContextViewWithParamsVmodel = cvwpVmodel;
            cvwpVmodel.CurrentTabRp.Action(D, tabVm => IsOpenTabRp.Value = tabVm == this);
        }

        public void SetInventoryModeStream(IStream<InventoryNode.WindowMode> orgStream)
        {
            if (!orgStream.AssertIfNull(nameof(orgStream)))
                orgStream.Bind(D, InventoryModeRp);
        }

        public override string ToString()
        {
            return $"{nameof(InventoryTabVmodel)}[{TabType}] IsOpen{IsOpenTabRp.Value.AsSign()} IsOpenable{IsOpenableRp.Value.AsSign()}";
        }
    }
}