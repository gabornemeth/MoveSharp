//
// ITracker.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    public class ActivityListResult
    {
        public IEnumerable<IActivitySummary> Activities { get; set; }
        public object NextPageToken { get; set; }
    }

    /// <summary>
    /// Activity tracker interface
    /// </summary>
    public interface ITracker
    {
        /// <summary>
        /// Name of the tracker
        /// </summary>
        string Name { get; }
        /// <summary>
        /// JSON representation of user credentials
        /// </summary>
        JObject Credentials { get; set; }
        bool IsPagingSupported { get; }
        /// <summary>
        /// Fetching list of activities.
        /// </summary>
        /// <param name="last">The last activity that is already fetched.</param>
        /// <returns></returns>
        Task<ActivityListResult> GetActivitiesAsync(object lastPageToken);

        Task<IActivitySummary> GetActivityAsync(string id);
        /// <summary>
        /// Retrieve user info
        /// </summary>
        /// <returns></returns>
        Task<IUserProfile> GetUserAsync();
        /// <summary>
        /// Logs in to the tracker
        /// </summary>
        /// <returns><see cref="System.Threading.Tasks.Task"/> to await</returns>
        Task LoginAsync();
        void Logout();
        /// <summary>
        /// Gets if the tracker is logged in
        /// </summary>
        bool IsLoggedIn { get; }
        event EventHandler LoginChanged;
        UploadOptions CreateUploadOptions();
        Task<UploadStatus> UploadAsync(Stream source, UploadOptions options);
        Task<UploadStatus> CheckUploadAsync(UploadStatus lastStatus);
        Task<MemoryActivity> DownloadAsync(IActivitySummary activity);
        Task DeleteAsync(IActivitySummary activity);
        /// <summary>
        /// Gets if we can upload to the tracker
        /// </summary>
        bool CanUpload { get; }
        /// <summary>
        /// Gets if we can download from the tracker
        /// </summary>
        bool CanDownload { get; }
        /// <summary>
        /// Gets if deleting activities is supported
        /// </summary>
        bool CanDelete { get; }
    }

    /// <summary>
    /// Authentication for a <c>Tracker</c>
    /// </summary>
    public interface ITrackerAuthentication
    {
        event EventHandler Authenticated;
        void OnAuthenticated();
        bool IsAuthenticated { get; }
        Task Authenticate();
        void Reset();
    }

    public interface IPPTTrackerAuthentication : ITrackerAuthentication
    {
        string UserName { get; }

        string Password { get; }
    }
}
