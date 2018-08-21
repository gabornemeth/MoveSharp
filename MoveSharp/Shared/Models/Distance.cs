
namespace MoveSharp.Models
{
    /// <summary>
    /// Unit of distance
    /// </summary>
    public enum DistanceUnit
    {
        Meter,
        Kilometer,
        Mile,
        Yard,
        Foot
    }

    /// <summary>
    /// Distance
    /// </summary>
    public struct Distance
    {
        public const float InvalidValue = -1.0f;

        public float Value { get; set; }
        public DistanceUnit Unit { get; set; }

        private static float[] conversion = new[] { 1.0f, 1000.0f, 1609.0f, 0.9144f, 0.3048f };

        /// <summary>
        /// Empty value
        /// </summary>
        public readonly static Distance Empty = new Distance();

        public bool HasValue
        {
            get
            {
                return Value != 0.0f;
            }
        }

        public Distance(float value, DistanceUnit unit) : this()
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Returns the value in the desired unit
        /// </summary>
        /// <param name="unit">the desired unit</param>
        /// <returns>value in <c>unit</c></returns>
        public float GetValueAs(DistanceUnit unit)
        {
            return Value * conversion[(int)Unit] / conversion[(int)unit];
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Distance))
                return Equals((Distance)obj);

            return false;
        }

        public bool Equals(Distance compareTo)
        {
            return Value == compareTo.GetValueAs(Unit);
        }

        public static bool operator ==(Distance a, Distance b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Distance a, Distance b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }
}
