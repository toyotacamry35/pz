using ReactivePropsNs;
using Src.Effects;
using UnityEngine;

namespace Uins
{
    public interface IBadgePoint : IHasLookAtAnchor
    {
        //Нужен для подключения к GuiBadge тестовых данных
        bool HasPoint { get; }
        ReactiveProperty<bool> IsVisibleLogicallyRp { get; }
        ReactiveProperty<bool> IsSelectedRp { get; }
        Vector3 Position { get;  }
        bool IsDebug { get; }
    }
}