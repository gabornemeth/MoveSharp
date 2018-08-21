//
// ActivityRecorder.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Threading.Tasks;
using System.IO;
using MoveSharp.Extensions;
using MoveSharp.Format;
using MoveSharp.Storage;
using DateTime = System.DateTime;
using MoveSharp.Sensors;
using XTools.Diagnostics;

namespace MoveSharp.Models
{
    public class ActivityRecorder
    {
        private FitExporter _exporter;
        /// <summary>
        /// Temporary file name used during recording
        /// </summary>
        private const string TempFileName = "activity.rec";
        /// <summary>
        /// Destination stream
        /// </summary>
        private Stream _streamDest;
        private ILocalFile _fileDest;
        private RecordingActivity _activity;

        public event EventHandler<ActivityRecorderEventArgs> Started;
        public event EventHandler<ActivityRecorderEventArgs> Stopped;
        public event EventHandler<ActivityRecorderEventArgs> Finished;
        public event EventHandler Modified;

        private DateTime _lastRecordedTime;
        private bool _autoPaused;
        public bool AutoPause { get; set; }
        public Speed AutoPauseSpeedLimit { get; set; }

        public bool IsStarted { get; private set; }

        private bool _isActive;

        public event EventHandler AutoPaused;
        public event EventHandler AutoResumed;

        /// <summary>
        /// Gets or sets whether recording is in progress
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;

            }
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    _activity.IsRecording = value;
                }
            }
        }

        public RecordingActivity Activity { get { return _activity; } }

        private ISensorService SensorService
        {
            get; set;// { return Platform.Current.ServiceLocator.SensorService; }
        }

        private readonly IStorage _storage;

        public ActivityRecorder(ISensorService sensorService, IStorage storage)
        {
            SensorService = sensorService;
            _storage = storage;
            _activity = new RecordingActivity();
        }

        private bool _initialized;

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            //    Activity.Sport = Settings.ActivityRecorder.Sport; // default sport

            // restore interrupted activity recording
            var folderActivities = await GetFolderAsync();
            _fileDest = await folderActivities.TryGetItemAsync(TempFileName);
            if (_fileDest != null)
            {
                // there is an already started, interrupted recording, load data from it
                var importer = new FitImporter(_activity);

                try
                {
                    using (var stream = await _fileDest.OpenForReadAsync())
                    {
                        await importer.LoadAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                _activity.RecalculateCurrentLap();
                if (_activity.TimeFrames.Count > 0)
                {
                    await PrepareDestinationFileAsync();
                    IsStarted = true;
                }
            }

            SensorService.Clock.StatusChanged += OnClockTick;

            _initialized = true;
        }

        void OnClockTick(object sender, SensorStateEventArgs e)
        {
            OnTick((DateTime)e.Data);
        }

        public void Uninitialize()
        {
            if (!_initialized)
                return;

            SensorService.Clock.StatusChanged -= OnClockTick;
            if (_streamDest != null)
            {
                _exporter.Close();
                _streamDest.Dispose();
            }
            _fileDest = null;
            _initialized = false;
        }

        /// <summary>
        /// Starts a new lap
        /// </summary>
        public void NewLap()
        {
            var currentLap = _activity.Laps.Current;
            if (currentLap != null)
            {
                if (_streamDest != null)
                    _exporter.WriteLap(currentLap);
            }
            _activity.NewLap();
        }

        /// <summary>
        /// Gets the folder where the file containing recording activity found.
        /// </summary>
        /// <returns>The folder's path</returns>
        private async Task<ILocalFolder> GetFolderAsync()
        {
            return await _storage.GetFolderAsync("Activities");
        }

        private void AddTimeFrame(ActivityTimeFrame timeFrame)
        {
            lock (_activity)
            {
                _activity.AddTimeFrame(timeFrame);
                if (_exporter != null)
                    _exporter.WriteTimeFrame(timeFrame); // save the activitiy
            }
        }

        private async Task PrepareDestinationFileAsync()
        {
            if (_fileDest == null)
                return;

            try
            {
                _streamDest = await _fileDest.OpenForWriteAsync();  // FIT encoder has to read also
            }
            catch (UnauthorizedAccessException accessException)
            {
                throw new Exception(string.Format("Cannot write file: {0}", _fileDest.Path), accessException);
            }

            // recode the found activity
            // the old file can be invalid
            _exporter = new FitExporter(_activity);
            _exporter.Open(_streamDest);
            _exporter.WriteHeader();
            _exporter.WriteTimeFrames();
            _streamDest.Flush();
        }

        /// <summary>
        /// Start recording
        /// </summary>
        public async Task StartAsync(bool isTrial = false)
        {
            if (IsStarted)
            {
                if (IsActive)
                    return; // it's already running

                // running and paused, let's resume
                _lastRecordedTime = DateTime.Now;
                AddTimeFrame(new ActivityTimeFrame { Timestamp = _lastRecordedTime, Type = ActivityTimeFrameType.Start });
                IsActive = true;
            }
            else
            {
                // Not started yet

                if (!isTrial)
                {
                    // if the app has been bought now, prepare saving to file
                    var folderActivities = await GetFolderAsync();
                    _fileDest = await folderActivities.CreateFileAsync(TempFileName, CreateFileOption.ReplaceExisting);
                    await PrepareDestinationFileAsync();
                }

                _lastRecordedTime = DateTime.Now;
                //                _timer.Start();
                IsStarted = true;
                IsActive = true;
            }
            if (Started != null)
                Started(this, new ActivityRecorderEventArgs { Recorder = this });
        }

        /// <summary>
        /// Finish recording
        /// </summary>
        public void Finish()
        {
            if (!IsStarted)
                return;

            //            _timer.Stop();
            IsActive = false;
            IsStarted = false;

            if (_streamDest != null)
            {
                // if the activity should be saved, then export it
                _exporter.WriteSummary();
                _exporter.Close();
                _streamDest.Dispose();

                _streamDest = null;
                // rename the temp file
                _fileDest.RenameAsync(string.Format("{0:yyyy-MM-dd-HHmmss}.fit", _activity.StartTime));
                _fileDest = null;
            }
            // reset measurement values
            _activity.Reset();
            //            _moveCalculator.Reset();

            Finished?.Invoke(this, new ActivityRecorderEventArgs { Recorder = this });
        }

        /// <summary>
        /// Stop/puse recording
        /// </summary>
        public void Stop()
        {
            if (!IsStarted)
                return;

            if (!IsActive)
                return;

            AddTimeFrame(new ActivityTimeFrame { Timestamp = _lastRecordedTime, Type = ActivityTimeFrameType.Stop });
            IsActive = false;
            _autoPaused = false;
            if (Stopped != null)
                Stopped(this, new ActivityRecorderEventArgs { Recorder = this });
        }

        private void OnTick(DateTime now)
        {
            if (!IsStarted || !IsActive)
                return;

            _lastRecordedTime = now;

            var timeFrame = new ActivityTimeFrame();
            timeFrame.Timestamp = now;

            // Get useful data from detected sensors
            if (SensorService != null)
            {
                if (SensorService.Gps != null)
                {
                    // Position from GPS
                    var status = SensorService.Gps.Geolocator.Status;
                    if (!status.Position.IsEmpty)
                    {
                        timeFrame.Position = status.Position;
                    }
                }
                if (SensorService.HeartRateMonitor != null)
                {
                    var hrData = SensorService.HeartRateMonitor.HeartRateData;
                    if (hrData != null)
                        timeFrame.HeartRate = (byte)hrData.HeartRateValue;
                }

                if (SensorService.SpeedSensor != null)
                {
                    // trying to get speed from speed sensor first
                    timeFrame.Speed = SensorService.SpeedSensor.Speed;
                }
                else if (SensorService.Gps != null)
                {
                    // fallback to GPS
                    if (SensorService.Gps.Geolocator.Status != null)
                    {
                        var speed = (float)SensorService.Gps.Geolocator.Status.Speed;
                        if (speed > 0)
                            timeFrame.Speed = new Speed(speed, SpeedUnit.MeterPerSecond);
                    }
                }

                // update cadence data
                if (SensorService.CadenceSensor != null)
                {
                    timeFrame.Cadence = SensorService.CadenceSensor.Cadence.RevolutionsPerMinute;
                }

                // update power data
                if (SensorService.BikePowerMeter != null)
                {
                    if (SensorService.BikePowerMeter.Power.HasValue)
                        timeFrame.Power = (ushort)SensorService.BikePowerMeter.Power.Watts;
                }
            }
            if (AutoPause)
            {
                var speedMeterPerSecond = timeFrame.Speed.GetValueOrDefault().GetValueAs(SpeedUnit.MeterPerSecond);
                // check auto-pause speed limit
                if (speedMeterPerSecond < AutoPauseSpeedLimit.GetValueAs(SpeedUnit.MeterPerSecond) && !_autoPaused)
                {
                    _autoPaused = true;
                    AddTimeFrame(new ActivityTimeFrame { Timestamp = now, Type = ActivityTimeFrameType.Stop });
                    AutoPaused?.Invoke(this, EventArgs.Empty);
                }
                else if (_autoPaused && speedMeterPerSecond >= AutoPauseSpeedLimit.GetValueAs(SpeedUnit.MeterPerSecond))
                {
                    _autoPaused = false;
                    AddTimeFrame(new ActivityTimeFrame { Timestamp = now, Type = ActivityTimeFrameType.Start });
                    AutoResumed?.Invoke(this, EventArgs.Empty);
                }
                if (_autoPaused)
                    return;
            }
            AddTimeFrame(timeFrame);
            if (_streamDest != null)
                _streamDest.Flush();
            Modified?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ActivityRecorderEventArgs : EventArgs
    {
        public ActivityRecorder Recorder { get; set; }
    }
}
