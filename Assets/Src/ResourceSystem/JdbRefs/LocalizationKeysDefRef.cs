using Assets.Src.ResourcesSystem.Base;
using System;
using L10n;

namespace Assets.Src.ResourceSystem.L10n
{
    [Serializable]
    public class LocalizationKeysDefRef : JdbRef<LocalizationKeysDef>
    {
#if UNITY_EDITOR
        public EditorBaseResourceWrapper<LocalizationKeysDef> AlwaysFreshTarget => 
            new EditorBaseResourceWrapper<LocalizationKeysDef>(_metadata?.GetFullTreeCopy<LocalizationKeysDef>());
#endif
    }
}