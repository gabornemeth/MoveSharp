//
// HealthGraphExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using System.IO;
using System.Threading.Tasks;
using SharpGeo;
using HealthGraphNet.Models;

namespace MoveSharp.Extensions
{
    static class HealthGraphExtensions
    {
        ///// <summary>
        ///// Converts Strava activity type to FIT activity type
        ///// </summary>
        ///// <param name="type">Strava activity type</param>
        ///// <returns>The corresponding FIT activity type</returns>
        //public static Dynastream.Fit.ActivityType ToFitType(this FitnessActivityType type)
        //{
        //    // TODO: support as much activity type as possible!
        //    switch (type)
        //    {
        //        case FitnessActivityType.Cycling:
        //            return Dynastream.Fit.ActivityType.Cycling;
        //        case FitnessActivityType.Running:
        //            return Dynastream.Fit.ActivityType.Running;
        //        case FitnessActivityType.Walking:
        //            return Dynastream.Fit.ActivityType.Walking;
        //        default:
        //            return Dynastream.Fit.ActivityType.Generic;
        //    }
        //}

        public static Dynastream.Fit.Sport ToFitSport(this FitnessActivityType type)
        {
            // TODO: support as much types as possible!
            switch (type)
            {
                case FitnessActivityType.Cycling:
                    return Dynastream.Fit.Sport.Cycling;
                case FitnessActivityType.Running:
                    return Dynastream.Fit.Sport.Running;
                case FitnessActivityType.Walking:
                    return Dynastream.Fit.Sport.Walking;
                case FitnessActivityType.DownhillSkiing:
                    return Dynastream.Fit.Sport.AlpineSkiing;
                default:
                    return Dynastream.Fit.Sport.Generic;
            }
        }

        public static FitnessActivityType ToFitnessActivityType(this Dynastream.Fit.Sport sport)
        {
            // TODO: support as much types as possible!
            switch (sport)
            {
                case Dynastream.Fit.Sport.Cycling:
                    return FitnessActivityType.Cycling;
                case Dynastream.Fit.Sport.Running:
                    return FitnessActivityType.Running;
                case Dynastream.Fit.Sport.Walking:
                    return FitnessActivityType.Walking;
                default:
                    return FitnessActivityType.Other;
            }
        }
    }
}
