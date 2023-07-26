using Assets.Src.Aspects.Impl.Factions.Template;
using ReactivePropsNs;
using Assets.Src.ResourceSystem;
using JetBrains.Annotations;
using ReactivePropsNs.Touchables;
using Uins.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class TechnoAtlasContr : BindingController<TechnoAtlasVmodel>
    {
        [SerializeField, UsedImplicitly]
        private ScrollRect _contentScrollRect;

        [SerializeField, UsedImplicitly]
        private bool _isPreviewMode;

        [SerializeField, UsedImplicitly]
        private WorldConstantsResourceRef _worldConstantsResourceRef;

        [SerializeField, UsedImplicitly]
        private TechnologyAtlasDefRef _technologyAtlasDefRef;

        [SerializeField, UsedImplicitly]
        private TechnoTabContr _technoTabContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _technoTabContrsRoot;

        [SerializeField, UsedImplicitly]
        private TechnoTabContentContr _technoTabContentContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _technoTabContentContrsRoot;

        [SerializeField, UsedImplicitly]
        private MutationStageDefRef _defaultMutationStageDefRef;

        private TechnoTabContr[] _tabContrs;
        private TechnoTabContentContr[] _tabContentContrs;


        //=== Props ===========================================================

        public MutationStageDef DefaultMutationStage => _defaultMutationStageDefRef.Target;

        public ReactiveProperty<bool> IsInventoryWindowAndAtlasTabOpenRp { get; } = new ReactiveProperty<bool>() {Value = true};


        //=== Unity ===========================================================

        private void Start()
        {
            if (_worldConstantsResourceRef.Target.AssertIfNull(nameof(_worldConstantsResourceRef)) ||
                _technoTabContrPrefab.AssertIfNull(nameof(_technoTabContrPrefab)) ||
                _technoTabContentContrPrefab.AssertIfNull(nameof(_technoTabContentContrPrefab)) ||
                _technoTabContrsRoot.AssertIfNull(nameof(_technoTabContrsRoot)) ||
                _technoTabContentContrsRoot.AssertIfNull(nameof(_technoTabContentContrsRoot)) ||
                _defaultMutationStageDefRef.Target.AssertIfNull(nameof(_defaultMutationStageDefRef)) ||
                (!_isPreviewMode && _contentScrollRect.AssertIfNull(nameof(_contentScrollRect))))
                return;

            //--- Переключение в дочерних TechnoTabVmodels значение IsSelectedRp по изменениям SelectedTabIndexRp
            var selectedTabIndexStream = Vmodel
                .SubStream(D, model => model.SelectedTabIndexRp)
                .Zip(D, Vmodel)
                .Where(D, (selectedIndex, vm) => vm != null);

            selectedTabIndexStream.Action(D, (selectedIndex, vm) =>
            {
                var tabVmodels = Vmodel.Value.TechnoTabVmodels;
                if (tabVmodels != null)
                    for (int i = 0; i < tabVmodels.Length; i++)
                        tabVmodels[i].IsSelectedRp.Value = i == selectedIndex;
            });

            Vmodel.Action(D, CreateAtlas);
        }


        //=== Public ==========================================================

        public void SetCharacterStreams(IPawnSource pawnSource, ICharacterPoints characterPoints, IGuiWindow inventoryGuiWindow,
            IStream<ContextViewWithParamsVmodel> cvwpVmodelStream)
        {
            var mutationStageStream = pawnSource != null
                ? pawnSource.TouchableEntityProxy
                    .Child(D, character => character.MutationMechanics)
                    .ToStream(D, faction => faction.Stage)
                : new ReactiveProperty<MutationStageDef>() {Value = DefaultMutationStage};
            var characterStreamsData = new CharacterStreamsData()
            {
                CharacterPoints = characterPoints,
                PawnSource = pawnSource,
                MutationStageStream = mutationStageStream,
                AccountLevelStream = pawnSource?.AccountLevelStream
            };

            //В режиме просмотра берем jdb атласа из _technologyAtlasDefRef, а если нет - то из WorldConstantsResource,
            //в режиме игры - только из WorldConstantsResource
            var technoAtlasDef = _isPreviewMode
                ? _technologyAtlasDefRef.Target ?? _worldConstantsResourceRef.Target?.TechnologyAtlas.Target
                : _worldConstantsResourceRef.Target?.TechnologyAtlas.Target;
            if (!technoAtlasDef.AssertIfNull(nameof(technoAtlasDef)))
                Vmodel.Value = new TechnoAtlasVmodel(technoAtlasDef, characterStreamsData);

            if (inventoryGuiWindow != null && cvwpVmodelStream != null)
            {
                var technosInventoryTabIsOpenStream = cvwpVmodelStream.SubStream(D, vm => vm.GetTabVmodel(InventoryTabType.Technologies).IsOpenTabRp);
                inventoryGuiWindow.State
                    .Zip(D, technosInventoryTabIsOpenStream)
                    .Func(D, (state, isTabOpen) => state == GuiWindowState.Opened && isTabOpen)
                    .Bind(D, IsInventoryWindowAndAtlasTabOpenRp);
            }
        }


        //=== Private =========================================================

        private void CreateAtlas(TechnoAtlasVmodel vmodel)
        {
            if (_tabContrs != null && _tabContrs.Length > 0)
            {
                foreach (var tabContr in _tabContrs)
                    Destroy(tabContr.gameObject);

                _tabContrs = null;
            }

            if (_tabContentContrs != null && _tabContentContrs.Length > 0)
            {
                foreach (var tabContentContr in _tabContentContrs)
                    Destroy(tabContentContr.gameObject);

                _tabContentContrs = null;
            }

            if (vmodel.AssertIfNull(nameof(vmodel)) ||
                vmodel.TechnoTabVmodels.AssertIfNull(nameof(vmodel.TechnoTabVmodels)))
                return;

            var tabVmodels = vmodel.TechnoTabVmodels;
            var tabVmodelsLength = tabVmodels.Length;
            _tabContentContrs = new TechnoTabContentContr[tabVmodelsLength];
            if (!_isPreviewMode)
                _tabContrs = new TechnoTabContr[tabVmodelsLength];

            for (int i = 0; i < tabVmodelsLength; i++)
            {
                var tabContentContr = Instantiate(_technoTabContentContrPrefab, _technoTabContentContrsRoot);
                tabContentContr.BeforeSetVmodel(_isPreviewMode, this);
                tabContentContr.SetVmodel(tabVmodels[i]);
                _tabContentContrs[i] = tabContentContr;

                if (!_isPreviewMode)
                {
                    var tabContr = Instantiate(_technoTabContrPrefab, _technoTabContrsRoot);
                    tabContr.SetVmodel(tabVmodels[i]);
                    _tabContrs[i] = tabContr;
                }
            }

            if (!_isPreviewMode)
            {
                //-- Переключение _tabContentContrs
                var currentTabIndexStream = Vmodel.SubStream(D, vm => vm.SelectedTabIndexRp);
                currentTabIndexStream.Action(D, idx =>
                {
                    for (int i = 0; i < _tabContentContrs.Length; i++)
                    {
                        _tabContentContrs[i].gameObject.SetActive(i == idx);
                    }

                    if (idx >= 0)
                        _contentScrollRect.content = _tabContentContrs[idx].transform.GetRectTransform();
                });
            }
        }
    }
}