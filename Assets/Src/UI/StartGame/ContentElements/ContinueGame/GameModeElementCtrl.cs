using JetBrains.Annotations;
using L10n;
using UnityEngine;
using UnityWeld.Binding;
using Assets.ReactiveProps;
using ReactivePropsNs;

namespace Uins
{
    [Binding]
    public class GameModeElementCtrl : BindingControllerWithUsingProp<RealmRulesVM>
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
        public bool Available { get; set; }

        protected override void Awake()
        {
            base.Awake();

            foreach (Transform child in RulesContainer.transform)
                Destroy(child.gameObject);

            var realmRulesDefStream = Vmodel.Func(D, vm => vm?.Def);
            var availableStream = Vmodel.SubStream(D, vm => vm.Available);

            var realmRuleDefs = realmRulesDefStream.NonMutableEnumerableAsSubListStream(D, def => def?.Rules);
            var realmRuleVMs = realmRuleDefs.Transform(D, def => def != null ? new RealmRuleVM(def, availableStream) : null);
            var realmRulesPool = new BindingControllersPool<RealmRuleVM>(RulesContainer, RulePrefab);
            realmRulesPool.Connect(realmRuleVMs);

            Bind(realmRulesDefStream.Func(D, def => def?.Title ?? default), () => Title);
            Bind(realmRulesDefStream.Func(D, def => def?.Description ?? default), () => Description);
            Bind(availableStream, () => Available);
        }
    }
}