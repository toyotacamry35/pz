using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace L10n
{
    public class LocalizationDef : BaseResource
    {
        public Dictionary<string, TranslationDataExt> Translations { get; set; }

        public string PluralFormsRule { get; set; }

        public override string ToString()
        {
            return $"({____GetDebugRootName()}, {Translations?.Count} records)";
        }
    }
}