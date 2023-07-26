using System.Collections.Generic;
using System.Linq;
using L10n.Loaders;

namespace L10n
{
    public delegate void LocalizationChangedDelegate(string localizationName);

    public class LocalizationSource
    {
        public event LocalizationChangedDelegate LocalizationChanged;


        //=== Props ===========================================================

        /// <summary>
        /// Локализация по умолчанию, если не передана. Таковой не может являться dev-локализация
        /// </summary>
        public string DefaultCultureCode { get; }

        /// <summary>
        /// Доступные CultureData, без dev-культуры. Из них можно выбирать снаружи текущую культуру
        /// </summary>
        public CultureData[] AvailableCultures { get; }

        public TextCatalog TextCatalog => _currentLocalization?.TextCatalog;

        private CultureLocalization _currentLocalization;

        public CultureLocalization CurrentLocalization
        {
            get => _currentLocalization;
            set
            {
                if (_currentLocalization != value)
                {
                    _currentLocalization = value;
                    if (_currentLocalization != null)
                        LocalizationChanged?.Invoke(_currentLocalization.CultureData.Code);
                }
            }
        }


        //=== Ctor ============================================================

        public LocalizationSource(
            CultureData devCultureData,
            CultureData[] availableCultures,
            string defaultCultureCode,
            ILoader loader,
            string chosenCultureCode = null)
        {
            DefaultCultureCode = defaultCultureCode;
            AvailableCultures = availableCultures;
            var lst = new List<CultureData>();
            lst.Add(devCultureData);
            lst.AddRange(availableCultures);
            CultureLocalization.Init(loader, lst.ToArray());
            CurrentLocalization = CultureLocalization.GetLocalization(IsSuitableCultureCode(chosenCultureCode) ? chosenCultureCode : DefaultCultureCode);
        }


        //=== Public ==========================================================

        public void ChangeLocalization(string cultureName)
        {
            CurrentLocalization = CultureLocalization.GetLocalization(cultureName);
        }

        public bool IsSuitableCultureCode(string cultureCode)
        {
            return !string.IsNullOrEmpty(cultureCode) && AvailableCultures.Any(c => c.Code == cultureCode);
        }

        public override string ToString()
        {
            return $"({nameof(LocalizationSource)}: {nameof(CurrentLocalization)}: {CurrentLocalization})";
        }
    }
}