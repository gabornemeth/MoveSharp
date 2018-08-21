using RestSharp.Portable.OAuth2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Authentication
{
    public abstract class OAuth2Authenticator : RestSharp.Portable.OAuth2.OAuth2Authenticator, IOAuth2Authenticator
    {
        public OAuth2Authenticator(OAuth2Client client) : base(client)
        {
        }

        public abstract bool IsAuthenticated { get; }

        public abstract string AccessToken { get; set; }

        public abstract Task Authenticate();

        /// <summary>
        /// Raised when an access token is received from the server.
        /// </summary>
        //public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;
        public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;

        protected void OnAccessTokenReceived(TokenReceivedEventArgs e)
        {
            AccessTokenReceived?.Invoke(this, e);
        }
    }
}
