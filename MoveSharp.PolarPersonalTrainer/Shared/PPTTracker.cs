//
// PPTTracker.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolarPersonalTrainerLib;
using Newtonsoft.Json.Linq;
using XTools.Diagnostics;
using SharpGeo;
using MoveSharp.Models;
using System.IO;

namespace MoveSharp.PolarPersonalTrainer
{
    /// <summary>
    /// PolarPersonalTrainer.com tracker
    /// </summary>
    public class PPTTracker : ITracker
    {
        private PPTExport _client;
        private IPPTTrackerAuthentication _auth;

        public PPTTracker(IPPTTrackerAuthentication authentication)
        {
            _auth = authentication;
            _auth.Authenticated += OnAuthenticated;
        }

        public bool IsPagingSupported
        {
            get
            {
                return false;
            }
        }

        void OnAuthenticated(object sender, EventArgs e)
        {
            Credentials = new JObject { { "UserName", _auth.UserName }, { "Password", _auth.Password } };
            OnLoginChanged();
        }

        public string Name
        {
            get { return "Polar Personal Trainer"; }
        }

        public async Task<ActivityListResult> GetActivitiesAsync(object lastPageToken)
        {
            if (_client == null)
                throw new Exception("No credentials have been set.");

            var activities = new List<IActivitySummary>();
            var endTime = (DateTime)lastPageToken;
            var startTime = endTime.AddMonths(-1);
            var exercises = await _client.GetExercises(startTime, endTime);
            foreach (var exercise in exercises.OrderBy(exercise => -exercise.StartTime.Ticks))
                activities.Add(new PPTActivitySummary(exercise));
            return new ActivityListResult
            {
                Activities = activities,
                NextPageToken = startTime
            };
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
                    if (_credentials.HasValues)
                    {
                        _client = new PPTExport(_credentials.Value<string>("UserName"), _credentials.Value<string>("Password"));
                        OnLoginChanged();
                    }
                    else
                    {
                        _client = null;
                        OnLoginChanged();
                    }
                }
            }
        }

        public async Task<IUserProfile> GetUserAsync()
        {
            if (_client == null)
                return null;

            var user = await _client.GetUser();
            return new PolarUserProfile(user);
        }

        public async Task LoginAsync()
        {
            if (_auth.IsAuthenticated)
                return;

            _auth.Authenticate();
        }

        public void Logout()
        {
            if (!IsLoggedIn)
                return; // no need to do anything

            _client = null;
            OnLoginChanged();
        }

        public bool IsLoggedIn
        {
            get
            {
                return _client != null;
            }
        }

        protected void OnLoginChanged()
        {
            // notify the subscribed handlers
            if (LoginChanged != null)
                LoginChanged(this, EventArgs.Empty);
        }

        public event EventHandler LoginChanged;

        public async Task<MemoryActivity> DownloadAsync(IActivitySummary activity)
        {
            if (_client == null)
                return null;

            var pptActivity = activity as PPTActivitySummary;
            if (pptActivity == null)
                return null;

            var exercise = pptActivity.Exercise; // PolarPersonalTrainerLib model
            GpxExercise gpsData = null;
            try
            {
                gpsData = await _client.GetGpsData(exercise);
            }
            catch (PPTException)
            {
                Log.Diagnostics("No GPS data found for activity {0}.", activity.Name);
            }

            return await Task.Run<MemoryActivity>(() =>
                {
                    var samples = new[]
                    {
                        exercise.HeartRate.Values.Count,
                        exercise.CadenceValues.Count,
                        exercise.SpeedValues.Count
                    };
                    var samplesCount = samples.Max(); // number of samples
                    var result = new MemoryActivity();
                    DateTime timestamp = exercise.StartTime;
                    // process samples
                    for (int i = 0; i < samplesCount; i++)
                    {
                        var timeFrame = new ActivityTimeFrame();
                        timeFrame.Timestamp = timestamp;
                        if (exercise.HeartRate.Values.Count > i)
                            timeFrame.HeartRate = exercise.HeartRate.Values[i];
                        if (exercise.CadenceValues.Count > i)
                            timeFrame.Cadence = exercise.CadenceValues[i];
                        if (exercise.SpeedValues.Count > i)
                            timeFrame.Speed = new Speed(exercise.SpeedValues[i], SpeedUnit.KilometerPerHour);
                        if (gpsData != null)
                        {
                            var trackPoint = gpsData.Track.GetTrackPoint(i);
                            if (trackPoint != null)
                            {
                                timeFrame.Position = new Position(Convert.ToSingle(trackPoint.Longitude),
                                    Convert.ToSingle(trackPoint.Latitude),
                                    Convert.ToSingle(trackPoint.Elevation));
                            }
                        }

                        result.AddTimeFrame(timeFrame);
                        timestamp = timestamp.AddSeconds(exercise.RecordingRate); // time of next sample
                    }
                    var summary = ActivitySummary.FromActivity(pptActivity);
                    result.SetSummary(summary);
                    return result;
                });
        }

        public Task DeleteAsync(IActivitySummary activity)
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<IActivitySummary>> GetMoreActivitiesAsync(int lastVisibleIndex)
        {
            throw new NotImplementedException();
        }

        public Task<IActivitySummary> GetActivityAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UploadStatus> UploadAsync(Stream source, UploadOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<UploadStatus> CheckUploadAsync(UploadStatus lastStatus)
        {
            throw new NotImplementedException();
        }

        public UploadOptions CreateUploadOptions()
        {
            throw new NotImplementedException();
        }

        public bool CanUpload
        {
            get { return false; }
        }

        public bool CanDownload
        {
            get { return true; }
        }

        public bool CanDelete
        {
            get { return false; }
        }

        /// <summary>
        /// User profile for PolarPersonalTrainer
        /// </summary>
        private class PolarUserProfile : IUserProfile
        {
            PPTUser _user;

            public PolarUserProfile(PPTUser user)
            {
                _user = user ?? throw new ArgumentNullException("user");
            }

            public string Name
            {
                get { return string.Format("{0} {1}", _user.FirstName, _user.LastName); }
            }

            public string Description
            {
                get { return _user.Nickname; }
            }

            public bool IsAuthenticated
            {
                get { return true; }
            }

            public string ProfileImageUrl
            {
                get { return null; }
            }
        }
    }

    public abstract class PPTTrackerAuthenticationBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
