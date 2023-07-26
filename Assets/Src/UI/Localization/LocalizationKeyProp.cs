using System;
using Assets.Src.ResourceSystem.L10n;
using Uins;
using UnityEngine;

namespace L10n
{
    [Serializable]
    public class LocalizationKeyProp
    {
        [SerializeField]
        private LocalizationKeysDefRef JdbRef;

        [SerializeField]
        private string JdbKey;

        public LocalizedString LocalizedString
        {
            get
            {
                if (IsValid)
                {
                    return JdbRef.Target.LocalizedStrings[JdbKey];
                }

                var msg = string.IsNullOrEmpty(JdbKey)
                        ? "JdbKey is empty"
                        : JdbRef == null || JdbRef.Target == null
                            ? "JdbRef is null"
                            : JdbRef.Target.LocalizedStrings == null
                                ? "LocalizedStrings is null"
                                : $"LocalizedStrings don't contains key '{JdbKey}'";

                UI.ErrorOrWarn(null, true, msg);
                return LsExtensions.EmptyWarning;
            }
        }

        public LocalizationKeysDefRef Ref => JdbRef;
        public string Key => JdbKey;

        public bool IsValid => !string.IsNullOrEmpty(JdbKey) &&
                               JdbRef?.Target?.LocalizedStrings != null &&
                               JdbRef.Target.LocalizedStrings.ContainsKey(JdbKey);
    }
}