//
// GoogleDriveTracker.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Models;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Http;
using System.Net.Http;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using MoveSharp.Format;
using RestSharp.Portable.OAuth2;
using Google.Apis.Util;

namespace MoveSharp.GoogleDrive
{
    public class GoogleDriveTracker : ITracker
    {
        private const string MimeTypeFolder = "application/vnd.google-apps.folder";

        private readonly string _folderName;
        private readonly string _folderId;
        private Google.Apis.Drive.v3.DriveService _service;

        private class Clock : IClock
        {
            public DateTime Now => DateTime.Now;
            public DateTime UtcNow => DateTime.UtcNow;
        }

        public GoogleDriveTracker(OAuth2Client client, string folderName)
        {
            _folderName = folderName ?? "";
            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = new HttpClientInitializer(client)
            };
            _service = new Google.Apis.Drive.v3.DriveService(initializer);
            _folderId = GetFolderIdFromName(_folderName);
        }

        /// <summary>
        /// Resolve the folder's name to the ID.
        /// </summary>
        /// <param name="folderPath">full path of the folder.</param>
        /// <returns>Identifier of the specified folder.</returns>
        private string GetFolderIdFromName(string folderPath)
        {
            var folders = folderPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            string id = "root";
            foreach (var folder in folders)
            {
                id = GetFolderId(folder, id);
                if (id == null)
                    return null;
            }

            return id;

            string GetFolderId(string folderName, string parentFolderId = null)
            {
                var request = _service.Files.List();
                request.Fields = "files(id, name, parents)";
                request.Q = $"mimeType = '{MimeTypeFolder}' and name = '{folderName}'";
                if (parentFolderId != null)
                    request.Q += $" and '{parentFolderId}' in parents";
                var list = request.Execute();
                if (list.Files.Count > 0)
                {
                    return list.Files[0].Id;
                }

                return null;
            }
        }

        public string Name => "Google Drive";

        public JObject Credentials { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsPagingSupported => throw new NotImplementedException();

        public bool IsLoggedIn => throw new NotImplementedException();

        public bool CanUpload => true;

        public bool CanDownload => true;

        public bool CanDelete => true;

        public event EventHandler LoginChanged;

        public Task DeleteAsync(IActivitySummary activity)
        {
            throw new NotImplementedException();
        }

        public async Task<MemoryActivity> DownloadAsync(string id)
        {
            var request = _service.Files.Get(id);
            var activity = new GoogleDriveMemoryActivity();
            using (var stream = new MemoryStream())
            {
                await request.DownloadAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var importer = new FitImporter(activity);
                await importer.LoadAsync(stream);
            }

            return activity;
        }

        public async Task<MemoryActivity> DownloadAsync(IActivitySummary activity)
        {
            if (activity is GoogleDriveActivity googleDriveActivity)
            {
                return await DownloadAsync(googleDriveActivity.Id);
            }
            else if (activity is GoogleDriveMemoryActivity memoryActivity)
            {
                return memoryActivity;
            }

            return null;
        }

        public async Task<ActivityListResult> GetActivitiesAsync(object lastPageToken)
        {
            if (_folderId == null)
                throw new Exception("Invalid folder has been specified.");

            var activities = new List<IActivitySummary>();
            if (lastPageToken as string == NoNextPage)
            {
                return new ActivityListResult
                {
                    Activities = activities,
                    NextPageToken = NoNextPage
                };
            }
            // get list of FIT files
            var request = _service.Files.List();
            request.Spaces = "drive";
            request.Fields = "nextPageToken, files";
            request.Q = $"'{_folderId}' in parents";// and mimeType contains 'fit'";
            request.OrderBy = "name desc";
            if (lastPageToken != null)
                request.PageToken = (string)lastPageToken;
            var files = await request.ExecuteAsync();
            foreach (var file in files.Files)
            {
                if (file.FileExtension != "fit")
                    continue;

                var activity = new GoogleDriveActivity(file);
                activities.Add(activity);
            }

            return new ActivityListResult
            {
                Activities = activities,
                NextPageToken = files.NextPageToken ?? NoNextPage
            };
        }

        private const string NoNextPage = "NO_NEXT_PAGE";

        public async Task<IActivitySummary> GetActivityAsync(string id)
        {
            return await DownloadAsync(id);
        }

        public async Task<IUserProfile> GetUserAsync()
        {
            var request = _service.About.Get();
            request.Fields = "user";
            var about = await request.ExecuteAsync();
            return new GoogleUser
            {
                Name = about.User.DisplayName,
                Description = about.User.EmailAddress,
                IsAuthenticated = true,
                ProfileImageUrl = about.User.PhotoLink
            };
        }

        public Task LoginAsync()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<UploadStatus> UploadAsync(Stream source, UploadOptions options)
        {
            var file = new Google.Apis.Drive.v3.Data.File();
            file.Name = options.FileName;
            file.Parents = new List<string>(new[] { _folderId });
            var request = _service.Files.Create(file, source, "application/octet-stream");
            request.Fields = "id, webViewLink";
            var uri = await request.InitiateSessionAsync();
            var progress = await request.UploadAsync();
            return new GoogleDriveUploadStatus(uri.AbsoluteUri, request);
        }

        public Task<UploadStatus> CheckUploadAsync(UploadStatus lastStatus)
        {
            var googleStatus = (GoogleDriveUploadStatus)lastStatus;
            googleStatus.MediaUpload.GetProgress();

            return Task.FromResult(googleStatus as UploadStatus);
        }

        public UploadOptions CreateUploadOptions()
        {
            return new UploadOptions();
        }

        private class HttpClientInitializer : IConfigurableHttpClientInitializer,
            IHttpExecuteInterceptor,
            IHttpUnsuccessfulResponseHandler
        {

            private readonly OAuth2Client _client;
            private readonly IAccessMethod _accessMethod = new BearerToken.AuthorizationHeaderAccessMethod();

            public HttpClientInitializer(OAuth2Client client)
            {
                _client = client ?? throw new ArgumentNullException(nameof(client));
            }

            public async Task<bool> HandleResponseAsync(HandleUnsuccessfulResponseArgs args)
            {
                if (args.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await _client.GetCurrentToken();
                    return true;
                }

                return false;
            }

            public void Initialize(ConfigurableHttpClient httpClient)
            {
                httpClient.MessageHandler.AddExecuteInterceptor(this);
                httpClient.MessageHandler.AddUnsuccessfulResponseHandler(this);
            }

            public Task InterceptAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _accessMethod.Intercept(request, _client.AccessToken);
                return Task.FromResult(0);
            }
        }
    }
}
