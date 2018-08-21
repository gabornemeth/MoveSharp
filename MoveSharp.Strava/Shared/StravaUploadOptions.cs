//
// StravaUploadOptions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using System;
using MoveSharp.Models;

namespace MoveSharp.Strava
{
    /// <summary>
    /// Upload options for Strava. Based on <see cref="UploadOptions"/> .
    /// </summary>
    public class StravaUploadOptions : UploadOptions
    {
        /// <summary>
        /// True if activity is private, false is public
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// True if this activity is commuting, false otherwise
        /// </summary>
        public bool IsCommute { get; set; }

        /// <summary>
        /// Name of the activity. If null, Strava names it automatically.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
    }

    public class StravaUploadStatus : UploadStatus
    {
        /// <summary>
        /// Identifier of the upload process.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identifier of the activity created.
        /// </summary>
        public int ActivityId { get; set; }

        public string ExternalId { get; set; }

        public override bool IsCompleted
        {
            get => Status == "Your activity is ready.";
            protected set { }
        }

        public string ActivityUrl => $"https://www.strava.com/activities/{ActivityId}";
    }
}
