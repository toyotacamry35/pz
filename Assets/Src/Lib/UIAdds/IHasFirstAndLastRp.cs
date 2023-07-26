using ReactivePropsNs;

namespace Uins
{
    public interface IHasFirstAndLastRp
    {
        ReactiveProperty<bool> IsFirstRp { get; }
        ReactiveProperty<bool> IsLastRp { get; }
    }
}