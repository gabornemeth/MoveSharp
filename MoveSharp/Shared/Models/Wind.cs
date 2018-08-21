namespace MoveSharp.Models
{
    /// <summary>
    /// Wind data
    /// </summary>
    public class Wind
    {
        /// <summary>
        /// Speed of the wind
        /// </summary>
        /// <value>The speed.</value>
        public double Speed { get; set; }
        /// <summary>
        /// Where the wind comes from.
        /// </summary>
        public double Degree { get; set; }
    }
}

