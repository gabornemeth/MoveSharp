using System;

namespace MoveSharp.Models
{
    /// <summary>
    /// Summary information about the activity
    /// Can be easily serialized
    /// </summary>
    public class ActivitySummary : LapSummary, IActivitySummary
    {
        public virtual Dynastream.Fit.Sport Sport
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual System.Threading.Tasks.Task GetPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        public static ActivitySummary FromActivity(IActivitySummary activity)
        {
            var summary = new ActivitySummary();
            summary.CopyFrom(activity);
            return summary;
        }

        public virtual void CopyFrom(IActivitySummary source)
        {
            Name = source.Name;
            StartTime = source.StartTime;
            Sport = source.Sport;
            Distance = source.Distance;
            AvgSpeed = source.AvgSpeed;
            MaxSpeed = source.MaxSpeed;
            AvgHeartRate = source.AvgHeartRate;
            MaxHeartRate = source.MaxHeartRate;
            AvgPower = source.AvgPower;
            MaxPower = source.MaxPower;
            AvgCadence = source.AvgCadence;
            MaxCadence = source.MaxCadence;
            ElapsedTime = source.ElapsedTime;
            MovingTime = source.MovingTime;
            Ascent = source.Ascent;
            Descent = source.Descent;
        }

        private LapSummaryCollection _laps = new LapSummaryCollection();
        public LapSummaryCollection Laps
        {
            get { return _laps; }
        }
    }
}
