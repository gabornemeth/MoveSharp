using StravaSharp;
using System.IO;
using System.Threading.Tasks;
using SharpGeo;

namespace MoveSharp.Extensions
{
    static class StravaExtensions
    {
        /// <summary>
        /// Converts Strava activity type to FIT activity type
        /// </summary>
        /// <param name="type">Strava activity type</param>
        /// <returns>The corresponding FIT activity type</returns>
        public static Dynastream.Fit.ActivityType ToFitType(this ActivityType type)
        {
            // TODO: support as much activity type as possible!
            switch (type)
            {
                case ActivityType.Ride:
                    return Dynastream.Fit.ActivityType.Cycling;
                case ActivityType.Run:
                    return Dynastream.Fit.ActivityType.Running;
                case ActivityType.Walk:
                    return Dynastream.Fit.ActivityType.Walking;
                default:
                    return Dynastream.Fit.ActivityType.Generic;
            }
        }

        public static Dynastream.Fit.Sport ToFitSport(this ActivityType type)
        {
            // TODO: support as much types as possible!
            switch (type)
            {
                case ActivityType.Ride:
                    return Dynastream.Fit.Sport.Cycling;
                case ActivityType.Run:
                    return Dynastream.Fit.Sport.Running;
                case ActivityType.Walk:
                    return Dynastream.Fit.Sport.Walking;
                case ActivityType.AlpineSki:
                    return Dynastream.Fit.Sport.AlpineSkiing;
                default:
                    return Dynastream.Fit.Sport.Generic;
            }
        }

        public static ActivityType ToStravaActivityType(this Dynastream.Fit.Sport sport)
        {
            // TODO: support as much types as possible!
            switch (sport)
            {
                case Dynastream.Fit.Sport.Cycling:
                    return ActivityType.Ride;
                case Dynastream.Fit.Sport.Running:
                    return ActivityType.Run;
                case Dynastream.Fit.Sport.Walking:
                    return ActivityType.Walk;
                default:
                    return ActivityType.Workout;
            }
        }

        public static DataType GetStravaDataType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension == ".fit" ? DataType.Fit : DataType.Tcx;
        }
    }
}
