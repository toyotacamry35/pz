using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Aspects.Locomotion
{
    public class CharacterLocomotionBindingsDef : BaseResource
    {
        public ResourceRef<PredicateDef> JumpPredicate;
        public ResourceRef<PredicateDef> JumpStaminaPredicate;
        public ResourceRef<PredicateDef> SprintPredicate;
        public ResourceRef<PredicateDef> SprintStaminaPredicate;
        public ResourceRef<PredicateDef> FallingPredicate;
        public ResourceRef<SpellDef> JumpReaction;
        public ResourceRef<SpellDef> AirborneReaction;
        public ResourceRef<SpellDef> LandReaction;
        public ResourceRef<SpellDef> SprintReaction;
        public ResourceRef<SpellDef> SlippingReaction;
        public ResourceRef<InputActionTriggerDef> AirborneAttackHitInputAction;
    }
}