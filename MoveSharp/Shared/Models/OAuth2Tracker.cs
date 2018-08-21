//
// OAuth2Tracker.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using MoveSharp.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MoveSharp.Models
{
    /// <summary>
    /// Base class for OAuth2 based activity trackers
    /// </summary>
    public abstract class OAuth2Tracker : ITracker
    {
        private IOAuth2Authenticator _authenticator;
        protected IOAuth2Authenticator Authenticator
        {
            get
            {
                return _authenticator;
            }
            set
            {
                _authenticator = value;
                if (_authenticator != null)
                    _authenticator.AccessTokenReceived += authenticator_AccessTokenReceived;
            }
        }

        private JObject _credentials;

        public JObject Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                if (_credentials != value)
                {
                    _credentials = value;
                    if (_credentials.HasValue("AccessToken"))
                    {
                        Authenticator.AccessToken = _credentials.Value<string>("AccessToken");
                        IsLoggedIn = true;
                    }
                    else
                        IsLoggedIn = false;
                }
            }
        }

        public async Task LoginAsync()
        {
            // Login to Strava
            if (Authenticator.IsAuthenticated)
                IsLoggedIn = true;
            else
                await Authenticator.Authenticate();
        }

        public void Logout()
        {
            if (Authenticator.IsAuthenticated)
            {
                // forget access token
                Authenticator.AccessToken = null;
                IsLoggedIn = false;
            }
        }

        private bool _isLoggedIn;
        /// <summary>
        /// True if logged in, otherwise false
        /// </summary>
        public bool IsLoggedIn
        {
            get
            {
                return _isLoggedIn;
            }
            private set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    LoginChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public abstract string Name { get; }
        public abstract bool IsPagingSupported { get; }
        public abstract bool CanUpload { get; }
        public abstract bool CanDownload { get; }
        public abstract bool CanDelete { get; }

        public event EventHandler LoginChanged;

        void authenticator_AccessTokenReceived(object sender, TokenReceivedEventArgs e)
        {
            // successful authentication. save the access token
            var credentials = new JObject();
            credentials.Add("AccessToken", e.Token);
            Credentials = credentials;
            IsLoggedIn = true;
        }

        public abstract Task<ActivityListResult> GetActivitiesAsync(object lastPageToken);
        public abstract Task<IActivitySummary> GetActivityAsync(string id);
        public abstract Task<IUserProfile> GetUserAsync();
        public abstract Task<UploadStatus> UploadAsync(Stream source, UploadOptions options);
        public abstract Task<UploadStatus> CheckUploadAsync(UploadStatus lastStatus);
        public abstract Task<MemoryActivity> DownloadAsync(IActivitySummary activity);
        public abstract Task DeleteAsync(IActivitySummary activity);
        public abstract UploadOptions CreateUploadOptions();
    }
}
