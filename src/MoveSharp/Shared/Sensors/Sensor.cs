//
// Sensor.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Threading.Tasks;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Wrapper class for <see cref="ISensor"/>
    /// This is mainly a facade.
    /// This way we don't restrict subclassing. (e.g. for Blueotooth and ANT+ sensor implementations)
    /// </summary>
    public class Sensor : ISensor
    {
        private ISensor _sensor;
        private ISensorService _sensorService;

        public Sensor(ISensor sensor, ISensorService sensorService)
        {
            _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
            _sensorService = sensorService ?? throw new ArgumentNullException(nameof(sensorService));
            _sensor.ConnectionChanged += sensor_ConnectionChanged;
            _sensor.StatusChanged += sensor_StatusChanged;
        }

        private void sensor_StatusChanged(object sender, SensorStateEventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(sender, e);
        }

        private void sensor_ConnectionChanged(object sender, SensorConnectionStateEventArgs e)
        {
            if (e.ConnectionState == SensorConnectionState.ConnectionLost)
            {
                // Connection lost
                if (IsEnabled && _sensorService.IsConnected)
                {
                    // Trying to reconnect, if the sensor is enabled
                    Task.Run(async () =>
                    {
                        while (!IsConnected)
                        {
                            await ConnectAsync();
                            await Task.Delay(5000);
                        }
                        ;
                    });
                }
            }

            // forward event
            if (ConnectionChanged != null)
                ConnectionChanged(sender, e);
        }

        public void Cleanup()
        {
            _sensor.ConnectionChanged -= sensor_ConnectionChanged;
            _sensor.StatusChanged -= sensor_StatusChanged;
        }

        public ISensor Implementation
        {
            get { return _sensor; }
        }

        public event EventHandler EnabledChanged;
        private bool _isEnabled;
        /// <summary>
        /// Whether the sensor is enabled (use it if available)
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    if (_sensorService.IsConnected)
                    {
                        if (_isEnabled)
                            _sensor.ConnectAsync();
                        else
                            _sensor.Disconnect();
                    }
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #region ISensor implementation

        public string Id
        {
            get { return _sensor.Id; }
        }

        public string Name
        {
            get { return _sensor.Name; }
        }

        public bool IsConnected
        {
            get { return _sensor.IsConnected; }
        }

        public async Task ConnectAsync()
        {
            await _sensor.ConnectAsync();
        }

        public void Disconnect()
        {
            _sensor.Disconnect();
        }

        public event EventHandler<SensorConnectionStateEventArgs> ConnectionChanged;

        public event EventHandler<SensorStateEventArgs> StatusChanged;

        public object Data
        {
            get { return _sensor.Data; }
        }

        #endregion
    }
}
