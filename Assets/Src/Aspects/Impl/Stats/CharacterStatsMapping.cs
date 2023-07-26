using Assets.Src.ResourceSystem.L10n;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Stats/CharacterStatsMapping")]
public class CharacterStatsMapping : ScriptableObject
{
    public StatResourceRef StaminaCurrentRef;

    public StatResourceRef HealthCurrentRef;

    public StatResourceRef SatietyRef;
    public StatResourceRef WaterBalanceRef;

    public StatResourceRef ToxicRef;

    public StatResourceRef TemperatureRef;
    public StatResourceRef TemperatureBonusRef;
    public StatResourceRef ComfortTemperatureMinRef;
    public StatResourceRef ComfortTemperatureMaxRef;

    public StatResourceRef OverheatRef;
    public StatResourceRef HypotermiaRef;
    public StatResourceRef IntoxicationRef;

    public StatResourceRef InventoryMaxWeightRef;
    public StatResourceRef InventoryItemsWeightRef;
    public StatResourceRef DollItemsWeightRef;
    public StatResourceRef IntermediateItemsWeightRef;

    public StatResourceRef DmgResistanceSlashingRef;

    [FormerlySerializedAs("LightAttackDamageRef")]
    public StatResourceRef DamageRef;
    public StatResourceRef StaminaCurrentRegenRef;

    public StatResourceRef SpeedFactorRef;

    public StatResourceRef MiningLootMultiplierRef;
    public StatResourceRef OutgoingDamageModRef;
    public StatResourceRef IncomingDamageModRef;

    public StatResourceRef CalorieConsumptionRef;
    public StatResourceRef CalorieConsumptionModRef;
    public StatResourceRef WaterConsumptionRef;
    public StatResourceRef WaterConsumptionModRef;

    public StatResourceRef DetoxicationRef;
    public StatResourceRef ToxicResistanceRef;
    public StatResourceRef FallDamageModifierRef;
}