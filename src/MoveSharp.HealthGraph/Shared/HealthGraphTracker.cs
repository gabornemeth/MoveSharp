using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HealthGraphNet;
using HealthGraphNet.Models;
using MoveSharp.Extensions;
using MoveSharp.Models;
using SharpGeo;
using MoveSharp.Authentication;
using MoveSharp.Format;
using System.Linq;

namespace MoveSharp.HealthGraph
{
    /// <summary>
    /// Runkeeper tracker
    /// </summary>
    public class HealthGraphTracker : OAuth2Tracker
    {
        private const int PageSize = 20;
        private Client _client;
        private UsersModel _user;

        public HealthGraphTracker(IOAuth2Authenticator authenticator)
        {
            Authenticator = authenticator;
            _client = new Client(Authenticator);
        }

        private async Task<UsersModel> GetUser()
        {
            if (_user != null)
                return _user;

            _user = await new UsersEndpoint(_client).GetUser();
            return _user;
        }

        public override bool CanDelete
        {
            get
            {
                return true;
            }
        }

        public override bool CanDownload
        {
            get
            {
                return true;
            }
        }

        public override bool CanUpload
        {
            get
            {
                return false;
            }
        }


        public override string Name
        {
            get
            {
                return "Runkeeper";
            }
        }

        private MemoryActivity ParseActivity(FitnessActivitiesPastModel healthGraphActivity)
        {
            var activityDetails = new MemoryActivity();
            activityDetails.StartTime = healthGraphActivity.StartTime;
            activityDetails.Sport = healthGraphActivity.Type.ToFitSport();

            // TODO: watch for other samples too!
            var numberOfTimeframes = healthGraphActivity.Path.Count;

            for (var i = 0; i < numberOfTimeframes; i++)
            {
                var timeFrame = new ActivityTimeFrame();
                timeFrame.Timestamp = activityDetails.StartTime.AddSeconds(healthGraphActivity.Distance[i].Timestamp);
                timeFrame.Position = new Position((float)healthGraphActivity.Path[i].Longitude, (float)healthGraphActivity.Path[i].Latitude,
                    (float)healthGraphActivity.Path[i].Altitude);
                if (healthGraphActivity.HeartRate.Count > i)
                    timeFrame.HeartRate = (byte)healthGraphActivity.HeartRate[i].HeartRate;

                activityDetails.AddTimeFrame(timeFrame);
            }

            return activityDetails;
        }

        public override async Task DeleteAsync(IActivitySummary activity)
        {
            var healthGraphActivity = activity as HealthGraphActivitySummary;
            if (healthGraphActivity == null)
                return;

            var activitiesEndpoint = new FitnessActivitiesEndpoint(_client, await GetUser());
            await activitiesEndpoint.DeleteActivity(healthGraphActivity.HealthGraphActivity.Uri);
        }

        public override async Task<MemoryActivity> DownloadAsync(IActivitySummary activity)
        {
            var healthGraphActivitySummary = activity as HealthGraphActivitySummary;
            if (healthGraphActivitySummary == null)
                return null;

            var healthGraphActivity = await _client.FitnessActivities.GetActivity(healthGraphActivitySummary.HealthGraphActivity.Uri);
            return ParseActivity(healthGraphActivity);
        }

        public override async Task<ActivityListResult> GetActivitiesAsync(object lastPageToken)
        {
            var lastIndex = Convert.ToInt32(lastPageToken);
            var activities = new List<IActivitySummary>();

            // Fetch activities from HealthGraph
            //var activitiesEndpoint = new FitnessActivitiesEndpoint(_client, await GetUser());
            var feed = await _client.FitnessActivities.GetFeedPage(lastIndex == 0 ? 0 : (lastIndex + 1) / PageSize + 1, PageSize);
            foreach (var feedItem in feed.Items)
            {
                var activitySummary = new HealthGraphActivitySummary(feedItem);
                activities.Add(activitySummary);
            }

            return new ActivityListResult
            {
                Activities = activities,
                NextPageToken = lastIndex + PageSize
            };
        }

        public override async Task<IActivitySummary> GetActivityAsync(string id)
        {
            var activity = await _client.FitnessActivities.GetActivity(id);
            return new HealthGraphActivitySummary(activity);
            //return ParseActivity(activity);
        }

        public override async Task<IUserProfile> GetUserAsync()
        {
            var profile = await _client.Profile.GetProfile();
            return new UserProfile(profile);
        }

        public override async Task<UploadStatus> UploadAsync(Stream source, UploadOptions options)
        {
            // TODO: UploadAsync should use MemoryActivity as a parameter instead of Stream
            var activity = new MemoryActivity();
            var fitImporter = new FitImporter(activity);
            await fitImporter.LoadAsync(source);

            var activityToCreate = new FitnessActivitiesNewModel
            {
                StartTime = activity.StartTime,
                AverageHeartRate = activity.AvgHeartRate,
                HeartRate = new List<HeartRateModel>(),
                Path = new List<PathModel>()
            };

            foreach (var timeFrame in activity.TimeFrames)
            {
                var timestamp = timeFrame.Timestamp.Subtract(activity.StartTime).TotalSeconds;
                var location = timeFrame.Position.GetValueOrDefault(Position.Empty);

                activityToCreate.HeartRate.Add(new HeartRateModel
                {
                    HeartRate = timeFrame.HeartRate.GetValueOrDefault(),
                    Timestamp = timestamp
                });
                activityToCreate.Path.Add(new PathModel
                {
                    Timestamp = timestamp,
                    Altitude = location.Altitude,
                    Longitude = location.Longitude,
                    Latitude = location.Latitude,
                    Type = "gps"
                });
            }

            var result = await _client.FitnessActivities.CreateActivity(activityToCreate);
            return new HealthGraphUploadStatus { Status = result };
        }

        public override Task<UploadStatus> CheckUploadAsync(UploadStatus lastStatus)
        {
            throw new NotImplementedException();
        }

        public override UploadOptions CreateUploadOptions()
        {
            throw new NotImplementedException();
        }

        public override bool IsPagingSupported
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// HealthGraph user profile
        /// </summary>
        private class UserProfile : IUserProfile
        {
            private ProfileModel _profile;

            public UserProfile(ProfileModel profile)
            {
                _profile = profile;
            }

            public string Description
            {
                get
                {
                    return _profile.Location;
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return _profile.Name;
                }
            }

            public string ProfileImageUrl
            {
                get
                {
                    return _profile.MediumPicture;
                }
            }
        }
    }
}
