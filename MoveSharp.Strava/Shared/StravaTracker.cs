//
// StravaTracker.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MoveSharp.Extensions;
using MoveSharp.Helpers;
using XTools.Diagnostics;
using Newtonsoft.Json.Linq;
using StravaSharp;
using MoveSharp.Models;
using Newtonsoft.Json;

namespace MoveSharp.Strava
{
    /// <summary>
    /// Strava tracker
    /// </summary>
    public class StravaTracker : ITracker
    {
        private Client _client;
        private Authentication.IOAuth2Authenticator _auth;

        public Client Client
        {
            get
            {
                return _client;
            }
        }

        public StravaTracker(Authentication.IOAuth2Authenticator authenticator)
        {
            _auth = authenticator;
            _client = new Client(authenticator);
        }

        public string Name
        {
            get { return "Strava"; }
        }

        public async Task<IEnumerable<IActivitySummary>> GetActivitiesAsync(DateTime startTime, DateTime endTime)
        {
            var activities = new List<IActivitySummary>();
            // Fetch activities from Strava
            var stravaActivities = await _client.Activities.GetAthleteActivities(endTime, startTime);
            await ParseResults(stravaActivities, activities);

            return activities;
        }

        private const int PageSize = 20;

        public async Task<ActivityListResult> GetActivitiesAsync(object lastPageToken)
        {
            var lastIndex = Convert.ToInt32(lastPageToken);
            var activities = new List<IActivitySummary>();
            // Fetch activities from Strava
            var stravaActivities = await _client.Activities.GetAthleteActivities((lastIndex + 1) / PageSize + 1, PageSize);
            await ParseResults(stravaActivities, activities);

            return new ActivityListResult
            {
                Activities = activities,
                NextPageToken = lastIndex + PageSize
            };
        }

        public async Task<IActivitySummary> GetActivityAsync(string id)
        {
            var stravaActivity = await _client.Activities.Get(Convert.ToInt64(id));
            return await ParseResult(stravaActivity);
        }

        private async Task<IActivitySummary> ParseResult(StravaSharp.ActivitySummary stravaActivity)
        {
            var summary = new StravaActivitySummary(stravaActivity);
            await summary.GetPropertiesAsync();
            return summary;
        }

        private async Task ParseResults(IEnumerable<StravaSharp.ActivitySummary> stravaActivities, IList<IActivitySummary> activities)
        {
            foreach (var stravaActivity in stravaActivities)
            {
                var summary = await ParseResult(stravaActivity);
                activities.Add(summary);
            }
        }

        public async Task<IUserProfile> GetUserAsync()
        {
            var athlete = await _client.Athletes.GetCurrent();
            return new StravaUserProfile(athlete);
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
                        _auth.AccessToken = _credentials.Value<string>("AccessToken");
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
            if (_auth.IsAuthenticated)
                IsLoggedIn = true;
            else
                await _auth.Authenticate();
        }

        public void Logout()
        {
            if (_auth.IsAuthenticated)
            {
                // forget access token
                _auth.AccessToken = null;
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

        public event EventHandler LoginChanged;

        StravaUploadStatus CreateUploadStatus(StravaSharp.UploadStatus status)
        {
            return new StravaUploadStatus
            {
                Id = status.Id,
                ExternalId = status.ExternalId,
                ActivityId = status.ActivityId,
                Status = status.Status,
            };
        }

        public async Task<Models.UploadStatus> CheckUploadAsync(Models.UploadStatus lastStatus)
        {
            var lastStravaStatus = lastStatus as StravaUploadStatus;
            var currentStatus = await _client.Activities.GetUploadStatus(lastStravaStatus.Id);
            return CreateUploadStatus(currentStatus);
        }

        public async Task<Models.UploadStatus> UploadAsync(System.IO.Stream source, UploadOptions options)
        {
            var dataFormat = StravaExtensions.GetStravaDataType(options.FileName);
            var stravaOptions = (StravaUploadOptions)options;
            
            var result = await _client.Activities.Upload(options.Sport.ToStravaActivityType(), dataFormat, source, options.FileName,
                stravaOptions.Name, stravaOptions.Description, stravaOptions.IsPrivate, stravaOptions.IsCommute);

            return CreateUploadStatus(result);
        }

        public async Task<MemoryActivity> DownloadAsync(IActivitySummary activity)
        {
            var stravaActivity = activity as StravaActivitySummary;
            if (stravaActivity != null) // only Strava activity can be downloaded
            {
                var streams = await _client.Activities.GetActivityStreams(stravaActivity.Activity, StreamType.Time, StreamType.LatLng, StreamType.Watts, StreamType.HeartRate, StreamType.Cadence, StreamType.VelocitySmooth, StreamType.Altitude, StreamType.Moving);
                Debug.WriteLine("Memory allocated for downlading: " + FileHelper.GetSizeString(GC.GetTotalMemory(false)));

                // determine what streams we have
                StravaSharp.Stream timeStream = null, posStream = null, hrStream = null, speedStream = null, powerStream = null, cadenceStream = null, altitudeStream = null, movingStream = null;
                foreach (var stream in streams)
                {
                    if (stream.Type == StreamType.Time)
                        timeStream = stream;
                    else if (stream.Type == StreamType.HeartRate)
                        hrStream = stream;
                    else if (stream.Type == StreamType.LatLng)
                        posStream = stream;
                    else if (stream.Type == StreamType.VelocitySmooth)
                        speedStream = stream;
                    else if (stream.Type == StreamType.Cadence)
                        cadenceStream = stream;
                    else if (stream.Type == StreamType.Watts)
                        powerStream = stream;
                    else if (stream.Type == StreamType.Altitude)
                        altitudeStream = stream;
                    else if (stream.Type == StreamType.Moving)
                        movingStream = stream;
                    Log.Diagnostics("{0} stream length: {1}", stream.Type, stream.Data.Length);
                }

                return await Task.Run<MemoryActivity>(() =>
                    {
                        // build activity from the streams' frames
                        // pauses are not recognized this way

                        var isMoving = false;
                        var destActivity = new MemoryActivity { Sport = stravaActivity.Sport, Name = stravaActivity.Name };
                        DateTime startDate = stravaActivity.Activity.StartDate;
                        for (int i = 0; i < streams[0].Data.Length; i++)
                        {
                            var timeFrame = new ActivityTimeFrame();
                            timeFrame.Timestamp = startDate.AddSeconds(Convert.ToInt32(timeStream.Data[i]));

                            var moving = Convert.ToBoolean(movingStream.Data[i]);
                            if (isMoving && !moving)
                            {
                                // now stops
                                timeFrame.Type = ActivityTimeFrameType.Stop;
                                destActivity.AddTimeFrame(timeFrame);
                                isMoving = false;
                                continue;
                            }
                            else if (!isMoving && moving)
                            {
                                // now starts moving again
                                timeFrame.Type = ActivityTimeFrameType.Start;
                                destActivity.AddTimeFrame(timeFrame);
                                isMoving = true;
                            }
                            else if (!isMoving && !moving)
                                continue; // still standing

                            if (hrStream != null)
                                timeFrame.HeartRate = Convert.ToByte(hrStream.Data[i]);
                            if (posStream != null)
                            {
                                // position is json array
                                var pos = posStream.Data[i] as JArray;
                                if (pos != null)
                                {
                                    var p = new SharpGeo.Position();
                                    p.Latitude = pos[0].ToObject<float>();
                                    p.Longitude = pos[1].ToObject<float>();
                                    if (altitudeStream != null)
                                        p.Altitude = Convert.ToSingle(altitudeStream.Data[i]);
                                    timeFrame.Position = p;
                                }
                            }
                            if (speedStream != null)
                            {
                                timeFrame.Speed = new Speed(Convert.ToSingle(speedStream.Data[i]), SpeedUnit.MeterPerSecond);
                            }
                            if (cadenceStream != null)
                            {
                                timeFrame.Cadence = Convert.ToByte(cadenceStream.Data[i]);
                            }
                            if (powerStream != null)
                            {
                                timeFrame.Power = Convert.ToUInt16(powerStream.Data[i]);
                            }
                            destActivity.AddTimeFrame(timeFrame);
                        }
                        var summary = new Models.ActivitySummary();
                        summary.ElapsedTime = stravaActivity.ElapsedTime;
                        summary.MovingTime = stravaActivity.MovingTime;
                        summary.Sport = stravaActivity.Sport;
                        summary.AvgSpeed = stravaActivity.AvgSpeed;
                        summary.MaxSpeed = stravaActivity.MaxSpeed;
                        summary.AvgHeartRate = stravaActivity.AvgHeartRate;
                        summary.MaxHeartRate = stravaActivity.MaxHeartRate;
                        summary.AvgCadence = stravaActivity.AvgCadence;
                        summary.MaxCadence = stravaActivity.MaxCadence;
                        if (stravaActivity.HasRealPowerData)
                        {
                            summary.AvgPower = stravaActivity.AvgPower;
                            summary.MaxPower = stravaActivity.MaxPower;
                        }
                        summary.Ascent = stravaActivity.Ascent;
                        summary.Descent = stravaActivity.Descent;
                        destActivity.SetSummary(summary);
                        return destActivity;
                    });
            }

            return null;
        }

        public async Task DeleteAsync(IActivitySummary activity)
        {
            var stravaActivity = activity as StravaActivitySummary;
            if (stravaActivity != null)
            {
                await _client.Activities.Delete(stravaActivity.Activity.Id);
            }
        }

        public bool CanUpload
        {
            get { return true; }
        }

        public bool CanDownload
        {
            get { return true; }
        }

        public bool CanDelete
        {
            get { return true; }
        }

        public bool IsPagingSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Strava athlete info
        /// </summary>
        private class StravaUserProfile : IUserProfile
        {
            private Athlete _athlete;

            public StravaUserProfile(Athlete athlete)
            {
                _athlete = athlete;
            }

            public string Name
            {
                get
                {
                    return string.Format("{0} {1}", _athlete.FirstName, _athlete.LastName);
                }
            }

            public string Description
            {
                get
                {
                    return string.Format("{0}, {1}, {2}", _athlete.City, _athlete.State, _athlete.Country);
                }
            }

            public bool IsAuthenticated
            {
                get { return true; }
            }

            public string ProfileImageUrl
            {
                get
                {
                    if ( _athlete.ProfileMedium == "avatar/athlete/medium.png")
                        return null; // no image has been specified by the user

                    return _athlete.ProfileMedium;
                }
            }
        }

        public UploadOptions CreateUploadOptions()
        {
            return new StravaUploadOptions();
        }
    }
}
