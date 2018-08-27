using MoveSharp.Authentication;
using RestSharp.Portable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Strava
{
    public class StaticAuthenticator : IOAuth2Authenticator
    {
        public string AccessToken { get; set; }

        public bool IsAuthenticated => true;

        public StaticAuthenticator(string accessToken)
        {
            AccessToken = accessToken;
        }

        public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;

        #region IAuthenticator implementation

        public bool CanPreAuthenticate(IRestClient client, IRestRequest request, System.Net.ICredentials credentials)
        {
            return true;
        }

        public bool CanPreAuthenticate(IHttpClient client, IHttpRequestMessage request, System.Net.ICredentials credentials)
        {
            return false;
        }

        public bool CanHandleChallenge(IHttpClient client, IHttpRequestMessage request, System.Net.ICredentials credentials, IHttpResponseMessage response)
        {
            return false;
        }

        public System.Threading.Tasks.Task PreAuthenticate(IRestClient client, IRestRequest request, System.Net.ICredentials credentials)
        {
            return Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(AccessToken))
                    request.AddHeader("Authorization", "Bearer " + AccessToken);
            });
        }

        public System.Threading.Tasks.Task PreAuthenticate(IHttpClient client, IHttpRequestMessage request, System.Net.ICredentials credentials)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task HandleChallenge(IHttpClient client, IHttpRequestMessage request, System.Net.ICredentials credentials, IHttpResponseMessage response)
        {
            throw new NotImplementedException();
        }

        public Task Authenticate()
        {
            return Task.Run(() => { });
        }

        #endregion
    }
}
