using System;

namespace MoveSharp.Models
{
    /// <summary>
    /// Activity interface
    /// </summary>
    public interface IActivity : ILap, IActivitySummary
    {
        ///// <summary>
        ///// Each frame of the activity
        ///// </summary>
        //IList<ActivityTimeFrame> TimeFrames { get; }

        //event EventHandler Modified;
        /// <summary>
        /// Starts a new lap
        /// </summary>
        ILapSummary NewLap();
    }
}
