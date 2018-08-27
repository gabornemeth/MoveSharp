//
// HealthGraphActivitySummary.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using System;
using System.Threading.Tasks;
using Dynastream.Fit;
using HealthGraphNet.Models;
using MoveSharp.Extensions;
using MoveSharp.Models;

namespace MoveSharp.HealthGraph
{
    class HealthGraphActivitySummary : IActivitySummary<string>
    {
        internal FitnessActivitiesFeedItemModel HealthGraphActivity
        {
            get; private set;
        }

        public HealthGraphActivitySummary(FitnessActivitiesFeedItemModel activity)
        {
            HealthGraphActivity = activity;
            Laps = new LapSummaryCollection();
            Laps.Add(this);
        }
        public string Id
        {
            get
            {
                return HealthGraphActivity.Uri;
            }
        }


        public float Ascent
        {
            get
            {
                return 0;
            }
        }

        public int AvgCadence
        {
            get
            {
                return 0;
            }
        }

        public int AvgHeartRate
        {
            get
            {
                return 0;
            }
        }

        public int AvgPower
        {
            get
            {
                return 0;
            }
        }

        public Speed AvgSpeed
        {
            get
            {
                return Speed.Empty;
            }
        }

        public float Descent
        {
            get
            {
                return 0;
            }
        }

        public Distance Distance
        {
            get
            {
                return new Distance(Convert.ToSingle(HealthGraphActivity.TotalDistance), DistanceUnit.Meter);
            }
        }

        public int ElapsedTime
        {
            get
            {
                return Convert.ToInt32(HealthGraphActivity.Duration);
            }
        }

        public LapSummaryCollection Laps
        {
            get; private set;
        }

        public int MaxCadence
        {
            get
            {
                return 0;
            }
        }

        public int MaxHeartRate
        {
            get
            {
                return 0;
            }
        }

        public int MaxPower
        {
            get
            {
                return 0;
            }
        }

        public Speed MaxSpeed
        {
            get
            {
                return Speed.Empty;
            }
        }

        public int MovingTime
        {
            get
            {
                return Convert.ToInt32(HealthGraphActivity.Duration);
            }
        }

        public string Name
        {
            get
            {
                return string.Format("{0} {1}", HealthGraphActivity.StartTime.DayOfWeek, HealthGraphActivity.Type);
            }
        }

        public Sport Sport
        {
            get
            {
                return HealthGraphActivity.Type.ToFitSport();
            }
        }

        public System.DateTime StartTime
        {
            get
            {
                return HealthGraphActivity.StartTime;
            }
        }

        public void CopyFrom(IActivitySummary source)
        {
            throw new NotImplementedException();
        }

        public Task GetPropertiesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
