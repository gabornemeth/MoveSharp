using System;
using System.Threading.Tasks;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// ISensor manager
    /// </summary>
    public interface ISensorService
    {
        event EventHandler<Sensor> SensorDetected;
        SensorCollection Sensors { get; }
        event EventHandler ScanCompleted;
        event EventHandler ScanStarted;
        bool IsScanning { get; }
        Task ScanAsync(TimeSpan timeout);

        /// <summary>
        /// GPS
        /// </summary>
        IGps Gps { get; }

        /// <summary>
        /// Primary heart rate monitor.
        /// </summary>
        IHeartRateMonitor HeartRateMonitor { get; }

        /// <summary>
        /// Primary speed sensor.
        /// </summary>
        ISpeedSensor SpeedSensor { get; }

        /// <summary>
        /// Primary cadence sensor.
        /// </summary>
        ICadenceSensor CadenceSensor { get; }

        /// <summary>
        /// Primary bike power meter.
        /// </summary>
        IBikePowerMeter BikePowerMeter { get; }

        /// <summary>
        /// System clock
        /// </summary>
        Clock Clock { get; }

        Task ConnectAsync();
        void Disconnect();
        bool IsConnected { get; }

        void LoadState();
    }
}
