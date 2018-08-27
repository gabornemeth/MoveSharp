//
// IOAuth2Authenticator.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using System;
using System.Threading.Tasks;

namespace MoveSharp.Authentication
{
    public interface IOAuth2Authenticator : RestSharp.Portable.IAuthenticator
    {
        bool IsAuthenticated { get; }

        string AccessToken { get; set; }

        Task Authenticate();

        /// <summary>
        /// Raised when an access token is received from the server.
        /// </summary>
        event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;
    }

    public class TokenReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Access token.
        /// </summary>
        public String Token { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TokenReceivedEventArgs class.
        /// </summary>
        /// <param name="token">The token received from the server.</param>
        public TokenReceivedEventArgs(string token)
        {
            Token = token;
        }
    }

}
