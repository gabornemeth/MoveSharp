using MoveSharp.Format;
using MoveSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SharpGeo;

namespace MoveSharp.Geolocation
{
    /// <summary>
    /// Geolocator simulator, that works from a FIT activity file
    /// </summary>
    public class FitSimulator : IGeolocator
    {
        public event EventHandler<GeoLocatorStatusChangedEventArgs> StatusChanged;
        private GeolocatorStatus _status = new GeolocatorStatus();
        private bool _isConnected;
        private Task _task;
        
        protected void Start(Stream source, int millisecondsDelay)
        {
            if (_task != null)
                return;

            _task = Task.Run(() =>
            {
                // import activity from GPX
                var activity = new MemoryActivity();
                var importer = new FitImporter(activity);
                importer.Load(source);
                // mock positions from the activity
                var moveCalculator = new MoveCalculator();
                while (true)
                {
                    foreach (var frame in activity.TimeFrames)
                    {
                        Task.Delay(millisecondsDelay).Wait(); // wait for 1 second
                        if (!_isConnected)
                            continue;

                        if (frame.Position.HasValue)
                        {
                            moveCalculator.Add(frame.Position.Value, TimeSpan.FromSeconds(1));
                            if (StatusChanged != null)
                            {
                                _status.Position = frame.Position.Value;
                                _status.Speed = moveCalculator.CurrentSpeed;
                                var args = new GeoLocatorStatusChangedEventArgs(_status);
                                StatusChanged(this, args);
                            }
                        }
                    }
                }
            });
            _task.ConfigureAwait(false);
        }

        public GeolocatorStatus Status
        {
            get
            {
                lock (_status)
                {
                    return new GeolocatorStatus { Position = _status.Position, Speed = _status.Speed };
                }
            }
        }

        public virtual async Task ConnectAsync()
        {
            await Task.Run(() => { _isConnected = true; });
        }

        public virtual void Disconnect()
        {
            _isConnected = false;
        }


        public void SetMaxAgeLimit(int maxAgeSeconds)
        {
        }
    }
}
