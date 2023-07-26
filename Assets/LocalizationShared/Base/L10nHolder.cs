using System;

namespace L10n
{
    public static class L10nHolder
    {
        private static LocalizationSource _instance;
        public static LocalizationSource Instance
        {
            get => _instance;
            set
            {
                _instance = value;
                L10nHolderInited?.Invoke();
            }
        }
        public static event Action L10nHolderInited;
    }
}