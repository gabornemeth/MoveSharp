using MoveSharp.Geolocation;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// GPS base class
    /// </summary>
    public interface IGps : ISensor
    {
        /// <summary>
        /// returns the Geolocator
        /// </summary>
        IGeolocator Geolocator { get; }
//        MoveCalculator MoveCalculator { get; }
    }
}
