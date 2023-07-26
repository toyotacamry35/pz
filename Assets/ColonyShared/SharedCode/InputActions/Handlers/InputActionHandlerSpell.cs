using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Wizardry;
using static ColonyShared.SharedCode.Wizardry.SpellCastBuilder;

namespace ColonyShared.SharedCode.InputActions
{
    /// <summary>
    /// Работает по принципу "запустил и забыл". Нельзя использовать с бесконечными спеллами 
    /// </summary>
    public class InputActionHandlerSpell : IInputActionTriggerHandler
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISpellDoer _spellDoer;
        private readonly int _bindingId;
        private readonly bool _once;
        private readonly SpellDef _spell;
        private readonly bool _chain;
        private readonly IEnumerable<SpellParameterDef> _spellParameters;
        private readonly SpellParameterDef[] _spellParams;
        private bool _activated;
        private ISpellDoerCastPipeline _spellCast;

        public InputActionHandlerSpell(IInputActionHandlerSpellDescriptor desc, ISpellDoer spellDoer, int bindingId) : this(desc.Spell, desc.Parameters, desc.Chain, spellDoer, bindingId, false, desc) {}

        public InputActionHandlerSpell(IInputActionHandlerSpellOnceDescriptor desc, ISpellDoer spellDoer, int bindingId) : this(desc.Spell, desc.Parameters, desc.Chain, spellDoer, bindingId, true, desc) {}
        
        private InputActionHandlerSpell(SpellDef spell, IEnumerable<SpellParameterDef> @params, bool chain, ISpellDoer spellDoer, int bindingId, bool once, object desc)
        {
            _spell = spell ?? throw new ArgumentNullException($"{nameof(spell)}");
            _spellParameters = @params;
            _chain = chain;
            _spellDoer = spellDoer ?? throw new ArgumentNullException(nameof(spellDoer));
            _bindingId = bindingId;
            _once = once;
            if (_spell.IsInfinite) throw new InvalidOperationException($"Нельзя использовать бесконечный спел {_spell.____GetDebugAddress()} совместно с {nameof(InputActionHandlerSpell)} в {desc}");
        }
        
        public bool PassThrough => false;

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            if (@event.Activated && !_activated && (_spellCast == null || _spellCast.State > CastPipelineState.Casting))
            {
                if (_once) _activated = true;
                var prevSpell = _chain ? ctx.CurrentSpell : SpellId.Invalid;
                var spell = new SpellCastBuilder().SetSpell(_spell).SetParameters(_spellParameters, new SetParameterContext(prevSpellId: prevSpell));
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Cast spell | Binding:#{_bindingId} {_spell} PrevSpell:{prevSpell} Event:{@event} Context:{ctx}").Write();
                if (prevSpell.IsValid)
                    _spellCast = _spellDoer.DoCastChain(spell, prevSpell, @event.Awaiter);
                else
                    _spellCast = _spellDoer.DoCast(spell, @event.Awaiter);
            }
        }
        

        public void Dispose()
        {
        }

        public override string ToString() => $"{nameof(InputActionHandlerSpell)}(Spell:{_spell.____GetDebugAddress()})";
    }
}