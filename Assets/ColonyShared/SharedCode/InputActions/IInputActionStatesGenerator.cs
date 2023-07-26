using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionStatesGenerator
    {
        void PushTrigger([NotNull] object causer, [NotNull] InputActionTriggerDef action, AwaitableSpellDoerCast awaitable = default);

        void PopTrigger([NotNull] object causer, [NotNull] InputActionTriggerDef action);
        
        bool TryPopTrigger([NotNull] object causer, [NotNull] InputActionTriggerDef action, bool all);

        void SetValue([NotNull] InputActionValueDef action, float value);
    }
    
#if false
    public interface IInputActionStateHolder
    {
        void PushAction([NotNull] object causer, [NotNull] InputActionTriggerDef action);

        void PushAction([NotNull] object causer, [NotNull] InputActionValueDef action, float value);

        void UpdateAction([NotNull] object causer, [NotNull] InputActionValueDef action, float value);

        void PopAction([NotNull] object causer, [NotNull] InputActionDef action);
        
        bool TryPopAction([NotNull] object causer, [NotNull] InputActionDef action, bool all);
    }
#endif

}