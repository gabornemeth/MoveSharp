using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Connection state of the sensor
    /// </summary>
    public enum SensorConnectionState
    {
        Connected,
        Disconnected,
        ConnectionLost,
        Disconnecting
    }

    /// <summary>
    /// Event arguments for connection state change
    /// </summary>
    public class SensorConnectionStateEventArgs : EventArgs
    {
        public SensorConnectionState ConnectionState { get; private set; }

        public SensorConnectionStateEventArgs(SensorConnectionState state)
        {
            ConnectionState = state;
        }
    }

    /// <summary>
    /// Event arguments for state change
    /// </summary>
    public class SensorStateEventArgs : EventArgs
    {
        public object Data { get; set; }
        public string Status { get; set; }

        public SensorStateEventArgs(object data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// ISensor attached to the smartphone
    /// Like GPS, Heart rate monitor, power meter, cadence/speed sensor, footpod
    /// </summary>
    public interface ISensor
    {
        /// <summary>
        /// Identifier of the sensor
        /// </summary>
        string Id
        {
            get;
        }

        string Name
        {
            get;
        }

        //bool IsEnabled { get; set; }

        event EventHandler<SensorConnectionStateEventArgs> ConnectionChanged;
        event EventHandler<SensorStateEventArgs> StatusChanged;

        [JsonIgnore]
        bool IsConnected { get; }
        [JsonIgnore]
        object Data { get; }

        //protected void OnConnectionChanged(SensorConnectionState state)
        //{
        //    if (ConnectionChanged != null)
        //        ConnectionChanged(this, new SensorConnectionStateEventArgs(state));
        //}

        //protected void OnStatusChanged(SensorStateEventArgs e)
        //{
        //    if (StatusChanged != null)
        //        StatusChanged(this, e);
        //}

        Task ConnectAsync();
        void Disconnect();
    }
}
