using ReactivePropsNs;

namespace Uins
{
    public class TimeLeftVM : BindingVmodel
    {
        public IStream<long> TimeLeftSeconds { get; }

        public TimeLeftVM(IStream<long> timeLeftSeconds)
        {
            TimeLeftSeconds = timeLeftSeconds;
        }
    }
}