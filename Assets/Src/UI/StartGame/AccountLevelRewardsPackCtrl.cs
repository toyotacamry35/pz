using System.Runtime.InteropServices.ComTypes;
using JetBrains.Annotations;
using L10n;
using UnityWeld.Binding;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.EventSystems;

using AchievedConsumedState = Uins.AccountLevelRewardsPackVM.AchievedConsumedState;
// ReSharper disable UnusedAutoPropertyAccessor.Local //private sets are used by Wield via reflection

namespace Uins
{
    [Binding]
    public class AccountLevelRewardsPackCtrl : BindingController<AccountLevelRewardsPackVM>, IPointerEnterHandler, IPointerExitHandler
    {
        //#Replaced:
        // [Binding]  public bool IsAchieved       { get; private set; }
        [Binding]  public bool IsConsumed        { get; private set; }
        [Binding]  public int StateInt           { get; private set; }
        [Binding]  public AchievedConsumedState State { get; private set; }
        [Binding]  public int Level              { get; private set; }
        [Binding]  public Sprite AchievementIcon { get; private set; }
        [Binding]  public LocalizedString AchievementName { get; private set; }
        [Binding]  public bool IsInteractable    { get; private set; }

        private IStream _selectedStream;

        [UsedImplicitly] //U
        private void Awake()
        {
            //var isAchievedStream = Vmodel.SubStream(D, vm => vm.IsAchieved).Log(D, $"{name} isAchievedStream");
            var isConsumedStream     = Vmodel.SubStream(D, vm => vm.IsConsumed);
            var isInteractableStream = Vmodel.SubStream(D, vm => vm.IsInteractable);
            var stateStream          = Vmodel.SubStream(D, vm => vm.State);
            var stateIntStream       = Vmodel.SubStream(D, vm => vm.StateInt);
            var lvlStream            = Vmodel.SubStream(D, vm => vm.Level);
            var defStream            = Vmodel.SubStream(D, vm => vm.Def);

            var achievementIconStream = defStream.Func(D, def => def?.Achievement.Target?.Icon?.Target);
            var achievementNameStream = defStream.Func(D, def => def?.Achievement.Target?.Name ?? LsExtensions.Empty);

            Bind(isConsumedStream, () => IsConsumed);
            Bind(isInteractableStream, () => IsInteractable);
            Bind(stateStream,      () => State);
            Bind(stateIntStream,   () => StateInt);
            Bind(lvlStream,        () => Level);
            Bind(achievementIconStream, () => AchievementIcon);
            Bind(achievementNameStream, () => AchievementName);

            Vmodel.Action(D, (v) =>
            {
                if (Vmodel.HasValue && Vmodel.Value != null)
                    // At this point Ctrl prefab is already instantiated & all possible inits caused by set Vmodel are done (I suppose). So it's time to recalc scroll (f.e.)
                    Vmodel.Value.WhenElemIsAddedToSceneCallback();
            });
        }



        public void OnClick()
        {
            //if (!IsAchieved || IsConsumed)
            if (State != AchievedConsumedState.Achieved)
                return;

            Vmodel.Value.ConsumeCallback(Vmodel.Value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Vmodel.Value.SelectedCallback?.Invoke(Vmodel.Value, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Vmodel.Value.SelectedCallback?.Invoke(Vmodel.Value, false);
        }
    }
}
