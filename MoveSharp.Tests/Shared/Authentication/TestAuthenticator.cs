//
// TestAuthenticator.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using RestSharp.Portable;
using System;
using System.Threading.Tasks;
using System.Net;
using MoveSharp.Authentication;

namespace MoveSharp.Tests.Authentication
{
    /// <summary>
    /// Authenticator used for tests
    /// </summary>
    public class TestAuthenticator : IOAuth2Authenticator
    {
        public string AccessToken { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        #region IAuthenticator implementation

        public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;

        public Task Authenticate()
        {
            return Task.Run(() => { });
        }

        public bool CanHandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response)
        {
            return false;
        }

        public bool CanPreAuthenticate(IRestClient client, IRestRequest request, System.Net.ICredentials credentials)
        {
            return true;
        }

        public  bool CanPreAuthenticate(IHttpClient client, IHttpRequestMessage request, System.Net.ICredentials credentials)
        {
            return false;
        }

        public Task HandleChallenge(IHttpClient client, IHttpRequestMessage request, ICredentials credentials, IHttpResponseMessage response)
        {
            throw new NotImplementedException();
        }

        public Task PreAuthenticate(IRestClient client, IRestRequest request, System.Net.ICredentials credentials)
        {
            return Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(AccessToken))
                    request.AddHeader("Authorization", "Bearer " + AccessToken);
            });
        }

        public Task PreAuthenticate(IHttpClient client, IHttpRequestMessage request, System.Net.ICredentials credentials)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
