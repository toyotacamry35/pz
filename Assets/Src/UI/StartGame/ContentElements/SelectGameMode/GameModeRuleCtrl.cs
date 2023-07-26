using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using ReactivePropsNs.ThreadSafe;
using SharedCode.Aspects.Sessions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class GameModeRuleCtrl : BindingControllerWithUsingProp<RealmRuleVM>
    {
        [SerializeField]
        private SemanticColorsHolder SemanticColorsHolder;

        [SerializeField]
        private RealmRulesLocalizationDefRef RealmRulesLocalizationDef;

        [Binding, UsedImplicitly]
        public LocalizedString Label { get; set; }

        [Binding, UsedImplicitly]
        public LocalizedString Value { get; set; }

        [Binding, UsedImplicitly]
        public int ValuePluralNum { get; set; }

        [Binding, UsedImplicitly]
        public Color ValueColor { get; set; }

        private RealmRulesLocalizationDef RulesLocalizationDef => RealmRulesLocalizationDef.Target;

        protected override void Awake()
        {
            base.Awake();

            var defStream = Vmodel.Func(D, vm => vm?.Def);
            var availableStream = Vmodel.SubStream(D, vm => vm.Available);
            var translationStream = defStream.Func(D, Translate);
            Bind(translationStream.Func(D, (label, _, __) => label), () => Label);
            Bind(translationStream.Func(D, (_, value, __) => value), () => Value);
            Bind(translationStream.Func(D, (_, __, valuePluralNum) => valuePluralNum), () => ValuePluralNum);

            var colorStream = defStream
                .ZipSecondOrDefault(D, availableStream)
                .Func(
                    D,
                    (def, available) => SemanticColorsHolder.GetColor(
                        available && def != null ? def.SemanticContext : SemanticContext.Primary
                    )
                );
            Bind(colorStream, () => ValueColor);
        }

        private (LocalizedString label, LocalizedString value, int valuePluralNum) Translate(RealmRuleDef realmRuleDef)
        {
            switch (realmRuleDef)
            {
                case RealmGameTime realmGameTime:
                {
                    return (RulesLocalizationDef.GameTimeLabel, RulesLocalizationDef.DaysCount, realmGameTime.DaysCount);
                }
                case RealmPlayersInteraction playersInteraction:
                {
                    RulesLocalizationDef.PlayersInteraction.TryGetValue(playersInteraction.PlayersInteraction, out var localizedString);
                    return (RulesLocalizationDef.PlayersInteractionLabel, localizedString, 0);
                }
                case RealmDeathLimit realmDeathLimit:
                {
                    RulesLocalizationDef.DeathLimit.TryGetValue(realmDeathLimit.DeathLimit, out var localizedString);
                    return (RulesLocalizationDef.DeathLimitLabel, localizedString, 0);
                }
                case RealmExp realmExp:
                {
                    RulesLocalizationDef.ExpReward.TryGetValue(realmExp.ExpReward, out var localizedString);
                    return (RulesLocalizationDef.ExpLabel, localizedString, 0);
                }
            }

            return (default, default, 0);
        }
    }
}