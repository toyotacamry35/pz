using ReactivePropsNs;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;

namespace Uins
{
    public class RealmRulesQueryVM : BindingVmodel
    {
        private readonly SelectGameModeContentVM _parentVM;
        private readonly ReactiveProperty<RealmRulesQueryState> _state = new ReactiveProperty<RealmRulesQueryState>();
        private readonly ReactiveProperty<bool> _hovered = new ReactiveProperty<bool>();

        public RealmRulesQueryDef Def { get; }
        public IReactiveProperty<RealmRulesQueryState> State => _state;
        public IStream<bool> Selected { get; }
        public IReactiveProperty<bool> Hovered => _hovered;

        public RealmRulesQueryVM(
            RealmRulesQueryDef def,
            IDictionaryStream<RealmRulesQueryDef, RealmRulesQueryState> queryStates,
            SelectGameModeContentVM parentVM)
        {
            _parentVM = parentVM;
            Def = def;
            Selected = _parentVM.SelectedRealmRulesQuery.Func(D, vm => vm == this);

            queryStates.AddStream
                .WhereProp(D, e => e.Key == def)
                .Action(D, e => _state.Value = e.Value);
            queryStates.RemoveStream
                .WhereProp(D, e => e.Key == def)
                .Action(D, e => _state.Value = default);
            queryStates.ChangeStream
                .WhereProp(D, e => e.Key == def)
                .Action(D, e => _state.Value = e.NewValue);
        }

        public void SetSelected()
        {
            if (_state.Value.Available)
                _parentVM.SetSelected(this);
        }

        public void SetHovered(bool value)
        {
            if (_state.Value.Available)
                _hovered.Value = value;
        }
    }
}