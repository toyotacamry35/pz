using System.ComponentModel;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public interface IStatusEffectVM : INotifyPropertyChanged
    {
        /// <summary>
        /// SyncTime Milliseconds
        /// </summary>
        [Binding]
        long StartTime { get; }

        /// <summary>
        /// Duration Milliseconds
        /// </summary>
        [Binding]
        long Duration { get; }

        [Binding]
        bool IsInfinite { get; }

        [Binding]
        Sprite Sprite { get; }

        [Binding]
        LocalizedString Description { get; }
    }
}