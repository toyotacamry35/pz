using System;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using Uins;
// using System.Linq;
// using Uins;
using UnityEngine;
using UnityWeld.Binding;

namespace L10n
{
    [Binding]
    public class LocalizedText : LocalizedObject
    {
        private const string EllipsisSubstitution = "…";

        public event Action<string> ValueChanged;

        [SerializeField, UsedImplicitly]
        private int _pluralNum;

        [SerializeField, UsedImplicitly]
        private string _textPrefix;

        [SerializeField, UsedImplicitly]
        private string _textSuffix;

        [SerializeField, UsedImplicitly]
        private int _maxSymbols;

        // public bool IsDebug;


        //=== Props ===========================================================

        private LocalizedString _localizedString;

        public LocalizedString LocalizedString
        {
            get => _localizedString;
            set
            {
                if (!_localizedString.Equals(value))
                {
                    _localizedString = value;
                    OnLocalizationChanged();
                }
            }
        }

        private string _value;

        /// <summary>
        /// Значение, полученное по GetString()
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    ValueChanged?.Invoke(_value);
                }
            }
        }

        public int PluralNum
        {
            get => _pluralNum;
            set
            {
                _pluralNum = value;
                OnLocalizationChanged();
            }
        }

        private object[] _actualArgs;

        [UsedImplicitly]
        public object[] ActualArgs
        {
            get => _actualArgs;
            set
            {
                _actualArgs = value;
                OnLocalizationChanged();
            }
        }

        /// <summary>
        /// Адаптер, чтобы не возиться с массивом аргументов, когда нужен всего один
        /// </summary>
        [UsedImplicitly]
        public object FirstActualArg
        {
            get => HasActualArgs ? ActualArgs[0] : null;
            set => ActualArgs = new[] {value};
        }

        protected bool HasActualArgs => _actualArgs != null && _actualArgs.Length > 0;


        //=== Protected =======================================================

        protected override void GetLocalization(LocalizationSource localizationSource)
        {
            if (localizationSource?.TextCatalog == null)
            {
                 Logger.IfError()?.Message("localizationSource?.TextCatalog is null").Write();;
                return;
            }

            var str = LocalizedString.GetString(localizationSource.TextCatalog, PluralNum, ActualArgs)?.Replace("\\n", "\n") ?? "";
            // if (string.IsNullOrWhiteSpace(str))
            //     UI.Logger.IfWarn()?.Message($"Empty text -- {LocalizedString}\n{transform.FullName()}").Write();

            if (_maxSymbols > 0 && str.Length > _maxSymbols)
            {
                UI.Logger.Warn(
                    $"Too much long text: {str.Length} > max={_maxSymbols}, cropped out! -- {LocalizedString}, " +
                    $"{localizationSource.TextCatalog.CultureData.Code}\n{transform.FullName()}");
                str = str.Substring(0, _maxSymbols - 1) + EllipsisSubstitution;
            }

            if (!string.IsNullOrWhiteSpace(str) && (!string.IsNullOrEmpty(_textSuffix) || !string.IsNullOrEmpty(_textPrefix)))
                str = $"{_textPrefix}{str}{_textSuffix}";
            Value = str;
            // if (IsDebug)
            // UI.CallerLog($"'{str}' -- PluralNum={PluralNum}, Arg[0]={FirstActualArg} -- {transform.FullName()}"); //2del
        }
    }
}