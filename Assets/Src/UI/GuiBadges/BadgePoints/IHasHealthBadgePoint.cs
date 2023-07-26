using ReactivePropsNs;

namespace Uins
{
    public interface IHasHealthBadgePoint : IBadgePoint
    {
        ReactiveProperty<float> CurrentHealthRp { get; }
        ReactiveProperty<float> MaxHealthRp { get; }
    }
}