namespace Alpha.CloneAccount.Api.Filters
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Caching;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Configurations;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal sealed class HmacAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        private const long RequestMaxAgeInSeconds = 300; // 5 mins.
        private const string TokenAuthenticationScheme = "amx";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (string.IsNullOrWhiteSpace(AppSettings.ApiKey))
            {
                return;
            }

            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext), "Action Context cannot be null");
            }

            var request = actionContext.Request;
            var authorization = request.Headers.Authorization;

            if (authorization != null
                && TokenAuthenticationScheme.Equals(authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var rawAuthzHeader = authorization.Parameter;
                var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);

                if (autherizationHeaderArray != null)
                {
                    var machineIP = autherizationHeaderArray[0];
                    var incomingBase64Signature = autherizationHeaderArray[1];
                    var nonce = autherizationHeaderArray[2];
                    var requestTimeStamp = autherizationHeaderArray[3];

                    var isValid = IsValidRequest(request, machineIP, incomingBase64Signature, nonce, requestTimeStamp);

                    if (isValid)
                    {
                        Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(machineIP), null);
                    }
                    else
                    {
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
            else
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        private static string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {
            var credArray = rawAuthzHeader.Split(':');

            return credArray.Length == 4 ? credArray : null;
        }

        private static void HandleUnauthorizedRequest(HttpActionContext actionContext)
            => actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

        private static bool IsValidRequest(
            HttpRequestMessage request,
            string siteId,
            string incomingBase64Signature,
            string nonce,
            string requestTimeStamp)
        {
            if (IsReplayRequest(nonce, requestTimeStamp))
            {
                return false;
            }

            var requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
            var requestHttpMethod = request.Method.Method;
            var data = string.Format("{0}{1}{2}{3}{4}", siteId, requestHttpMethod, requestUri, requestTimeStamp, nonce);
            var secretKeyBytes = Convert.FromBase64String(AppSettings.ApiKey);
            var signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                var signatureBytes = hmac.ComputeHash(signature);

                return incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal);
            }
        }

        private static bool IsReplayRequest(string nonce, string requestTimeStamp)
        {
            if (MemoryCache.Default.Contains(nonce))
            {
                return true;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentTimeSpan = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTimeSpan.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if (((long)serverTotalSeconds - (long)requestTotalSeconds) > RequestMaxAgeInSeconds)
            {
                return true;
            }

            MemoryCache.Default.Add(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(RequestMaxAgeInSeconds));

            return false;
        }
    }
}