using System;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Wizardry;
using SharedCode.Utils;
using SharedCode.Wizardry;
using Uins;

namespace Src.Aspects.Locomotion
{
    public class CharacterLocomotionReactions : IDisposable
    {
        private readonly CharacterLocomotionBindingsDef _bindings;
        private readonly ISpellDoer _spellDoer;
        private readonly IInputActionStatesGenerator _inputReceiver;

        private readonly SpellDoerCastPipelineSet _sprintCast = new SpellDoerCastPipelineSet();
        private readonly SpellDoerCastPipelineSet _jumpCast = new SpellDoerCastPipelineSet();
        private readonly SpellDoerCastPipelineSet _airborneCast = new SpellDoerCastPipelineSet();
        private readonly SpellDoerCastPipelineSet _slippingCast = new SpellDoerCastPipelineSet();
        private readonly Guid _entityId;

        public CharacterLocomotionReactions(CharacterLocomotionBindingsDef bindings, ISpellDoer spellDoer, Guid entityId, IInputActionStatesGenerator inputReceiver)
        {
            _bindings = bindings ?? throw new ArgumentNullException(nameof(bindings));
            _spellDoer = spellDoer ?? throw new ArgumentNullException(nameof(spellDoer));
            _entityId = entityId;
            _inputReceiver = inputReceiver ?? throw new ArgumentNullException(nameof(inputReceiver));

            CheckIsNotInfinite(_bindings.LandReaction);
        }

        public void SprintStartReaction()
        {
            _sprintCast.Add(_spellDoer.DoCast(new SpellCastBuilder().SetSpell(_bindings.SprintReaction).Build()));
        }

        public void SprintEndReaction()
        {
            _sprintCast.StopCast();
        }

        public void JumpReaction()
        {
            _jumpCast.Add(_spellDoer.DoCast(new SpellCastBuilder().SetSpell(_bindings.JumpReaction).Build()));
        }

        public void AirborneReaction()
        {
            _airborneCast.Add(_spellDoer.DoCast(new SpellCastBuilder().SetSpell(_bindings.AirborneReaction).Build()));
        }

        public void SlippingStartReaction()
        {
            _slippingCast.Add(_spellDoer.DoCast(new SpellCastBuilder().SetSpell(_bindings.SlippingReaction).Build()));
        }

        public void SlippingEndReaction()
        {
            _slippingCast.StopCast();
        }

        public void LandReaction(float fallDistance)
        {
            _jumpCast.StopCast();
            _airborneCast.StopCast();
            _spellDoer.DoCast(new SpellCastBuilder().SetSpell(_bindings.LandReaction).SetDirection(new Vector3(fallDistance,0,0)).Build());
        }

        public void NotEnoughStaminaReaction()
        {
            if (SurvivalGuiNode.Instance.PlayerGuid == _entityId)
                HudGuiNode.StaminaFlash();
        }

        public void AirborneAttackHit()
        {
            _inputReceiver.PushTrigger(this, _bindings.AirborneAttackHitInputAction.Target);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        }

        public void AirborneAttackHitEnd(float fallDistance)
        {
            _spellDoer.DoCast(new SpellCastBuilder().SetSpell(_bindings.LandReaction).SetDirection(new Vector3(fallDistance,0,0)).Build());
            _inputReceiver.TryPopTrigger(this, _bindings.AirborneAttackHitInputAction.Target, true);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        }
        
        public void StopAll()
        {
            _jumpCast.StopCast();
            _airborneCast.StopCast();
            _slippingCast.StopCast();
            _sprintCast.StopCast();
            _inputReceiver.TryPopTrigger(this, _bindings.AirborneAttackHitInputAction.Target, true);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
        }
        
        public void Dispose()
        {
            StopAll();
        }

        private void CheckIsNotInfinite(SpellDef spell)
        {
            if (spell.IsInfinite) throw new Exception($"Spell {spell} can't be Infinite!");
        }
    }
}