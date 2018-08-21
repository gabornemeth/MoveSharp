//
// GpsBase.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using MoveSharp.Geolocation;
using System;
using System.Threading.Tasks;
using SharpGeo;
using XTools.Diagnostics;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// GPS functionality
    /// This is a platform independent abstraction
    /// </summary>
    public abstract class GpsBase : IGps
    {
        private IGeolocator _geolocator;

        /// <summary>
        /// Geolocator instance
        /// </summary>
        public IGeolocator Geolocator
        {
            get
            {
                return _geolocator;
            }
        }

        public MoveCalculator MoveCalculator
        {
            get;
            private set;
        }

        public GpsBase(IGeolocator geolocator)
        {
            Name = "GPS";
            _geolocator = geolocator;
            Id = "GPS";
            MoveCalculator = new MoveCalculator();
        }

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public async Task ConnectAsync()
        {
            if (!IsConnected)
            {
                try
                {
                    await _geolocator.ConnectAsync();
                    _geolocator.StatusChanged += geolocator_StatusChanged;
                    _isConnected = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                _geolocator.Disconnect();
                _geolocator.StatusChanged -= geolocator_StatusChanged;
                _isConnected = false;
                Status = null;
            }
        }

        protected abstract void RunOnUiThread(Action action);

        void geolocator_StatusChanged(object sender, GeoLocatorStatusChangedEventArgs e)
        {
            RunOnUiThread(() =>
                {
                    var now = DateTime.Now;
                    var elapsedTime = TimeSpan.FromSeconds(0);
                    if (Status != null)
                    {
                        elapsedTime = e.Status.Timestamp.Subtract(_status.Timestamp);
                    }

                    Status = e.Status;
                });
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

        private GeolocatorStatus _status;
        public GeolocatorStatus Status
        {
            get
            {
                return _status;
            }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke(this, new SensorStateEventArgs(_status));
                }
            }
        }

        public object Data
        {
            get { return _status; }
        }
    }
}
