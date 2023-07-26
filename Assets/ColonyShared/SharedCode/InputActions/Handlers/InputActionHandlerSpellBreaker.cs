using System;
using ColonyShared.GeneratedCode.Shared.Aspects;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using NLog;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils.DebugCollector;
using SharedCode.Utils.Extensions;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionHandlerSpellBreaker  : IInputActionTriggerHandler, IInputActionHandlerSpellBreaker
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISpellDoer _spellDoer;
        private readonly OuterRef _entityRef;
        private readonly IEntitiesRepository _repo;
        private readonly int _bindingId;
        private readonly When _when;
        private readonly FinishReasonType _finishReason;
        private bool _activated;

        public InputActionHandlerSpellBreaker(IInputActionHandlerSpellBreakerDescriptor desc, ISpellDoer spellDoer, OuterRef entityRef, IEntitiesRepository repo, int bindingId)
        {
            _spellDoer = spellDoer ?? throw new ArgumentNullException(nameof(spellDoer));
            _entityRef = entityRef.IsValid ? entityRef : throw new ArgumentException(nameof(entityRef));;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));;
            _bindingId = bindingId;
            _when = desc.When;
            _finishReason = desc.FinishReason;
        }
        
        public bool PassThrough => _activated;

        public void ProcessEvent(InputActionTriggerState @event, InputActionHandlerContext ctx, bool inactive)
        {
            bool activate = false;
            switch (_when)
            {
                case When.Activated:
                    if (@event.Activated)
                        activate = true;
                    break;
                case When.Active:
                    if (@event.Active)
                        activate = true;
                    break;
                case When.Deactivated:
                    if (@event.Deactivated)
                        activate = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"_when={_when}");
            }
            
            if (activate && !_activated)
            {
                _activated = true;
                if(Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Break Spell | Binding:#{_bindingId} SpellId:{ctx.CurrentSpell} Event:{@event} Context:{ctx}").Write();
                Collect.IfActive?.Event("InputActionHandlerSpellBreaker", _entityRef);
                var pipeline = _spellDoer.GetPipeline(ctx.CurrentSpell);
                if (pipeline != null)
                { // Через spelldoer можно остановить только спеллы запущенные через spelldoer. Но зато локальное выполнение будет остановлено мгновенно (без раундтрипа через сервак)
                    pipeline.StopCast(_finishReason);
                }
                else
                { // Если же спелл запущен не через spelldoer (например с сервака), то останавливаем его через wizard'а
                    var entityRef = _entityRef.To<IHasWizardEntityClientFull>();
                    var finishReason = _finishReason == FinishReasonType.Fail ? SpellFinishReason.FailOnDemand : SpellFinishReason.SucessOnDemand;
                    AsyncUtils.RunAsyncTask(() => ClusterHelpers.UseWizard(entityRef, _repo, wizard => wizard.StopCastSpell(ctx.CurrentSpell, finishReason)));
                }
            }
        }

        public void Dispose() {}

        public override string ToString() => $"{nameof(InputActionHandlerSpellBreaker)}";
    }
}