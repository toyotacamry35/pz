using Assets.ReactiveProps;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class GameModeQueryElementCtrl : BindingControllerWithUsingProp<RealmRulesQueryVM>
    {
        // private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]
        private GameModeRuleCtrl RulePrefab;

        [SerializeField, UsedImplicitly]
        private Transform RulesContainer;

        [Binding, UsedImplicitly]
        public LocalizedString Title { get; set; }

        [Binding, UsedImplicitly]
        public LocalizedString Description { get; set; }

        [Binding, UsedImplicitly]
        public LocalizedString LockedDescription { get; set; }

        [Binding, UsedImplicitly]
        public bool Selected { get; set; }

        [Binding, UsedImplicitly]
        public bool Hovered { get; set; }

        [Binding, UsedImplicitly]
        public bool Available { get; set; }

        protected override void Awake()
        {
            base.Awake();

            foreach (Transform child in RulesContainer.transform)
                Destroy(child.gameObject);

            var defStream = Vmodel.Func(D, vm => vm?.Def);
            var realmRulesDefStream = defStream.Func(D, def => def?.RealmRules.Target);
            var stateStream = Vmodel.SubStream(D, vm => vm.State);
            var availableStream = stateStream.Func(D, state => state.Available);

            var realmRuleDefs = realmRulesDefStream.NonMutableEnumerableAsSubListStream(D, def => def?.Rules);
            var realmRuleVMs = realmRuleDefs.Transform(D, def => def != null ? new RealmRuleVM(def, availableStream) : null);
            var realmQueriesPool = new BindingControllersPool<RealmRuleVM>(RulesContainer, RulePrefab);
            realmQueriesPool.Connect(realmRuleVMs);
            D.Add(new DisposeAgent(realmQueriesPool.Disconnect));

            Bind(realmRulesDefStream.Func(D, def => def?.Title ?? default), () => Title);
            Bind(realmRulesDefStream.Func(D, def => def?.Description ?? default), () => Description);

            Bind(defStream.Func(D, def => def?.LockedDescription ?? default), () => LockedDescription);
            Bind(Vmodel.SubStream(D, vm => vm.Selected), () => Selected);
            Bind(Vmodel.SubStream(D, vm => vm.Hovered), () => Hovered);
            Bind(availableStream, () => Available);
        }

        [UsedImplicitly]
        public void OnClick()
        {
            Vmodel.Value?.SetSelected();
        }

        [UsedImplicitly]
        public void OnPointerEnter()
        {
            Vmodel.Value?.SetHovered(true);
        }

        [UsedImplicitly]
        public void OnPointerExit()
        {
            Vmodel.Value?.SetHovered(false);
        }
    }
}