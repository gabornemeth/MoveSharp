//
// Clock.cs
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
    /// Clock as a sensor
    /// </summary>
    public class Clock : ISensor
    {
        private Timer _timer;
        private bool _isConnected;

        public Clock()
        {
            _timer = new Timer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += timer_Tick;
            _isConnected = false;
            Id = "Sytem_Clock";
            Name = "System clock";
        }

        private void timer_Tick(object sender, DateTime dateTime)
        {
            Data = dateTime;
            if (StatusChanged != null)
                StatusChanged(this, new SensorStateEventArgs(Data));
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public async Task ConnectAsync()
        {
            _timer.Start();
            _isConnected = true;
            if (ConnectionChanged != null)
                ConnectionChanged(this, new SensorConnectionStateEventArgs(SensorConnectionState.Connected));
        }

        public void Disconnect()
        {
            _timer.Stop();
            _isConnected = false;
            if (ConnectionChanged != null)
                ConnectionChanged(this, new SensorConnectionStateEventArgs(SensorConnectionState.Disconnected));
        }

        public string Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public event EventHandler<SensorConnectionStateEventArgs> ConnectionChanged;

        public event EventHandler<SensorStateEventArgs> StatusChanged;

        public object Data
        {
            get;
            private set;
        }
    }
}
