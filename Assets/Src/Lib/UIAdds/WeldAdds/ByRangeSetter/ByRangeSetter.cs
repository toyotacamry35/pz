using System;
using JetBrains.Annotations;
using UnityEngine;

namespace WeldAdds
{
    public enum UsageType
    {
        Undefined,
        Amount,
        SettingsIndex,
        Flag
    }

    /// <summary>
    /// Назначает таргету какие-то свойства в зависимости от того в каком из заданных диапазонов находится Amount
    /// </summary>
    public abstract class ByRangeSetter : MonoBehaviour
    {
        private bool _isAfterAwake;

        public RangeSettings[] RangeSettings;
        public Settings ElseSettings;

        [SerializeField, UsedImplicitly]
        [Obsolete("Use _usageType")]
        private bool _useFlagNorIndex;

        [SerializeField, UsedImplicitly]
        private UsageType _usageType;


        //=== Props ===============================================================

        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (!enabled)
                    return;

                if (_flag != value)
                {
                    _flag = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private float _amount;

        public float Amount
        {
            get => _amount;
            set
            {
                if (!Mathf.Approximately(_amount, value))
                {
                    _amount = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        private bool IsUsedFlagFinally => _usageType == UsageType.Flag || (_usageType != UsageType.Amount && _useFlagNorIndex);


        //=== Unity ===============================================================

        private void Awake()
        {
            if (!SetupOnAwake() || RangeSettings == null || RangeSettings.Length == 0)
            {
                enabled = false;
                return;
            }

            SyncIfWoken();
            _isAfterAwake = true;
        }



        //=== Protected ===========================================================

        /// <summary>
        /// Частная инициализация. Если возвратит false - не работаем
        /// </summary>
        protected abstract bool SetupOnAwake();

        /// <summary>
        /// Применить к таргету rangeSettings
        /// </summary>
        protected abstract void ApplySettingsToTarget(Settings settings);


        //=== Private =============================================================

        /// <summary>
        /// Синхронизировать внутренние доп. вычисления при изменении Amount. 
        /// Гарантируется, что выполняется только после Awake (т.е. все инициировано)
        /// </summary>
        private void SyncIfWoken()
        {
            var currentSettings = ElseSettings;

            if (RangeSettings != null)
            {
                if (_usageType == UsageType.SettingsIndex)
                {
                    if (Index >= 0 && Index < RangeSettings.Length)
                        currentSettings = RangeSettings[Index].Settings;
                }
                else if (IsUsedFlagFinally)
                {
                    if (Flag && RangeSettings.Length > 0)
                        currentSettings = RangeSettings[0].Settings;
                }
                else
                {
                    for (int i = 0, len = RangeSettings.Length; i < len; i++)
                    {
                        var rangeSettings = RangeSettings[i];
                        if (Amount >= rangeSettings.Min && Amount < rangeSettings.Max)
                        {
                            currentSettings = rangeSettings.Settings;
                            break;
                        }
                    }
                }
            }

            ApplySettingsToTarget(currentSettings);
        }
    }
}