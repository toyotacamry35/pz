using System;
using System.Net;
using System.Net.Http;

namespace GeneratedCode.DeltaObjects
{
    public class HttpResponseData<T>
    {
        //Исходящие
        public Guid RequestGuid;
        public HttpMethod Method;
        public string QueryUrl;
        public string ApiHostName;
        public string AuthToken;

        /// <summary>
        /// Сериализованные данные запроса
        /// </summary>
        public string RequestMessageContent;

        //Входящие
        public HttpStatusCode StatusCode = HttpStatusCode.Unused;
        public bool IsSuccessStatusCode;

        /// <summary>
        /// Десериализованный объект ответа, содержимое свойства data или error.data
        /// </summary>
        public T Data;

        /// <summary>
        /// Сериализованные данные ответа
        /// </summary>
        public string ResponseMessageContent;

        //Входящие, если вернули ошибку
        public string ErrorDescription;
        public string ErrorCode;

        public HttpResponseData(Guid requestGuid)
        {
            RequestGuid = requestGuid;
        }

        private const string CuttedPartSubstitution = "...";

        private string CropText(string text, int maxLen = 6)
        {
            return (text == null || text.Length <= maxLen)
                ? text
                : text.Substring(0, Math.Max(maxLen - CuttedPartSubstitution.Length, 1)) + CuttedPartSubstitution;
        }

        public override string ToString()
        {
            return $"({GetType().NiceName()} Response: {nameof(IsSuccessStatusCode)}{IsSuccessStatusCode.AsSign()}, {nameof(StatusCode)}={StatusCode},\n" +
                   $"{nameof(ResponseMessageContent)}='{ResponseMessageContent}', {nameof(Data)}={Data},\n" +
                   $"{nameof(ErrorCode)}='{ErrorCode}', {nameof(ErrorDescription)}='{ErrorDescription}'\n" +
                   $"Request: {nameof(RequestGuid)}={RequestGuid}, {nameof(Method)}={Method}, {nameof(QueryUrl)}='{QueryUrl}', " +
                   $"{nameof(ApiHostName)}='{ApiHostName}', {nameof(RequestMessageContent)}='{RequestMessageContent}', " +
                   $"{nameof(AuthToken)}='{CropText(AuthToken)}')";
        }
    }
}