using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    [Localized]
    public class LocalizationKeysDef : BaseResource
    {
        public Dictionary<string, LocalizedString> LocalizedStrings { get; set; }
    }
}
