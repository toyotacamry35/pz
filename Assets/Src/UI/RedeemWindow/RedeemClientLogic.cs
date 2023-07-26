using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using GeneratedCode.DeltaObjects;
using Newtonsoft.Json;
using ReactivePropsNs;

namespace Uins
{
    public class RedeemClientLogic : IDisposable
    {
        private const int SendAttemptMaxCount = 3;
        private const float BetweenSendsPeriod = 10;

        private bool _isEmailSendingAllowed;
        private bool _isCodeSendingAllowed;

        private DisposableComposite _d = new DisposableComposite();
        private ServerServicesConfigDef _serverServicesConfigDef;
        private string _userId;
        private DateTime _lastEmailSendDt;
        private DateTime _lastCodeSendDt;
        private bool _isTestMode;
        private string _platformUrl;


        //=== Props ===========================================================

        public ReactiveProperty<bool> SendMailErrorRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<bool> SendCodeErrorRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public RedeemVmodel RedeemVmodel { get; }

        public IStream<bool> IsEmailSendingAllowedStream { get; }

        public IStream<bool> IsCodeSendingAllowedStream { get; }


        //=== Ctor ============================================================

        public RedeemClientLogic(ServerServicesConfigDef serverServicesConfigDef, bool isTestMode)
        {
            _isTestMode = isTestMode;
            _serverServicesConfigDef = serverServicesConfigDef;
            
            var platformApiTokenStream = GameState.Instance.PlatformApiTokenRp;

            _platformUrl = StartupParams.Instance.PlatformParams.PlatformUrl;
            if (string.IsNullOrEmpty(_platformUrl))
                _platformUrl = serverServicesConfigDef.APIEndpoint;
            if (string.IsNullOrEmpty(_platformUrl))
                UI.Logger.IfError()?.Message($"{nameof(_platformUrl)} is empty").Write();

            RedeemVmodel = new RedeemVmodel(platformApiTokenStream);
            var ticker = TimeTicker.Instance.GetLocalTimer(1);

            var isEmailSendingAllowedByStateStream = RedeemVmodel.HasTokenStream
                .Zip(_d, RedeemVmodel.GetAccountDataStateRp)
                .Zip(_d, RedeemVmodel.HasAcceptedEmailAddressStream)
                .Func(_d, (hasToken, state, hasAcceptedEmailAddress) => hasToken && state == GetAccountDataState.HasAccountData && !hasAcceptedEmailAddress);

            IsEmailSendingAllowedStream = isEmailSendingAllowedByStateStream
                .Zip(_d, ticker)
                .Func(_d, (isAllowed, dt) => isAllowed && (dt - _lastEmailSendDt).TotalSeconds > BetweenSendsPeriod);

            IsEmailSendingAllowedStream.Action(_d, b => _isEmailSendingAllowed = b);

            var isCodeSendingAllowedByStateStream = RedeemVmodel.HasTokenStream
                .Zip(_d, RedeemVmodel.GetAccountDataStateRp)
                .Zip(_d, RedeemVmodel.HasAcceptedEmailAddressStream)
                .Zip(_d, RedeemVmodel.HasSessionIdStream)
                .Func(_d, (token, state, mail, session) => token && state == GetAccountDataState.HasAccountData && !mail && session);

            IsCodeSendingAllowedStream = isCodeSendingAllowedByStateStream
                .Zip(_d, ticker)
                .Func(_d, (isAllowed, dt) => isAllowed && (dt - _lastCodeSendDt).TotalSeconds > BetweenSendsPeriod);

            IsCodeSendingAllowedStream.Action(_d, b => _isCodeSendingAllowed = b);

            platformApiTokenStream.Action(_d, OnTokenChanged);
        }


        //=== Public ==========================================================

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public async void SendEmail(string email)
        {
            if (!_isEmailSendingAllowed)
            {
                UI.Logger.Error("Unable to send email");
                return;
            }

            SendMailErrorRp.Value = false;
            HttpResponseData<EmailResponse> emailResponseData = null;
            var attemptCount = 0;
            while (attemptCount <= SendAttemptMaxCount && emailResponseData?.Data == null)
            {
                attemptCount++;
                UI.CallerLog($"Attempt {attemptCount}"); //2del

                emailResponseData = await HttpHelperUtility.HttpSend<EmailResponse>(
                    false,
                    HttpMethod.Put,
                    null,
                    _platformUrl,
                    _serverServicesConfigDef.APIEndpoint_EmailPut,
                    RedeemVmodel.TokenRp.Value,
                    new EmailRequest()
                    {
                        email = email,
                        userId = _userId
                    });
            }

            UnityQueueHelper.RunInUnityThreadNoWait(() => { OnEmailResponse(emailResponseData); });
        }

        public async void SendCode(string code)
        {
            if (!_isCodeSendingAllowed)
            {
                UI.Logger.Error("Unable to send code");
                return;
            }

            SendCodeErrorRp.Value = false;
            HttpResponseData<CodeResponse> codeResponseData = null;
            var attemptCount = 0;
            while (attemptCount <= SendAttemptMaxCount && (codeResponseData == null || codeResponseData.StatusCode == HttpStatusCode.Unused))
            {
                attemptCount++;
                UI.CallerLog($"Attempt {attemptCount}"); //2del

                codeResponseData = await HttpHelperUtility.HttpSend<CodeResponse>(
                    false,
                    HttpMethod.Post,
                    null,
                    _platformUrl,
                    _serverServicesConfigDef.APIEndpoint_CodePost,
                    RedeemVmodel.TokenRp.Value,
                    new CodeRequest()
                    {
                        code = code,
                        sessionId = RedeemVmodel.SessionIdRp.Value
                    });
            }

            UnityQueueHelper.RunInUnityThreadNoWait(() => { OnCodeResponse(codeResponseData); });
        }

        public void Dispose()
        {
            RedeemVmodel?.Dispose();
            _d.Dispose();
        }


        //=== Private =========================================================

        private void OnTokenChanged(string token)
        {
            RedeemVmodel.TokenRp.Value = token;
            RedeemVmodel.GetAccountDataStateRp.Value = GetAccountDataState.WaitingForAccountData;
            GetProfile();
        }

        private async void GetProfile()
        {
            var profileResponseData = await HttpHelperUtility.HttpSend<ProfileResponse>(
                false,
                HttpMethod.Get,
                null, //null, _serverServicesConfigDef.APIHostname
                _platformUrl,
                _serverServicesConfigDef.APIEndpoint_ProfileGet,
                RedeemVmodel.TokenRp.Value);
            UnityQueueHelper.RunInUnityThreadNoWait(() => { OnProfileResponse(profileResponseData); });
        }

        private void OnProfileResponse(HttpResponseData<ProfileResponse> profileResponseData)
        {
            var profileData = profileResponseData.Data;
            if (profileData == null || !profileResponseData.IsSuccessStatusCode)
                UI.Logger.IfWarn()?.Message($"Profile request is failed! -- {profileResponseData}").Write();

            RedeemVmodel.AcceptedEmailAddressRp.Value = string.IsNullOrEmpty(profileData?.email) ||
                                                        (_isTestMode && profileData.email.StartsWith("empty"))
                ? ""
                : profileData.email;
            _userId = string.IsNullOrEmpty(profileData?.id) ? "" : profileData.id;

            RedeemVmodel.GetAccountDataStateRp.Value = profileData != null
                ? GetAccountDataState.HasAccountData
                : GetAccountDataState.AccountDataRequestFailed;
        }

        private void OnEmailResponse(HttpResponseData<EmailResponse> emailResponseData)
        {
            _lastEmailSendDt = DateTime.Now;
            if (string.IsNullOrEmpty(emailResponseData.Data?.sessionId))
            {
                UI.Logger.Error("Send mail attempt is failed");
                SendMailErrorRp.Value = true;
            }
            else
            {
                RedeemVmodel.SessionIdRp.Value = emailResponseData.Data.sessionId;
            }
        }

        private void OnCodeResponse(HttpResponseData<CodeResponse> codeResponseData)
        {
            _lastCodeSendDt = DateTime.Now;
            if (codeResponseData.Data == null)
            {
                UI.Logger.Error("Send code attempt is failed");
                SendCodeErrorRp.Value = true;
                return;
            }

            GetProfile();
        }


        //=== Subclasses ==============================================================================================

        public class ProfileResponse
        {
            public string id { get; set; }
            public string username { get; set; }
            public string email { get; set; }

            public override string ToString()
            {
                return $"id='{id}' email='{email}' name='{username}'";
            }
        }

        public class EmailRequest
        {
            public string userId { get; set; }
            public string email { get; set; }
        }

        public class EmailResponse
        {
            public string sessionId { get; set; }
            public string resendTimeout { get; set; }
            public string contact { get; set; }
            public bool isResend { get; set; }
            public string method { get; set; }

            public override string ToString()
            {
                return $"{nameof(contact)}='{contact}' {nameof(sessionId)}='{sessionId}' {nameof(resendTimeout)}={resendTimeout}, " +
                       $"{nameof(isResend)}{isResend.AsSign()}, {nameof(method)}='{method}'";
            }
        }

        public class CodeRequest
        {
            public string sessionId { get; set; }
            public string code { get; set; }
        }

        public class CodeResponse
        {
            [JsonProperty("type")]
            public string theType { get; set; }

            public override string ToString()
            {
                return $"{nameof(theType)}='{theType}'";
            }
        }
    }
}