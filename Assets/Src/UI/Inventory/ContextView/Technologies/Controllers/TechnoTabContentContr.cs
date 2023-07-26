using System.Linq;
using Assets.Src.Aspects.Impl.Factions.Template;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TechnoTabContentContr : BindingController<TechnoTabVmodel>, IPositionSetter
    {
        [SerializeField, UsedImplicitly]
        private TechnoItemContr _technoItemContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _technoItemContrsRoot;

        [SerializeField, UsedImplicitly]
        private TechnoLinkContr _technoLinkContrPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _technoLinkContrsRoot;

        private TechnoItemContr[] _itemContrs;
        private TechnoLinkContr[] _linkContrs;

        private ReactiveProperty<float> _contentHeightRp = new ReactiveProperty<float>();
        private ReactiveProperty<float> _contentWidthRp = new ReactiveProperty<float>();


        //=== Props ===========================================================

        [Binding]
        public bool IsSelected { get; private set; }

        [Binding]
        public float Width { get; private set; }

        [Binding]
        public float Height { get; private set; }

        [Binding]
        public bool IsPreviewMode { get; private set; }

        public float GridPitch { get; private set; }

        public Vector2 LogicalOffset { get; private set; }

        public TechnoAtlasContr ParentTechnoAtlasContr { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_technoItemContrPrefab.AssertIfNull(nameof(_technoItemContrPrefab)) ||
                _technoItemContrsRoot.AssertIfNull(nameof(_technoItemContrsRoot)) ||
                _technoLinkContrPrefab.AssertIfNull(nameof(_technoLinkContrPrefab)) ||
                _technoLinkContrsRoot.AssertIfNull(nameof(_technoLinkContrsRoot)))
                return;

            Vmodel.Action(D, vm => name = vm?.TechnologyTabDef?.____GetDebugShortName() ?? nameof(TechnoTabContentContr));

            var isSelectedStream = Vmodel.SubStream(D, vm => vm.IsSelectedRp, false);
            Bind(isSelectedStream, () => IsSelected);

            Bind(_contentWidthRp, () => Width);
            Bind(_contentHeightRp, () => Height);
        }


        //=== Public ==========================================================

        [UsedImplicitly]
        public void OnContentAreaClick()
        {
            Vmodel.Value?.SetSelectedItemVmodel(null);
        }

        public void BeforeSetVmodel(bool isPreviewMode, TechnoAtlasContr technoAtlasContr)
        {
            ParentTechnoAtlasContr = technoAtlasContr;
            if (ParentTechnoAtlasContr.AssertIfNull(nameof(ParentTechnoAtlasContr)))
                return;

            Vmodel.Action(D, CreateItems);

            if (IsPreviewMode != isPreviewMode)
            {
                IsPreviewMode = isPreviewMode;
                NotifyPropertyChanged(nameof(IsPreviewMode));
            }
        }

        /// <summary>
        /// Точка отсчета - в центре поля под айтемы
        /// </summary>
        public void SetPosition(Vector2 position, RectTransform positionedRectTransform)
        {
            if (!Vmodel.HasValue || Vmodel.Value?.TechnologyTabDef == null)
            {
                UI.Logger.IfError()?.Message($"Unable to set position {position} cause {nameof(Vmodel)}/{nameof(Vmodel.Value.TechnologyTabDef)} isn't set").Write();
                return;
            }

            positionedRectTransform.localPosition = new Vector2((position.x - LogicalOffset.x) * GridPitch, (position.y - LogicalOffset.y) * GridPitch);
        }


        //=== Private =========================================================

        private void CalculateContentSizeParams(TechnoTabVmodel technoTabVmodel)
        {
            if (technoTabVmodel == null ||
                technoTabVmodel.ItemVmodels == null ||
                technoTabVmodel.ItemVmodels.Length == 0)
            {
                LogicalOffset = Vector2.zero;
                _contentWidthRp.Value = 10;
                _contentHeightRp.Value = 10;
                return;
            }

            GridPitch = technoTabVmodel.TechnologyTabDef.GridPitch;
            var marginX = technoTabVmodel.TechnologyTabDef.MarginX;
            var marginY = technoTabVmodel.TechnologyTabDef.MarginY;

            var itemLogicalMinPosX = technoTabVmodel.ItemVmodels.Min(itemVm => itemVm.TechnologyItem.X);
            var itemLogicalMaxPosX = technoTabVmodel.ItemVmodels.Max(itemVm => itemVm.TechnologyItem.X);
            var itemLogicalMinPosY = technoTabVmodel.ItemVmodels.Min(itemVm => itemVm.TechnologyItem.Y);
            var itemLogicalMaxPosY = technoTabVmodel.ItemVmodels.Max(itemVm => itemVm.TechnologyItem.Y);

            LogicalOffset = new Vector2(
                itemLogicalMinPosX + (itemLogicalMaxPosX - itemLogicalMinPosX) / 2,
                itemLogicalMinPosY + (itemLogicalMaxPosY - itemLogicalMinPosY) / 2);

            _contentWidthRp.Value = (itemLogicalMaxPosX - itemLogicalMinPosX) * GridPitch + marginX * 2;
            _contentHeightRp.Value = (itemLogicalMaxPosY - itemLogicalMinPosY) * GridPitch + marginY * 2;
        }

        private void CreateItems(TechnoTabVmodel technoTabVmodel)
        {
            CalculateContentSizeParams(technoTabVmodel);
            if (_itemContrs != null && _itemContrs.Length > 0)
            {
                foreach (var itemContr in _itemContrs)
                    Destroy(itemContr.gameObject);

                _itemContrs = null;
            }

            if (_linkContrs != null && _linkContrs.Length > 0)
            {
                foreach (var itemContr in _linkContrs)
                    Destroy(itemContr.gameObject);

                _linkContrs = null;
            }

            if (technoTabVmodel.AssertIfNull(nameof(technoTabVmodel)) ||
                technoTabVmodel.ItemVmodels.AssertIfNull(nameof(technoTabVmodel.ItemVmodels)))
                return;

            var itemVmodels = technoTabVmodel.ItemVmodels;
            var itemVmodelsLength = itemVmodels.Length;
            _itemContrs = new TechnoItemContr[itemVmodelsLength];

            for (int i = 0; i < itemVmodelsLength; i++)
            {
                var itemContr = Instantiate(_technoItemContrPrefab, _technoItemContrsRoot);
                itemContr.BeforeSetVmodel(this);
                itemContr.SetVmodel(itemVmodels[i]);
                _itemContrs[i] = itemContr;
            }

            if (technoTabVmodel.LinkVmodels == null)
            {
                UI.Logger.IfError()?.Message($"{nameof(technoTabVmodel.LinkVmodels)} is null -- {technoTabVmodel}").Write();
                return;
            }

            var linkVmodels = technoTabVmodel.LinkVmodels;
            var linkVmodelsLength = linkVmodels.Length;
            _linkContrs = new TechnoLinkContr [linkVmodelsLength];
            for (int i = 0; i < linkVmodelsLength; i++)
            {
                var linkContr = Instantiate(_technoLinkContrPrefab, _technoLinkContrsRoot);
                linkContr.InitBeforeSetVmodel(this);
                linkContr.SetVmodel(linkVmodels[i]);
                _linkContrs[i] = linkContr;
            }
        }
    }
}