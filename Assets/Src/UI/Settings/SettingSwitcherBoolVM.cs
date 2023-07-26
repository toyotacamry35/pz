using JetBrains.Annotations;

namespace Uins.Settings
{
    internal class SettingSwitcherBoolVM : SettingSwitcherImplementationBase<bool>
    {
        internal SettingSwitcherBoolVM(Definition definition, [NotNull] IApplyableCancelableProxy<bool> proxy) : base(definition, proxy)
        {}
        
        // --- Util types: ------------------------
        internal class Definition : SettingSwitcherDefinitionBase<bool>
        {}
    }
}