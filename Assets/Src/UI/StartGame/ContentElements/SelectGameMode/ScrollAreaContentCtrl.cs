using System;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;
using Transform = UnityEngine.Transform;

namespace Uins
{
    [Binding]
    public class ScrollAreaContentCtrl : BindingController<ScrollAreaContentVM>
    {
        private const int StaticElementsInScrollCount = 1;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]
        private PaginationContentCtrl PaginationContentCtrl;

        [SerializeField, UsedImplicitly]
        private GameModeQueryElementCtrl GameModeQueryPrefab;

        [SerializeField, UsedImplicitly]
        private Transform GameModeQueryContainer;

        [SerializeField, UsedImplicitly]
        private PlayWithFriendsCtrl PlayWithFriendsCtrl;

        [SerializeField, UsedImplicitly]
        private int VisibleQueriesCount = 3;

        [Binding, UsedImplicitly]
        public bool IsNextAvailable { get; set; }

        [Binding, UsedImplicitly]
        public bool IsPrevAvailable { get; set; }

        [Binding, UsedImplicitly]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                _queryPaginationVM?.Value?.SetSelectedIndex(value);
            }
        }

        private int _selectedIndex;
        private ReactiveProperty<QueryPaginationVM> _queryPaginationVM;
        private IPaginationElementVM _next;
        private IPaginationElementVM _prev;

        private void Awake()
        {
            foreach (Transform child in GameModeQueryContainer.transform)
                Destroy(child.gameObject);

            var playWithFriendsVM = Vmodel.Transform(D, vm => vm != null ? new PlayWithFriendsVM(vm) : null);
            PlayWithFriendsCtrl.BindVM(D, playWithFriendsVM);

            _queryPaginationVM = Vmodel.Transform(
                D,
                (vm, localD) =>
                {
                    var pagesCountStream = vm?.SelectGameModeContentVM.QueryDefs.CountStream
                        .Func(localD, count => Math.Max(count - VisibleQueriesCount + 1 + StaticElementsInScrollCount, 0));

                    return pagesCountStream != null ? new QueryPaginationVM(pagesCountStream) : null;
                }
            );
            PaginationContentCtrl.BindVM(D, _queryPaginationVM);

            Bind(_queryPaginationVM.SubStream(D, vm => vm.NextAvailable), () => IsNextAvailable);
            Bind(_queryPaginationVM.SubStream(D, vm => vm.PrevAvailable), () => IsPrevAvailable);

            _queryPaginationVM.SubStream(D, vm => vm.Next).Action(D, vm => _next = vm);
            _queryPaginationVM.SubStream(D, vm => vm.Prev).Action(D, vm => _prev = vm);

            Bind(_queryPaginationVM.SubStream(D, vm => vm.SelectedIndex), () => SelectedIndex);

            var realmQueriesPool = new BindingControllersPool<RealmRulesQueryVM>(GameModeQueryContainer, GameModeQueryPrefab);
            var realmRulesQueryVMs = Vmodel.SubListStream(D, vm => vm?.SelectGameModeContentVM.RealmRulesQueryVMs);
            realmQueriesPool.Connect(realmRulesQueryVMs);
        }

        [UsedImplicitly]
        public void OnNextButton()
        {
            if (_next != null) _queryPaginationVM.Value?.SetSelected(_next);
        }

        [UsedImplicitly]
        public void OnPrevButton()
        {
            if (_prev != null) _queryPaginationVM.Value?.SetSelected(_prev);
        }

        protected override void OnDestroy()
        {
            _queryPaginationVM = null;
            _next = null;
            _prev = null;
            base.OnDestroy();
        }
    }
}