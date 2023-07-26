using System;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ContainerApis;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using ReactivePropsNs;
using Uins.Extensions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PlayerMainStatsViewModel : BindingViewModel
    {
        public event Action<bool> HasPawn;

        public enum TemperatureSense
        {
            Comfortably = -1,
            TooHot = 0,
            TooCold
        }

        public const float Roughness100 = 0.01f;
        public const float Roughness1000 = 0.003f; //0.003 это около границы плавности восстановления барчика стамины


        [SerializeField, UsedImplicitly]
        private CharacterStatsMapping _statsMapping;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _frequentUpdateInterval; //0.05

        [SerializeField, UsedImplicitly]
        private UpdateInterval _moderateUpdateInterval; //0.3


        private EntityApiWrapper<HasStatsBroadcastApi> _hasStatsBroadcastApiWrapper;
        private EntityApiWrapper<HasStatsFullApi> _hasStatsFullApiWrapper;


        //=== Props ===============================================================

        //--- Health

        private PeriodicallyUpdatableStatValue _healthCurrentPu;

        [Binding]
        public float HealthCurrent { get; private set; }

        [Binding]
        public float HealthMaxCurrent { get; private set; }

        [Binding]
        public float HealthCurrentRatioClamped { get; private set; }


        //--- Stamina

        private PeriodicallyUpdatableStatValue _staminaPu;

        [Binding]
        public float StaminaCurrent { get; private set; }

        [Binding]
        public float StaminaMaxCurrent { get; private set; }

        [Binding]
        public float StaminaCurrentRatioClamped { get; private set; }

        private PeriodicallyUpdatableStatValue _staminaRegenPu;

        [Binding]
        public float StaminaRegen { get; private set; }

        private ReactiveProperty<float> _damageRp = new ReactiveProperty<float>();

        [Binding]
        public float Damage { get; private set; }

        private ReactiveProperty<float> _miningLootMultRp = new ReactiveProperty<float>();

        [Binding]
        public float MiningLootMult { get; private set; }

        private ReactiveProperty<float> _outgoingDamageModRp = new ReactiveProperty<float>();

        [Binding]
        public float OutgoingDamageMod { get; private set; }


        //--- Satiety, WaterBalance

        private PeriodicallyUpdatableStatValue _satietyPu;

        [Binding]
        public float Satiety { get; private set; }

        [Binding]
        public float SatietyMax { get; private set; }

        [Binding]
        public float SatietyRatioClamped { get; private set; }

        private PeriodicallyUpdatableStatValue _waterBalancePu;

        [Binding]
        public float WaterBalance { get; private set; }

        [Binding]
        public float WaterBalanceMax { get; private set; }

        [Binding]
        public float WaterBalanceRatioClamped { get; private set; }

        //--- Environment

        private PeriodicallyUpdatableStatValue _envTemperaturePu;

        [Binding]
        public float EnvTemperature { get; private set; }

        private ReactiveProperty<float> _comfortTempMinRp = new ReactiveProperty<float>();

        [Binding]
        public float ComfortTempMin { get; private set; }

        private ReactiveProperty<float> _comfortTempMaxRp = new ReactiveProperty<float>();

        [Binding]
        public float ComfortTempMax { get; private set; }

        [Binding]
        public int TemperatureSenseIndex { get; private set; }

        private PeriodicallyUpdatableStatValue _envToxicLevelPu;

        [Binding]
        public float EnvToxicLevel { get; private set; }

        //--- Negative factors

        private PeriodicallyUpdatableStatValue _overheatPu;
        // private ReactiveProperty<AnyStatState> _overheatStatStateRp = new ReactiveProperty<AnyStatState>();
        //
        // private ReactiveProperty<float> _overheatRp = new ReactiveProperty<float>();

        [Binding]
        public float OverheatRatio { get; private set; }

        public IStream<float> OverheatRatioStream { get; private set; }

        private PeriodicallyUpdatableStatValue _hypothermiaPu;
        // private ReactiveProperty<AnyStatState> _hypothermiaStatStateRp = new ReactiveProperty<AnyStatState>();
        //
        // private ReactiveProperty<float> _hypothermiaRp = new ReactiveProperty<float>();

        [Binding]
        public float HypothermiaRatio { get; private set; }

        public IStream<float> HypothermiaRatioStream { get; private set; }

        private PeriodicallyUpdatableStatValue _intoxicationPu;
        // private ReactiveProperty<AnyStatState> _intoxicationStatStateRp = new ReactiveProperty<AnyStatState>();
        //
        // private ReactiveProperty<float> _intoxicationRp = new ReactiveProperty<float>();

        [Binding]
        public float IntoxicationRatio { get; private set; }

        public IStream<float> IntoxicationRatioStream { get; private set; }

        [Binding]
        public bool IsOverheatVisible { get; private set; }

        [Binding]
        public bool IsHypothermiaVisible { get; private set; }

        [Binding]
        public bool IsIntoxicationVisible { get; private set; }


        //--- Inventory Weight
        private ReactiveProperty<float> _inventoryItemsWeightRp = new ReactiveProperty<float>();
        private ReactiveProperty<float> _dollItemsWeightRp = new ReactiveProperty<float>();
        private ReactiveProperty<float> _intermediateItemsWeightRp = new ReactiveProperty<float>();
        private ReactiveProperty<float> _inventoryMaxWeightRp = new ReactiveProperty<float>();

        [Binding]
        public float ItemsTotalWeight { get; private set; }

        [Binding]
        public float InventoryMaxWeight { get; private set; }

        [Binding]
        public float WeightRatio { get; private set; }

        [Binding]
        public bool WeightIndicatorVisibility { get; private set; }

        //---
        private ReactiveProperty<float> _incomingDamageModRp = new ReactiveProperty<float>();

        [Binding]
        public float IncomingDamageMod { get; private set; }

        private ReactiveProperty<float> _dmgResistanceSlashingRp = new ReactiveProperty<float>();

        [Binding]
        public float DmgResistanceSlashing { get; private set; }

        private ReactiveProperty<float> _toxicResistanceRp = new ReactiveProperty<float>();

        [Binding]
        public float ToxicResistance { get; private set; }

        private ReactiveProperty<float> _detoxicationRp = new ReactiveProperty<float>();

        [Binding]
        public float Detoxication { get; private set; }

        private ReactiveProperty<float> _caloriesConsRp = new ReactiveProperty<float>();

        [Binding]
        public float CaloriesCons { get; private set; }

        private ReactiveProperty<float> _caloriesConsModRp = new ReactiveProperty<float>();

        private ReactiveProperty<float> _waterConsRp = new ReactiveProperty<float>();

        [Binding]
        public float WaterCons { get; private set; }

        private ReactiveProperty<float> _waterConsModRp = new ReactiveProperty<float>();

        private ReactiveProperty<float> _speedFactorRp = new ReactiveProperty<float>();

        [Binding]
        public float SpeedFactor { get; private set; }

        private ReactiveProperty<float> _fallDamageModifierRp = new ReactiveProperty<float>();

        [Binding]
        public float FallDamageModifier { get; private set; }

        [Binding, UsedImplicitly]
        public ObservableList<IStatusEffectVM> TraumaIndicatorViewModels { get; } = new ObservableList<IStatusEffectVM>();


        //=== Public ==============================================================

        public void Init(IPawnSource pawnSource, ListStream<IStatusEffectVM> statusEffectVms)
        {
            _statsMapping.AssertIfNull(nameof(_statsMapping), gameObject);
            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);

            Bind(statusEffectVms, TraumaIndicatorViewModels);

            //--- Health
            _healthCurrentPu = new PeriodicallyUpdatableStatValue(D, Roughness100, true);
            Bind(_healthCurrentPu.ValueRp, () => HealthCurrent);

            var healthMaxCurrentStream = _healthCurrentPu.StatStateRp.Func(D, state => state.Max);
            Bind(healthMaxCurrentStream, () => HealthMaxCurrent);

            var healthCurrentRatioClampedStream = _healthCurrentPu.ValueRp
                .Zip(D, healthMaxCurrentStream)
                .Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(healthCurrentRatioClampedStream, () => HealthCurrentRatioClamped);

            //--- Stamina
            _staminaPu = new PeriodicallyUpdatableStatValue(D, Roughness1000);
            Bind(_staminaPu.ValueRp, () => StaminaCurrent);

            var staminaMaxCurrentStream = _staminaPu.StatStateRp.Func(D, state => state.Max);
            Bind(staminaMaxCurrentStream, () => StaminaMaxCurrent);

            var staminaCurrentRatioClampedStream = _staminaPu.ValueRp
                .Zip(D, staminaMaxCurrentStream)
                .Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(staminaCurrentRatioClampedStream, () => StaminaCurrentRatioClamped);

            _staminaRegenPu = new PeriodicallyUpdatableStatValue(D, Roughness100);
            Bind(_staminaRegenPu.ValueRp, () => StaminaRegen);

            //--- Satiety
            _satietyPu = new PeriodicallyUpdatableStatValue(D, Roughness100, true);
            Bind(_satietyPu.ValueRp, () => Satiety);

            var satietyMaxStream = _satietyPu.StatStateRp.Func(D, state => state.Max);
            Bind(satietyMaxStream, () => SatietyMax);

            var satietyRatioClampedStream = _satietyPu.ValueRp
                .Zip(D, satietyMaxStream)
                .Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(satietyRatioClampedStream, () => SatietyRatioClamped);

            //--- WaterBalance
            _waterBalancePu = new PeriodicallyUpdatableStatValue(D, Roughness100, true);
            Bind(_waterBalancePu.ValueRp, () => WaterBalance);

            var waterBalanceMaxStream = _waterBalancePu.StatStateRp.Func(D, state => state.Max);
            Bind(waterBalanceMaxStream, () => WaterBalanceMax);

            var waterBalanceRatioClampedStream = _waterBalancePu.ValueRp
                .Zip(D, waterBalanceMaxStream)
                .Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(waterBalanceRatioClampedStream, () => WaterBalanceRatioClamped);

            //---
            Bind(_damageRp, () => Damage);
            Bind(_miningLootMultRp, () => MiningLootMult);
            Bind(_outgoingDamageModRp, () => OutgoingDamageMod);

            _envToxicLevelPu = new PeriodicallyUpdatableStatValue(D, Roughness100);
            Bind(_envToxicLevelPu.ValueRp, () => EnvToxicLevel);

            _envTemperaturePu = new PeriodicallyUpdatableStatValue(D, Roughness100);
            Bind(_envTemperaturePu.ValueRp, () => EnvTemperature);

            Bind(_comfortTempMinRp, () => ComfortTempMin);
            Bind(_comfortTempMaxRp, () => ComfortTempMax);

            var temperatureSenseStateStream = _envTemperaturePu.ValueRp
                .Zip(D, _comfortTempMinRp)
                .Zip(D, _comfortTempMaxRp)
                .Func(
                    D,
                    (feelsLikeTemp, comfortTempMin, comfortTempMax) =>
                        feelsLikeTemp > comfortTempMax
                            ? TemperatureSense.TooHot
                            : feelsLikeTemp < comfortTempMin
                                ? TemperatureSense.TooCold
                                : TemperatureSense.Comfortably);

            Bind(temperatureSenseStateStream.Func(D, state => (int) state), () => TemperatureSenseIndex);

            Bind(_incomingDamageModRp, () => IncomingDamageMod);
            Bind(_dmgResistanceSlashingRp, () => DmgResistanceSlashing);
            Bind(_toxicResistanceRp, () => ToxicResistance);
            Bind(_detoxicationRp, () => Detoxication);

            var caloriesConsStream = _caloriesConsRp
                .Zip(D, _caloriesConsModRp)
                .Func(D, (cons, consMod) => cons * consMod);
            Bind(caloriesConsStream, () => CaloriesCons);

            var waterConsStream = _waterConsRp
                .Zip(D, _waterConsModRp)
                .Func(D, (cons, consMod) => cons * consMod);
            Bind(waterConsStream, () => WaterCons);

            Bind(_speedFactorRp, () => SpeedFactor);
            Bind(_fallDamageModifierRp, () => FallDamageModifier);

            //--- Weight
            Bind(_inventoryMaxWeightRp, () => InventoryMaxWeight);

            var itemsTotalWeightStream = _inventoryItemsWeightRp
                .Zip(D, _dollItemsWeightRp)
                .Zip(D, _intermediateItemsWeightRp)
                .Func(D, (f1, f2, f3) => f1 + f2 + f3);
            Bind(itemsTotalWeightStream, () => ItemsTotalWeight);

            var weightIndicatorVisibilityStream = itemsTotalWeightStream
                .Zip(D, _inventoryMaxWeightRp)
                .Func(D, (itw, max) => itw > max);
            Bind(weightIndicatorVisibilityStream, () => WeightIndicatorVisibility);

            var weightRatioStream = itemsTotalWeightStream
                .Zip(D, _inventoryMaxWeightRp)
                .Func(D, (itw, max) => itw.SafeDivide(max));
            Bind(weightRatioStream, () => WeightRatio);

            //--- Neg. factors
            _overheatPu = new PeriodicallyUpdatableStatValue(D, Roughness100, true);
            var overheatMaxStream = _overheatPu.StatStateRp.Func(D, state => state.Max);
            OverheatRatioStream = _overheatPu.ValueRp.Zip(D, overheatMaxStream).Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(OverheatRatioStream, () => OverheatRatio);
            var isOverheatVisibleStream = OverheatRatioStream.Func(D, f => f > Roughness100);
            Bind(isOverheatVisibleStream, () => IsOverheatVisible);

            _hypothermiaPu = new PeriodicallyUpdatableStatValue(D, Roughness100, true);
            var hypothermiaMaxStream = _hypothermiaPu.StatStateRp.Func(D, state => state.Max);
            HypothermiaRatioStream = _hypothermiaPu.ValueRp.Zip(D, hypothermiaMaxStream).Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(HypothermiaRatioStream, () => HypothermiaRatio);
            var isHypothermiaVisibleStream = HypothermiaRatioStream.Func(D, f => f > Roughness100);
            Bind(isHypothermiaVisibleStream, () => IsHypothermiaVisible);

            _intoxicationPu = new PeriodicallyUpdatableStatValue(D, Roughness100, true);
            var intoxicationMaxStream = _intoxicationPu.StatStateRp.Func(D, state => state.Max);
            IntoxicationRatioStream = _intoxicationPu.ValueRp.Zip(D, intoxicationMaxStream).Func(D, (curr, max) => curr.SafeRatio(max));
            Bind(IntoxicationRatioStream, () => IntoxicationRatio);
            var isIntoxicationVisibleStream = IntoxicationRatioStream.Func(D, f => f > Roughness100);
            Bind(isIntoxicationVisibleStream, () => IsIntoxicationVisible);
        }

        public void OnUpdate()
        {
            if (_frequentUpdateInterval.IsItTime())
            {
                _staminaPu?.UpdateIfNeed();
            }

            if (_moderateUpdateInterval.IsItTime())
            {
                _healthCurrentPu?.UpdateIfNeed();
                _satietyPu?.UpdateIfNeed();
                _waterBalancePu?.UpdateIfNeed();
                _overheatPu?.UpdateIfNeed();
                _hypothermiaPu?.UpdateIfNeed();
                _intoxicationPu?.UpdateIfNeed();
            }

//            if (_infrequentUpdateInterval.IsItTime()) // Павел разрешил убрать
//            {
//                ItemsTotalWeight = _calcerInventoryWeight.Calc(SelfPawn);
//            }
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _hasStatsBroadcastApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.HealthCurrentRef.Target, OnHealthCurrent);
                OnHealthCurrent(null, new AnyStatState());

                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.DamageRef.Target, OnDamage);
                OnDamage(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.MiningLootMultiplierRef.Target, OnMiningLootMult);
                OnMiningLootMult(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.OutgoingDamageModRef.Target, OnOutgoingDamageMod);
                OnOutgoingDamageMod(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.StaminaCurrentRef.Target, OnStamina);
                OnStamina(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.StaminaCurrentRegenRef.Target, OnStaminaRegen);
                OnStaminaRegen(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.WaterBalanceRef.Target, OnWaterBalance);
                OnWaterBalance(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.SatietyRef.Target, OnSatiety);
                OnSatiety(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.TemperatureRef.Target, OnEnvTemperature);
                OnEnvTemperature(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.ComfortTemperatureMinRef.Target, OnComfortTemperatureMin);
                OnComfortTemperatureMin(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.ComfortTemperatureMaxRef.Target, OnComfortTemperatureMax);
                OnComfortTemperatureMax(null, new AnyStatState());
                // _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.TemperatureBonusRef.Target, OnTemperatureBonus);
                // OnTemperatureBonus(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.ToxicRef.Target, OnEnvToxicLevel);
                OnEnvToxicLevel(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.OverheatRef.Target, OnOverheatRatio);
                OnOverheatRatio(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.HypotermiaRef.Target, OnHypothermiaRatio);
                OnHypothermiaRatio(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.IntoxicationRef.Target, OnIntoxicationRatio);
                OnIntoxicationRatio(null, new AnyStatState());

                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.InventoryItemsWeightRef.Target, OnInventoryItemsWeight);
                OnInventoryItemsWeight(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.DollItemsWeightRef.Target, OnDollItemsWeight);
                OnDollItemsWeight(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.IntermediateItemsWeightRef.Target, OnIntermediateItemsWeigh);
                OnIntermediateItemsWeigh(null, new AnyStatState());

                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.InventoryMaxWeightRef.Target, OnInventoryMaxWeight);
                OnInventoryMaxWeight(null, new AnyStatState());

                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.IncomingDamageModRef.Target, OnIncomingDamageMod);
                OnIncomingDamageMod(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.DmgResistanceSlashingRef.Target, OnDmgResistanceSlashing);
                OnDmgResistanceSlashing(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.ToxicResistanceRef.Target, OnToxicResistance);
                OnToxicResistance(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.DetoxicationRef.Target, OnDetoxication);
                OnDetoxication(null, new AnyStatState());

                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.CalorieConsumptionRef.Target, OnCaloriesCons);
                OnCaloriesCons(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.CalorieConsumptionModRef.Target, OnCaloriesConsMod);
                OnCaloriesConsMod(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.WaterConsumptionRef.Target, OnWaterCons);
                OnWaterCons(null, new AnyStatState());
                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.WaterConsumptionModRef.Target, OnWaterConsMod);
                OnWaterConsMod(null, new AnyStatState());


                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.SpeedFactorRef.Target, OnSpeedFactor);
                OnSpeedFactor(null, new AnyStatState());

                _hasStatsFullApiWrapper.EntityApi.UnsubscribeFromStats(_statsMapping.FallDamageModifierRef.Target, OnFallDamageModifier);
                OnFallDamageModifier(null, new AnyStatState());

                _hasStatsBroadcastApiWrapper.Dispose();
                _hasStatsBroadcastApiWrapper = null;
                _hasStatsFullApiWrapper.Dispose();
                _hasStatsFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _hasStatsBroadcastApiWrapper = EntityApi.GetWrapper<HasStatsBroadcastApi>(newEgo.OuterRef);
                _hasStatsFullApiWrapper = EntityApi.GetWrapper<HasStatsFullApi>(newEgo.OuterRef);

                _hasStatsBroadcastApiWrapper.EntityApi.SubscribeToStats(_statsMapping.HealthCurrentRef.Target, OnHealthCurrent);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.DamageRef.Target, OnDamage);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.MiningLootMultiplierRef.Target, OnMiningLootMult);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.OutgoingDamageModRef.Target, OnOutgoingDamageMod);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.StaminaCurrentRef.Target, OnStamina);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.StaminaCurrentRegenRef.Target, OnStaminaRegen);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.WaterBalanceRef.Target, OnWaterBalance);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.SatietyRef.Target, OnSatiety);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.TemperatureRef.Target, OnEnvTemperature);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.ComfortTemperatureMinRef.Target, OnComfortTemperatureMin);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.ComfortTemperatureMaxRef.Target, OnComfortTemperatureMax);

                // _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.TemperatureBonusRef.Target, OnTemperatureBonus);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.ToxicRef.Target, OnEnvToxicLevel);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.OverheatRef.Target, OnOverheatRatio);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.HypotermiaRef.Target, OnHypothermiaRatio);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.IntoxicationRef.Target, OnIntoxicationRatio);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.InventoryItemsWeightRef.Target, OnInventoryItemsWeight);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.DollItemsWeightRef.Target, OnDollItemsWeight);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.IntermediateItemsWeightRef.Target, OnIntermediateItemsWeigh);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.InventoryMaxWeightRef.Target, OnInventoryMaxWeight);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.IncomingDamageModRef.Target, OnIncomingDamageMod);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.DmgResistanceSlashingRef.Target, OnDmgResistanceSlashing);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.ToxicResistanceRef.Target, OnToxicResistance);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.DetoxicationRef.Target, OnDetoxication);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.CalorieConsumptionRef.Target, OnCaloriesCons);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.CalorieConsumptionModRef.Target, OnCaloriesConsMod);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.WaterConsumptionRef.Target, OnWaterCons);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.WaterConsumptionModRef.Target, OnWaterConsMod);

                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.SpeedFactorRef.Target, OnSpeedFactor);
                _hasStatsFullApiWrapper.EntityApi.SubscribeToStats(_statsMapping.FallDamageModifierRef.Target, OnFallDamageModifier);
            }

            HasPawn?.Invoke(newEgo != null);
        }

        private void OnHealthCurrent(StatResource statResource, AnyStatState state)
        {
            _healthCurrentPu.StatStateRp.Value = state;
        }

        private void OnStamina(StatResource statResource, AnyStatState state)
        {
            _staminaPu.StatStateRp.Value = state;
        }

        private void OnStaminaRegen(StatResource statResource, AnyStatState state)
        {
            _staminaRegenPu.StatStateRp.Value = state;
        }

        private void OnDamage(StatResource statResource, AnyStatState state)
        {
            _damageRp.Value = state.Value;
        }

        private void OnMiningLootMult(StatResource statResource, AnyStatState state)
        {
            _miningLootMultRp.Value = state.Value;
        }

        private void OnOutgoingDamageMod(StatResource statResource, AnyStatState state)
        {
            _outgoingDamageModRp.Value = state.Value;
        }

        private void OnWaterBalance(StatResource statResource, AnyStatState state)
        {
            _waterBalancePu.StatStateRp.Value = state;
        }

        private void OnSatiety(StatResource statResource, AnyStatState state)
        {
            _satietyPu.StatStateRp.Value = state;
        }

        private void OnEnvTemperature(StatResource statResource, AnyStatState state)
        {
            _envTemperaturePu.StatStateRp.Value = state;
        }

        // private void OnTemperatureBonus(StatResource statResource, AnyStatState state)
        // {
        //     RoughSetValue(state.Value, _temperatureBonusRp, Roughness100);
        // }

        private void OnComfortTemperatureMin(StatResource statResource, AnyStatState state)
        {
            _comfortTempMinRp.Value = state.Value;
        }

        private void OnComfortTemperatureMax(StatResource statResource, AnyStatState state)
        {
            _comfortTempMaxRp.Value = state.Value;
        }

        private void OnEnvToxicLevel(StatResource statResource, AnyStatState state)
        {
            _envToxicLevelPu.StatStateRp.Value = state;
        }

        private void OnOverheatRatio(StatResource statResource, AnyStatState state)
        {
            _overheatPu.StatStateRp.Value = state;
        }

        private void OnHypothermiaRatio(StatResource statResource, AnyStatState state)
        {
            _hypothermiaPu.StatStateRp.Value = state;
        }

        private void OnIntoxicationRatio(StatResource statResource, AnyStatState state)
        {
            _intoxicationPu.StatStateRp.Value = state;
        }

        private void OnInventoryItemsWeight(StatResource statResource, AnyStatState state)
        {
            _inventoryItemsWeightRp.Value = state.Value;
        }


        private void OnDollItemsWeight(StatResource statResource, AnyStatState state)
        {
            _dollItemsWeightRp.Value = state.Value;
        }

        private void OnIntermediateItemsWeigh(StatResource statResource, AnyStatState state)
        {
            _intermediateItemsWeightRp.Value = state.Value;
        }

        private void OnInventoryMaxWeight(StatResource statResource, AnyStatState state)
        {
            _inventoryMaxWeightRp.Value = state.Value;
        }

        private void OnIncomingDamageMod(StatResource statResource, AnyStatState state)
        {
            _incomingDamageModRp.Value = state.Value;
        }

        private void OnDmgResistanceSlashing(StatResource statResource, AnyStatState state)
        {
            _dmgResistanceSlashingRp.Value = state.Value;
        }

        private void OnToxicResistance(StatResource statResource, AnyStatState state)
        {
            _toxicResistanceRp.Value = state.Value;
        }

        private void OnDetoxication(StatResource statResource, AnyStatState state)
        {
            _detoxicationRp.Value = state.Value;
        }

        private void OnCaloriesCons(StatResource statResource, AnyStatState state)
        {
            _caloriesConsRp.Value = state.Value;
        }

        private void OnCaloriesConsMod(StatResource statResource, AnyStatState state)
        {
            _caloriesConsModRp.Value = state.Value;
        }

        private void OnWaterCons(StatResource statResource, AnyStatState state)
        {
            _waterConsRp.Value = state.Value;
        }

        private void OnWaterConsMod(StatResource statResource, AnyStatState state)
        {
            _waterConsModRp.Value = state.Value;
        }

        private void OnSpeedFactor(StatResource statResource, AnyStatState state)
        {
            _speedFactorRp.Value = state.Value;
        }

        private void OnFallDamageModifier(StatResource statResource, AnyStatState state)
        {
            _fallDamageModifierRp.Value = state.Value;
        }
    }
}