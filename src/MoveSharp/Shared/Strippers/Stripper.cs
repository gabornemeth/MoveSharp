using System;
using System.IO;

namespace MoveSharp.Strippers
{
    /// <summary>
    /// Strip options
    /// </summary>
    [Flags]
    public enum StripOptions
    {
        None = 0,
        /// <summary>
        /// Remove heart rate info
        /// </summary>
        HeartRate = 1,
        /// <summary>
        /// Remove power
        /// </summary>
        Power = 2,
        /// <summary>
        /// Remove cadence
        /// </summary>
        Cadence = 4
    }

    public interface IStripper
    {
        void Strip(Stream input, Stream output, StripOptions options);
    }
}
