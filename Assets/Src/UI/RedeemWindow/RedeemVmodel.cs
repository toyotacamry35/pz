using ReactivePropsNs;

namespace Uins
{
    public class RedeemVmodel : BindingVmodel
    {
        public ReactiveProperty<GetAccountDataState> GetAccountDataStateRp { get; }
            = new ReactiveProperty<GetAccountDataState>() {Value = Uins.GetAccountDataState.NothingHappenedYet};

        public ReactiveProperty<string> AcceptedEmailAddressRp { get; } = new ReactiveProperty<string>() {Value = ""};

        public IStream<bool> HasAcceptedEmailAddressStream { get; }

        public ReactiveProperty<string> TokenRp { get; } = new ReactiveProperty<string>() {Value = ""};

        public IStream<bool> HasTokenStream { get; }

        public ReactiveProperty<string> SessionIdRp { get; } = new ReactiveProperty<string>() {Value = ""};

        public IStream<bool> HasSessionIdStream { get; }

        public RedeemVmodel(IStream<string> tokenStream)
        {
            tokenStream.Bind(D, TokenRp);
            HasAcceptedEmailAddressStream = AcceptedEmailAddressRp.Func(D, s => !string.IsNullOrEmpty(s));
            HasTokenStream = TokenRp.Func(D, s => !string.IsNullOrEmpty(s));
            HasSessionIdStream = SessionIdRp.Func(D, s => !string.IsNullOrEmpty(s));
        }
    }

    public enum GetAccountDataState
    {
        NothingHappenedYet,
        WaitingForAccountData,
        AccountDataRequestFailed,
        HasAccountData
    }
}