using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.ResourcesSystem.Base;

namespace ResourceSystem.Aspects
{
    public class AttackActionsModifierDef : AttackModifierDef
    {
        public AttackActionTarget Target = AttackActionTarget.Victim;
        public ResourceRef<AttackActionDef> AddAction;
        public ResourceRef<AttackActionDef> RemoveAction;
    }
    
    public enum AttackActionTarget { Victim, Attacker }
}