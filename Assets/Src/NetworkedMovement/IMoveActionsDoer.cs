using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.Wizardry;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public interface IMoveActionsDoer
    {
        bool OnMoveEffectStarted(SpellId orderId, MoveEffectDef effect, SpellDef spell, SpellPredCastData cast);

        void OnMoveEffectFinished(SpellId orderId, MoveEffectDef effect);
    }
}