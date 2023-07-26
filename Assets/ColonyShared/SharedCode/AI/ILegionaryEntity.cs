using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Assets.ColonyShared.SharedCode.Entities;
using ColonyShared.SharedCode.Entities.Reactions;
using SharedCode.Entities;
using SharedCode.MovementSync;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using Assets.ColonyShared.SharedCode.Wizardry;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Entities;

namespace SharedCode.AI
{
    [GenerateDeltaObjectCode]
    public interface ILegionaryEntity : IEntity, IHasWizardEntity, IHasHealth, 
                                        IHasMortal, IHasCorpseSpawner, IHasBrute, 
                                        IHitZonesOwner, IWorldObject, IHasStatsEngine, 
                                        IHasMobMovementSync, IHasSpawnedObject, IHasDestroyable,
                                        IHasReactionsOwner, IHasAnimationDoerOwner, IHasDoll, IHasContainerApi,
                                        IBank, IHasBuffs, IHasInputActionHandlers, IHasAttackEngine, IHasLocomotionOwner, 
                                        IHasAiTargetRecipient, IHasFaction, IHasSquadId, IHasIncomingDamageMultiplier
    {
    }

    public interface IIsDummyLegionary
    {

    }
}
