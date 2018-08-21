using System;

namespace MoveSharp.Models
{
    public enum ActivityTimeFrameType
    {
        Active,
        Stop,
        Start
    }

    /// <summary>
    /// Activity's state in a time at the moment
    /// </summary>
    public class ActivityTimeFrame
    {
        public ActivityTimeFrameType Type { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Geographic position
        /// </summary>
        public SharpGeo.Position? Position { get; set; }
        /// <summary>
        /// Gets a value indicating whether this frame has a valid position
        /// </summary>
        /// <value><c>true</c> if this instance has position; otherwise, <c>false</c>.</value>
        public bool HasPosition
        {
            get
            {
                return Position.HasValue && Position.Value.IsEmpty == false;
            }
        }

        /// <summary>
        /// Heart rate [bpm]
        /// </summary>
        public byte? HeartRate { get; set; }

        /// <summary>
        /// Cycling power [watts]
        /// </summary>
        public ushort? Power { get; set; }

        /// <summary>
        /// Cycling cadence [rpm]
        /// </summary>
        public byte? Cadence { get; set; }

        /// <summary>
        /// Speed [m/s]
        /// </summary>
        public Speed? Speed { get; set; }

        /// <summary>
        /// Heart rate R-R values
        /// </summary>
        public int[] RRValues { get; set; }

        /// <summary>
        /// Whether contains R-R values
        /// </summary>
        public bool HasRRValues
        {
            get
            {
                return RRValues != null && RRValues.Length > 0;
            }
        }

        /// <summary>
        /// Distance (summary since the beginning)
        /// </summary>
        public Distance? Distance { get; set; }

        /// <summary>
        /// Altitude
        /// </summary>
        public Distance? Altitude { get; set; }

        public ActivityTimeFrame()
        {
            Type = ActivityTimeFrameType.Active;
        }

        public ActivityTimeFrame Clone()
        {
            var frame = new ActivityTimeFrame
            {
                Type = Type,
                Timestamp = Timestamp,
                Position = Position,
                Distance = Distance,
                Altitude = Altitude,
                HeartRate = HeartRate,
                RRValues = RRValues,
                Power = Power,
                Cadence = Cadence,
                Speed = Speed,
            };

            return frame;
        }
    }
}
