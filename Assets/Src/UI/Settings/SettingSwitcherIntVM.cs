using JetBrains.Annotations;

namespace Uins.Settings
{
    public class SettingSwitcherIntVM : SettingSwitcherImplementationBase<int>
    {
        internal SettingSwitcherIntVM(Definition definition, [NotNull] IApplyableCancelableProxy<int> proxy) : base(definition, proxy)
        {}

        // --- Util types: ------------------------
        internal class Definition : SettingSwitcherDefinitionBase<int>
        {}
    }
}