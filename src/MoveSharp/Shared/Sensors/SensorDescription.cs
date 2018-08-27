using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Easily serializable representation of a sensor
    /// </summary>
    public class SensorDescription : ISensor
    {
        [JsonIgnore]
        public bool IsConnected
        {
            get { return false; }
        }

        public async Task ConnectAsync()
        {
            await Task.Run(() => { });
        }

        public void Disconnect()
        {
        }

        public SensorDescription(ISensor sensor)
        {
            if (sensor == null)
                throw new ArgumentNullException("sensor");
            Id = sensor.Id;
            Name = sensor.Name;
        }

        public SensorDescription()
        {
        }

        public string Id
        {
            get; set;
        }

        public string Name
        {
            get;
            set;
        }

        public event EventHandler<SensorConnectionStateEventArgs> ConnectionChanged;

        public event EventHandler<SensorStateEventArgs> StatusChanged;

        [JsonIgnore]
        public object Data
        {
            get; set;
        }

        public void OnConnectionChanged(SensorConnectionState state)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(SensorStateEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
