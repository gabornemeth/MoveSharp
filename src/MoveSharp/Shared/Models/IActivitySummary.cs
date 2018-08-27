using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    /// <summary>
    /// Summary information about an activity
    /// </summary>
    public interface IActivitySummary : ILapSummary
    {
        /// <summary>
        /// Sport of the activity
        /// Note: FIT types are supported
        /// </summary>
        Dynastream.Fit.Sport Sport { get; }
        /// <summary>
        /// Name of the activity
        /// </summary>
        string Name { get; }

        LapSummaryCollection Laps { get; }

        /// <summary>
        /// Total Ascent
        /// </summary>
        //DistanceUnit Ascent { get; }
        /// <summary>
        /// Retrieving the activity's properties asyncrounously
        /// </summary>
        /// <returns></returns>
        Task GetPropertiesAsync();

        void CopyFrom(IActivitySummary source);
    }

    public interface IActivitySummary<T> : IActivitySummary
    {
        T Id { get; }
    }
}
