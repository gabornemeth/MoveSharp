using MoveSharp.Format;
using MoveSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGeo;

namespace MoveSharp.Geolocation
{
    /// <summary>
    /// Geolocator simulator, that works from a GPX route file
    /// </summary>
    public class GpxSimulator : IGeolocator
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
                var importer = new GpxImporter(activity);
                importer.Load(source);
                // mock positions from the activity
                while (true)
                {
                    foreach (var frame in activity.TimeFrames)
                    {
                        Task.Delay(millisecondsDelay).Wait(); // wait for 1 second
                        if (!_isConnected)
                            continue;

                        if (frame.Position.HasValue)
                        {
                            if (StatusChanged != null)
                            {
                                _status.Position = frame.Position.Value;
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
                    return new GeolocatorStatus { Position = _status.Position };
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
