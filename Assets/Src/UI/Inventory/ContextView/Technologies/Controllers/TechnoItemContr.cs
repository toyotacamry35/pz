using System.Linq;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using Uins.Sound;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TechnoItemContr : BindingController<TechnoItemVmodel>
    {
        public enum ActivationState
        {
            UnableToActivate = -1,
            EnableToActivate = 0,
            Activated = 1
        }

        private RectTransform _rectTransform;
        private IStream<bool> _isBlockedStream;
        private IStream<ActivationState> _activationStateStream;


        //=== Props ===========================================================

        [Binding]
        public bool IsSelected { get; set; }

        [Binding]
        public bool IsImportant { get; set; }

        public ActivationState State { get; set; }

        [Binding]
        public int ActivationStateIndex { get; set; }

        [Binding]
        public Sprite Icon { get; set; }

        [Binding]
        public bool HasIcon { get; set; }

        [Binding]
        public bool IsBlocked { get; set; }

        public TechnoTabContentContr ParentTechnoTabContentContr { get; private set; }

        public IPositionSetter PositionSetter => ParentTechnoTabContentContr;

        public bool IsItemVisibleToPlayer { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _rectTransform = transform.GetRectTransform();
            Vmodel.Action(D, vm => name = vm?.TechnologyItem.Technology.Target?.____GetDebugShortName() ?? nameof(TechnoItemContr));
            var isSelectedStream = Vmodel.SubStream(D, vm => vm.IsSelectedRp);
            Bind(isSelectedStream, () => IsSelected);
            isSelectedStream.Action(
                D,
                isSelected =>
                {
                    if (isSelected)
                        SoundControl.Instance.TechHover.Post(transform.root.gameObject);
                });

            var isImportantStream = Vmodel.Func(D, vm => vm?.TechnologyItem.Technology.Target?.IsImportant ?? false);
            Bind(isImportantStream, () => IsImportant);

            var positionStream = Vmodel.Func(D, vm => new Vector2(vm?.TechnologyItem.X ?? 0, vm?.TechnologyItem.Y ?? 0));
            positionStream.Action(D, v2 => PositionSetter?.SetPosition(v2, _rectTransform));

            var isActivatedStream = Vmodel.SubStream(D, vm => vm.IsActivatedRp);
            var isEnableToActivateByRequirementsStream = Vmodel.SubStream(D, vm => vm.IsEnableToActivateByRequirementsRp);
            _isBlockedStream = Vmodel.SubStream(D, vm => vm.IsBlockedByMutationRp);
            _activationStateStream = isActivatedStream
                .Zip(D, isEnableToActivateByRequirementsStream)
                .Zip(D, _isBlockedStream)
                .Func(D, GetActivationState);
            _activationStateStream.Action(D, state => State = state);

            Bind(_activationStateStream.Func(D, state => (int) state), () => ActivationStateIndex);
            Bind(_isBlockedStream, () => IsBlocked);
        }


        //=== Public ==========================================================

        public static ActivationState GetActivationState(bool isActivated, bool isEnableToActivate, bool isBlocked)
        {
            if (isBlocked)
                return ActivationState.UnableToActivate;

            return isActivated
                ? ActivationState.Activated
                : (isEnableToActivate ? ActivationState.EnableToActivate : ActivationState.UnableToActivate);
        }

        [UsedImplicitly]
        public void OnClick()
        {
            Vmodel.Value?.TechnoTabVmodel.SetSelectedItemVmodel(Vmodel.Value);
        }

        public void BeforeSetVmodel(TechnoTabContentContr technoTabContentContr)
        {
            ParentTechnoTabContentContr = technoTabContentContr;
            if (ParentTechnoTabContentContr.AssertIfNull(nameof(ParentTechnoTabContentContr)))
                return;

            //-- Извлечение потока текущей мутации
            var currentMutationStageStream = Vmodel
                .SubStream(D, vm => vm.CharData.MutationStageStream, ParentTechnoTabContentContr.ParentTechnoAtlasContr.DefaultMutationStage);

            //Из мутации - доступные знания
            var availKnowledgesStream = Vmodel
                .Zip(D, currentMutationStageStream)
                .Func(
                    D,
                    (vm, mutStage) =>
                        KnowledgeLogic.GetKnownKnowledges(Enumerable.Repeat(vm.TechnologyItem.Technology.Target, 1), null, mutStage, false));

            if (availKnowledgesStream.AssertIfNull(nameof(availKnowledgesStream)))
                return;

            //Из первого доступного рецепта с картинкой - картинка
            var recipeBlueprintIconStream = availKnowledgesStream
                .Func(
                    D,
                    knowledges =>
                    {
                        if (knowledges == null)
                            return null;

                        var bluprintIcon = knowledges?
                            .Where(knowledgeDef => knowledgeDef.BlueprintIcon?.Target != null)
                            .Select(knowledgeDef => knowledgeDef.BlueprintIcon.Target)
                            .FirstOrDefault();

                        if (bluprintIcon == null)
                        {
                            bluprintIcon = knowledges?
                                .Where(knowledgeDef => knowledgeDef.Recipes != null)
                                .SelectMany(knowledgeDef => knowledgeDef.Recipes)
                                .Where(resRef => resRef.Target != null)
                                .Select(resRef => resRef.Target) //все рецепты
                                .FirstOrDefault(baseRecipeDef => baseRecipeDef?.BlueprintIcon?.Target != null)
                                ?.BlueprintIcon?.Target;
                        }

                        return bluprintIcon;
                    }
                );

            var hasBlueprintIconStream = recipeBlueprintIconStream.Func(D, icon => icon != null);

            Bind(recipeBlueprintIconStream, () => Icon);
            Bind(hasBlueprintIconStream, () => HasIcon);

            var isItemVisibleToPlayerStream = _isBlockedStream
                .Zip(D, ParentTechnoTabContentContr.ParentTechnoAtlasContr.IsInventoryWindowAndAtlasTabOpenRp)
                .Zip(D, ParentTechnoTabContentContr.Vmodel.SubStream(D, vm => vm.IsSelectedRp))
                .Func(D, (isBlocked, isWindowAndTabOpen, isItemTechnoTabSelected) => !isBlocked && isWindowAndTabOpen && isItemTechnoTabSelected);
            Bind(isItemVisibleToPlayerStream, () => IsItemVisibleToPlayer);

            _activationStateStream
                .Action(
                    D,
                    state =>
                    {
                        if (state == ActivationState.Activated && IsItemVisibleToPlayer)
                            SoundControl.Instance.TechActivate.Post(transform.root.gameObject);
                    });
        }
    }
}