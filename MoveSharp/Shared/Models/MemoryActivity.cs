//
// MemoryActivity.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    /// <summary>
    /// Activity for in-memory use
    /// This type of activity can be used for analysis
    /// </summary>
    public class MemoryActivity : MemoryLap, IActivity
    {
        public Dynastream.Fit.Sport Sport
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Device { get; set; }

        public virtual Task GetPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resets the activity
        /// </summary>
        public override void Reset()
        {
            _laps.Clear();
            base.Reset();
        }

        public override void AddTimeFrame(ActivityTimeFrame timeFrame)
        {
            if (!timeFrame.Distance.HasValue)
            {
                // supply timeframe with the actual distance
                timeFrame.Distance = Distance;
            }

            // Add timeframe to the last lap
            var currentLap = (_laps.Current ?? NewLap()) as MemoryLap;
            if (currentLap != null)
                currentLap.AddTimeFrame(timeFrame);
            // Add timeframe
            base.AddTimeFrame(timeFrame);
        }

        /// <summary>
        /// Updates the total values with summary values
        /// </summary>
        /// <param name="summary"></param>
        public void SetSummary(IActivitySummary summary)
        {
            // update with valid values from summary
            base.SetSummary(summary);
            Sport = summary.Sport;
        }

        public void CopyFrom(IActivitySummary source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts a new lap
        /// </summary>
        public ILapSummary NewLap()
        {
            var lap = new MemoryLap();
            _laps.NewLap(lap);

            return lap;
        }

        public void RecalculateCurrentLap()
        {
            // readd timeframes to current lap after the end of last lap
            var currentLap = Laps.Current as MemoryLap;
            if (currentLap == null)
                return;

            currentLap.Reset();
            var timeStart = StartTime;
            foreach (var laps in Laps)
            {
                var lapEndTime = laps.StartTime.AddSeconds(laps.ElapsedTime);
                if (lapEndTime > timeStart)
                    timeStart = lapEndTime;
            }

            foreach (var timeFrame in TimeFrames.Where(timeFrame => timeFrame.Timestamp > timeStart))
            {
                currentLap.AddTimeFrame(timeFrame);
            }
        }

        private LapSummaryCollection _laps = new LapSummaryCollection();
        /// <summary>
        /// Collection of <see cref="ILapSummary"/>
        /// </summary>
        public LapSummaryCollection Laps
        {
            get { return _laps; }
        }

        /// <summary>
        /// Recodes the activity
        /// </summary>
        /// <returns></returns>
        public MemoryActivity Repair()
        {
            var activity = new MemoryActivity();
            foreach (var frame in TimeFrames)
            {
                activity.AddTimeFrame(frame);
            }

            return activity;
        }
    }
}
