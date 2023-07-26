using System.Linq;
using Assets.ReactiveProps;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using L10n;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.Aspects.Sessions;
using SharedCode.Entities;

namespace Uins
{
    public class SelectGameModeContentVM : BindingVmodel
    {
        private readonly ListStream<RealmRulesQueryDef> _queryDefs = new ListStream<RealmRulesQueryDef>();

        private readonly DictionaryStream<RealmRulesQueryDef, RealmRulesQueryState> _queryStates =
            new DictionaryStream<RealmRulesQueryDef, RealmRulesQueryState>();

        private readonly ListStream<RealmRulesQueryVM> _realmRulesQueryVMs;

        private readonly ReactiveProperty<RealmRulesQueryVM> _selectedRealmRulesQuery = new ReactiveProperty<RealmRulesQueryVM>
            {Value = null};

        public IListStream<RealmRulesQueryDef> QueryDefs => _queryDefs;
        public IStream<RealmRulesQueryVM> SelectedRealmRulesQuery => _selectedRealmRulesQuery;
        public IListStream<RealmRulesQueryVM> RealmRulesQueryVMs => _realmRulesQueryVMs;

        public StartGameWindowVM StartGameWindowVM { get; }

        public SelectGameModeContentVM(StartGameWindowVM vm)
        {
            StartGameWindowVM = vm;
            
            ConnectAvailableRealmQueries(vm.TouchableAccount);

            _realmRulesQueryVMs = _queryDefs
                .Transform(
                    D,
                    (def, localD) => new RealmRulesQueryVM(def, _queryStates, this)
                );

            InitSelectRealmRulesQueryToggle();
        }

        public void SetSelected(RealmRulesQueryVM realmRulesQueryVM)
        {
            if (realmRulesQueryVM == null || _realmRulesQueryVMs.Contains(realmRulesQueryVM))
                _selectedRealmRulesQuery.Value = realmRulesQueryVM;
        }

        private void ConnectAvailableRealmQueries(ITouchable<IAccountEntityClientFull> touchableAccount)
        {
            var queries = touchableAccount.ToDictionaryStream(D, entity => entity.AvailableRealmQueries);
            queries.AddStream.Action(
                D,
                addEvent =>
                {
                    var rulesQueryDef = addEvent.Key;
                    var realmRulesQueryState = addEvent.Value;

                    _queryStates.Add(rulesQueryDef, realmRulesQueryState);

                    var realmRulesQueryDef = rulesQueryDef;

                    var priority = realmRulesQueryDef.Priority;
                    var foundPlace = false;

                    var count = _queryDefs.Count;
                    for (var index = 0; index < count; index++)
                    {
                        var elementPriority = _queryDefs[index]?.Priority ?? -1;
                        if (priority > elementPriority)
                        {
                            _queryDefs.Insert(index, realmRulesQueryDef);
                            foundPlace = true;
                            break;
                        }
                    }

                    if (!foundPlace)
                        _queryDefs.Add(realmRulesQueryDef);
                });
            queries.RemoveStream.Action(
                D,
                removeEvent =>
                {
                    _queryStates.Remove(removeEvent.Key);

                    for (var index = _queryDefs.Count - 1; index >= 0; index--)
                        if (_queryDefs[index] == removeEvent.Key)
                        {
                            _queryDefs.RemoveAt(index);
                            break;
                        }
                });
            queries.ChangeStream.Action(
                D,
                changeEvent => { _queryStates[changeEvent.Key] = changeEvent.NewValue; }
            );
        }

        private void InitSelectRealmRulesQueryToggle()
        {
            _realmRulesQueryVMs.Transform(
                D,
                (vm, vmD) =>
                {
                    vm.State.Action(
                        vmD,
                        queryState =>
                        {
                            if (queryState.Available)
                            {
                                if (_selectedRealmRulesQuery.Value == null)
                                    _selectedRealmRulesQuery.Value = vm;
                            }
                            else if (_selectedRealmRulesQuery.Value == vm)
                                _selectedRealmRulesQuery.Value = FindNewSelected();
                        });
                    return new DisposeAgent(
                        () =>
                        {
                            if (_selectedRealmRulesQuery.Value == vm)
                                _selectedRealmRulesQuery.Value = FindNewSelected();
                        }
                    );
                }
            );

            RealmRulesQueryVM FindNewSelected()
            {
                return _realmRulesQueryVMs.FirstOrDefault(query => query.State.HasValue && query.State.Value.Available);
            }
        }
    }
}