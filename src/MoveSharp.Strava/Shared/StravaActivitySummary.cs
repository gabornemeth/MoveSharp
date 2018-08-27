using MoveSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoveSharp.Models;

namespace MoveSharp.Strava
{
    /// <summary>
    /// Strava activity summary
    /// </summary>
    public class StravaActivitySummary : IActivitySummary<long>
    {
        public StravaActivitySummary()
        {
            Laps = new LapSummaryCollection();
            Laps.Add(this);
        }

        public Distance Distance
        {
            get
            {
                return new Distance { Value = _activity.Distance, Unit = DistanceUnit.Meter };
            }
        }

        public int AvgHeartRate
        {
            get { return (int)_activity.AverageHeartRate; }
        }

        public int MaxHeartRate
        {
            get { return (int)_activity.MaxHeartRate; }
        }

        public int AvgPower
        {
            get { return (int)_activity.AveragePower; }
        }

        public int MaxPower
        {
            get { return _activity.MaxPower; }
        }

        public Speed AvgSpeed
        {
            get { return new Speed { Value = _activity.AverageSpeed, Unit = SpeedUnit.MeterPerSecond }; }
        }

        public Speed MaxSpeed
        {
            get { return new Speed { Value = _activity.MaxSpeed, Unit = SpeedUnit.MeterPerSecond }; }
        }

        public int ElapsedTime
        {
            get { return _activity.ElapsedTime; }
        }

        public int MovingTime
        {
            get { return _activity.MovingTime; }
        }

        private StravaSharp.ActivitySummary _activity;

        public StravaSharp.ActivitySummary Activity
        {
            get
            {
                return _activity;
            }
        }

        public StravaActivitySummary(StravaSharp.ActivitySummary activity)
        {
            _activity = activity;
        }

        public async Task GetPropertiesAsync()
        {
            // there is no need to do anything, all properties are known from ActivitySummary's members
            await Task.Run(() =>
                {
                }).ConfigureAwait(false);
        }

        public string Name
        {
            get { return _activity.Name; }
        }

        public Dynastream.Fit.Sport Sport
        {
            get { return _activity.Type.ToFitSport(); }
        }

        public int AvgCadence
        {
            get { return (int)_activity.AverageCadence; }
        }

        public int MaxCadence
        {
            get
            {
                return 0; // not supported by Strava API
            }
        }

        public DateTime StartTime
        {
            get { return _activity.StartDate; }
        }

        void IActivitySummary.CopyFrom(IActivitySummary source)
        {
            throw new NotSupportedException();
        }

        public LapSummaryCollection Laps { get; private set; }


        public float Ascent
        {
            get { return _activity.TotalElevationGain; }
        }

        public float Descent
        {
            get { return _activity.TotalElevationGain; }
        }

        /// <summary>
        /// Gets whether the activity contains real device power data.
        /// </summary>
        public bool HasRealPowerData
        {
            get
            {
                return _activity.DeviceWatts;
            }
        }

        public long Id
        {
            get
            {
                return _activity.Id;
            }
        }
    }
}
