using Assets.Src.Aspects.Impl.Stats;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class StatViewModel : BindingViewModel
    {
        private StatResource _statResource;


        //=== Props ===========================================================

        private float _value = -123456;

        public float Value
        {
            get => _value;
            set
            {
                _value = value; //Отсутствие сравнения с предыдущим значением - не ошибка, здесь и далее
                FormattedStatValue = GetFormattedStatValue(_statResource, _value);
            }
        }

        private LocalizedString _statNameLs;

        [Binding]
        public LocalizedString StatNameLs
        {
            get => _statNameLs;
            set
            {
                _statNameLs = value;
                NotifyPropertyChanged();
            }
        }

        private LocalizedString _statValueLs;

        [Binding]
        public LocalizedString StatValueLs
        {
            get => _statValueLs;
            set
            {
                _statValueLs = value;
                NotifyPropertyChanged();
            }
        }

        private string _formattedStatValue;

        [Binding]
        public string FormattedStatValue //перекладывать в 1-й аргумент LocalizedTextMeshPro
        {
            get => _formattedStatValue;
            set
            {
                _formattedStatValue = value;
                NotifyPropertyChanged();
            }
        }


        //=== Public ==========================================================

        public void SetStatResourceAndValue(StatResource statResource, float val)
        {
            SetStatResource(statResource);
            Value = val;
        }

        public void SetStatResource(StatResource statResource)
        {
            _statResource = statResource;
            StatNameLs = statResource.StatNameLs;
            StatValueLs = GetStatValueLs(statResource);
        }

        /// <summary>
        /// Возвращает форматированное значение стата
        /// </summary>
        public static string GetFormattedStatValue(StatResource statResource, float statValue)
        {
            if (statResource.AssertIfNull(nameof(statResource)))
                return LsExtensions.EmptyWarning.Key;

            statValue += statResource.OptionalSummand;
            return string.IsNullOrEmpty(statResource.ValueFormat)
                ? statValue.ToString()
                : statValue.ToString(statResource.ValueFormat);
        }

        /// <summary>
        /// Возвращает LocalizedString, в которой будет отображаться (форматированное) значение стата
        /// </summary>
        public static LocalizedString GetStatValueLs(StatResource statResource, int unitsVariantIndex = 0)
        {
            if (statResource.AssertIfNull(nameof(statResource)))
                return LsExtensions.EmptyWarning;

            var valueUnitsLsArray = statResource.ValueUnitsLs;
            var ls = (valueUnitsLsArray == null || valueUnitsLsArray.Length == 0)
                ? LsExtensions.FirstArgumentOnly //нет данных, показываем просто отформатированное значение
                : (valueUnitsLsArray.Length <= unitsVariantIndex ? LsExtensions.EmptyWarning : valueUnitsLsArray[unitsVariantIndex]);
            return ls;
        }
    }
}