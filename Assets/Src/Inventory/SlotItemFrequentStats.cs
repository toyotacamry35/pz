using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.Impl.Stats;
using Uins;
using UnityEngine;

namespace Assets.Src.Inventory
{
    public class SlotItemFrequentStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<StatResource, float> Stats { get; } = new Dictionary<StatResource, float>();

        public const float AlmostBrokenDurabilityRatio = 0.3f;
        private const float BrokenDurabilityRatio = 0.05f;


        //=== Props ===========================================================

        private StatResourcesDef _namedStatResources;

        private StatResourcesDef NamedStatResources
        {
            get
            {
                if (_namedStatResources == null)
                {
                    _namedStatResources = GlobalConstsHolder.StatResources;
                    _namedStatResources.AssertIfNull(nameof(_namedStatResources));
                }

                return _namedStatResources;
            }
        }

        private float _durability;

        public float Durability
        {
            get { return _durability; }
            set
            {
                if (!Mathf.Approximately(_durability, value))
                {
                    _durability = value;
                    NotifyPropertyChanged();
                    DurabilityRatio = GetDurabilityRatio();
                    DurabilityAbsRatio = GetDurabilityAbsRatio();
                    IsBroken = GetIsBroken();
                }
            }
        }

        private float _durabilityRatio;

        public float DurabilityRatio
        {
            get { return _durabilityRatio; }
            set
            {
                if (!Mathf.Approximately(_durabilityRatio, value))
                {
                    _durabilityRatio = value;
                    NotifyPropertyChanged();
                    IsAlmostBroken = GetIsAlmostBroken();
                }
            }
        }

        private float _durabilityAbsRatio;

        public float DurabilityAbsRatio
        {
            get { return _durabilityAbsRatio; }
            set
            {
                if (!Mathf.Approximately(_durabilityAbsRatio, value))
                {
                    _durabilityAbsRatio = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isBroken;

        public bool IsBroken
        {
            get { return _isBroken; }
            set
            {
                if (_isBroken != value)
                {
                    _isBroken = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isAlmostBroken;

        public bool IsAlmostBroken
        {
            get { return _isAlmostBroken; }
            set
            {
                if (_isAlmostBroken != value)
                {
                    _isAlmostBroken = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _durabilityMaxCurrent;

        public float DurabilityMaxCurrent
        {
            get { return _durabilityMaxCurrent; }
            set
            {
                if (!Mathf.Approximately(_durabilityMaxCurrent, value))
                {
                    _durabilityMaxCurrent = value;
                    NotifyPropertyChanged();
                    DurabilityRatio = GetDurabilityRatio();
                    DurabilityMaxCurrentRatio = GetDurabilityMaxCurrentRatio();
                }
            }
        }

        private float _durabilityMaxCurrentRatio;

        public float DurabilityMaxCurrentRatio
        {
            get { return _durabilityMaxCurrentRatio; }
            set
            {
                if (!Mathf.Approximately(_durabilityMaxCurrentRatio, value))
                {
                    _durabilityMaxCurrentRatio = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(DurabilityMaxCurrentInvertRatio));
                }
            }
        }

        public float DurabilityMaxCurrentInvertRatio => 1 - DurabilityMaxCurrentRatio;

        private float _durabilityMaxAbsolute;

        public float DurabilityMaxAbsolute
        {
            get { return _durabilityMaxAbsolute; }
            set
            {
                if (!Mathf.Approximately(_durabilityMaxAbsolute, value))
                {
                    _durabilityMaxAbsolute = value;
                    NotifyPropertyChanged();
                    HasDurability = GetHasDurability();
                    DurabilityMaxCurrentRatio = GetDurabilityMaxCurrentRatio();
                    DurabilityAbsRatio = GetDurabilityAbsRatio();
                }
            }
        }

        private bool _hasDurability;

        public bool HasDurability
        {
            get { return _hasDurability; }
            set
            {
                if (_hasDurability != value)
                {
                    _hasDurability = value;
                    NotifyPropertyChanged();
                    IsBroken = GetIsBroken();
                    IsAlmostBroken = GetIsAlmostBroken();
                }
            }
        }


        //=== Public ==============================================================

        public void OnStatsChanged(List<KeyValuePair<StatResource, float>> stats)
        {
            if (stats.AssertIfNull(nameof(stats)))
                return;

            foreach (var kvp in stats)
                OnStatChanged(kvp.Key, kvp.Value);
        }

        public void Reset()
        {
            Stats.Clear();

            DurabilityMaxAbsolute = GetDurabilityMaxAbsolute();
            DurabilityMaxCurrent = GetDurabilityMaxCurrent();
            Durability = GetDurability();
            HasDurability = GetHasDurability();
            DurabilityRatio = GetDurabilityRatio();
            DurabilityAbsRatio = GetDurabilityAbsRatio();
            IsBroken = GetIsBroken();
            IsAlmostBroken = GetIsAlmostBroken();
        }


        //=== Protected =======================================================

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //=== Private =========================================================

        private void OnStatChanged(StatResource statResource, float statValue)
        {
            Stats[statResource] = statValue;
            if (statResource == NamedStatResources.HealthCurrentStat)
                Durability = GetDurability();
            else if (statResource == NamedStatResources.HealthMaxStat)
                DurabilityMaxCurrent = GetDurabilityMaxCurrent();
            else if (statResource == NamedStatResources.HealthMaxAbsoluteStat)
                DurabilityMaxAbsolute = GetDurabilityMaxAbsolute();
        }

        private float GetStatValue(StatResource statResource)
        {
            float statValue;
            if (Stats.TryGetValue(statResource, out statValue))
                return statValue;

            return 0;
        }

        private float GetDurability()
        {
            return GetStatValue(NamedStatResources.HealthCurrentStat);
        }

        public bool GetHasDurability()
        {
            return !Mathf.Approximately(DurabilityMaxAbsolute, 0);
        }

        private float GetDurabilityMaxCurrent()
        {
            return GetStatValue(NamedStatResources.HealthMaxStat);
        }

        private float GetDurabilityMaxAbsolute()
        {
            return GetStatValue(NamedStatResources.HealthMaxAbsoluteStat);
        }

        private float GetDurabilityRatio()
        {
            return Mathf.Clamp01(Durability / Mathf.Max(DurabilityMaxCurrent, 1));
        }

        private float GetDurabilityAbsRatio()
        {
            return Mathf.Clamp01(Durability / Mathf.Max(DurabilityMaxAbsolute, 1));
        }

        private float GetDurabilityMaxCurrentRatio()
        {
            return Mathf.Clamp01(DurabilityMaxCurrent / Mathf.Max(DurabilityMaxAbsolute, 1));
        }

        private bool GetIsBroken()
        {
            return HasDurability && Durability < BrokenDurabilityRatio;
        }

        private bool GetIsAlmostBroken()
        {
            return HasDurability && DurabilityRatio < AlmostBrokenDurabilityRatio;
        }
    }
}