//
// MemoryLap.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using MoveSharp.Extensions;
using MoveSharp.Geolocation;
using MoveSharp.Math;

namespace MoveSharp.Models
{
    /// <summary>
    /// Lap for in-memory use
    /// Can be used for analysis
    /// </summary>
    public class MemoryLap : ILap
    {
        private List<ActivityTimeFrame> _timeFrames;
        private ActivityTimeFrame _lastFrame;

        /// <summary>
        /// Collection of frames
        /// </summary>
        public IList<ActivityTimeFrame> TimeFrames
        {
            get
            {
                return _timeFrames;
            }
        }

        private Distance _distance;
        /// <summary>
        /// Distance covered in the lap
        /// </summary>
        public Distance Distance
        {
            get { return _distance; }
            protected set { _distance = value; }
        }

        /// <summary>
        /// Average heart rate [bpm]
        /// </summary>
        public int AvgHeartRate
        {
            get;
            protected set;
        }

        /// <summary>
        /// Maximum heart rate [bpm]
        /// </summary>
        public int MaxHeartRate
        {
            get;
            protected set;
        }

        public int AvgPower
        {
            get;
            protected set;
        }

        public int MaxPower
        {
            get;
            protected set;
        }

        private Speed _avgSpeed;

        public Speed AvgSpeed
        {
            get { return _avgSpeed; }
            protected set
            {
                if (_avgSpeed.Equals(value))
                    return;
                _avgSpeed = value;
            }
        }

        private Speed _maxSpeed;
        public Speed MaxSpeed
        {
            get { return _maxSpeed; }
            protected set
            {
                if (!_maxSpeed.Equals(value))
                    _maxSpeed = value;
            }
        }

        public int AvgCadence
        {
            get;
            set;
        }

        public int MaxCadence
        {
            get;
            protected set;
        }


        private float _elapsedTime;
        /// <summary>
        /// Total elapsed seconds
        /// </summary>
        public int ElapsedTime
        {
            get
            {
                return (int)_elapsedTime;
            }
            protected set
            {
                _elapsedTime = value;
            }
        }

        private float _movingTime;
        /// <summary>
        /// Total elapsed seconds
        /// </summary>
        public int MovingTime
        {
            get
            {
                return (int)_movingTime;
            }
        }

        public float Ascent
        {
            get; protected set;
        }

        public float Descent
        {
            get; protected set;
        }

        private MoveCalculator _moveCalculator;
        private Statistics _hrCalculator, _powerCalculator, _cadenceCalculator, _speedCalculator;
        private AscentCalculator _ascentCalculator;

        public event EventHandler Reseted;
        public event EventHandler<ActivityTimeFrame> FrameAdded;

        public MemoryLap()
        {
            _timeFrames = new List<ActivityTimeFrame>();
            _moveCalculator = new MoveCalculator();
            _hrCalculator = new Statistics();
            _powerCalculator = new Statistics();
            _cadenceCalculator = new Statistics();
            _speedCalculator = new Statistics();
            _avgSpeed.Unit = _maxSpeed.Unit = SpeedUnit.MeterPerSecond;
            _ascentCalculator = new AscentCalculator(0.0f);
        }

        /// <summary>
        /// Resets the lap
        /// </summary>
        public virtual void Reset()
        {
            _lastFrame = null;
            AvgCadence = MaxCadence = 0;
            AvgHeartRate = MaxHeartRate = 0;
            AvgPower = MaxPower = 0;
            _avgSpeed.Value = 0;
            _maxSpeed.Value = 0;
            _distance = new Distance(0, DistanceUnit.Meter);
            StartTime = DateTime.MinValue;
            _elapsedTime = 0;
            _movingTime = 0;
            _timeFrames.Clear();
            _cadenceCalculator.Reset();
            _hrCalculator.Reset();
            _powerCalculator.Reset();
            _moveCalculator.Reset();
            _ascentCalculator.Reset();
            Reseted?.Invoke(this, EventArgs.Empty);
        }

        protected void AddFrame(ActivityTimeFrame timeFrame)
        {
            lock (this)
            {
                _timeFrames.Add(timeFrame);
                _lastFrame = timeFrame;
            }
            FrameAdded?.Invoke(this, timeFrame);
        }

        public virtual void AddTimeFrame(ActivityTimeFrame timeFrame)
        {
            TimeSpan elapsedTime = TimeSpan.FromSeconds(0);
            if (_timeFrames.Count > 0)
            {
                // calculate elapsed and moving time
                elapsedTime = timeFrame.Timestamp.Subtract(_lastFrame.Timestamp);
            }
            _elapsedTime += (float)elapsedTime.TotalSeconds;

            if (timeFrame.Type != ActivityTimeFrameType.Active)
            {
                AddFrame(timeFrame);
                _moveCalculator.Reset();
                _ascentCalculator.Reset();
                return;
            }

            _movingTime += (float)elapsedTime.TotalSeconds;

            // Calculating averages, sums

            // We should handle when multiple frames have been registered with the same timestamp.
            // Movescount does this.

            // update move calculator
            if (timeFrame.Position.HasValue)
            {
                _moveCalculator.Add(timeFrame.Position.Value, elapsedTime);
                var altitude = timeFrame.Position.Value.Altitude;
                if (altitude != 0)
                {
                    _ascentCalculator.AddAltitude(altitude);
                    if (_ascentCalculator.HasTresholdReached)
                    {
                        if (_ascentCalculator.Ascent > 0)
                            Ascent += _ascentCalculator.Ascent;
                        else
                            Descent += -_ascentCalculator.Ascent;
                        _ascentCalculator.Reset();
                        _ascentCalculator.AddAltitude(altitude);
                    }
                }
            }
            else
            {
                _moveCalculator.Reset();
            }

            float currentSpeed = 0;
            if (timeFrame.Speed.HasValue)
            {   // has speed data
                currentSpeed = timeFrame.Speed.Value.GetValueAs(SpeedUnit.MeterPerSecond);
            }
            else
            {
                // fallback to moveCalculator
                currentSpeed = _moveCalculator.CurrentSpeed;
            }
            if (elapsedTime.TotalSeconds > 0)
                _speedCalculator.Add(currentSpeed);
            else if (timeFrame.Speed.HasValue)
                _speedCalculator.UpdateLast(currentSpeed); // we have more precise speed data in the same timeframe
            _maxSpeed.Value = _speedCalculator.Maximum;
            _avgSpeed.Value = _speedCalculator.Average;
            _distance.Value += (float)(elapsedTime.TotalSeconds * currentSpeed);

            if (timeFrame.Power.HasValue)
            {
                _powerCalculator.Add(timeFrame.Power.Value);
                MaxPower = Convert.ToInt32(_powerCalculator.Maximum);
                AvgPower = Convert.ToInt32(_powerCalculator.Average);
            }
            if (timeFrame.HeartRate.HasValue)
            {
                _hrCalculator.Add(timeFrame.HeartRate.Value);
                MaxHeartRate = Convert.ToInt32(_hrCalculator.Maximum);
                AvgHeartRate = Convert.ToInt32(_hrCalculator.Average);
            }
            if (timeFrame.Cadence.HasValue)
            {
                _cadenceCalculator.Add(timeFrame.Cadence.Value);
                MaxCadence = Convert.ToInt32(_cadenceCalculator.Maximum);
                AvgCadence = Convert.ToInt32(_cadenceCalculator.Average);
            }

            if (StartTime == DateTime.MinValue)
                StartTime = timeFrame.Timestamp;

            if (_lastFrame == null || elapsedTime.TotalSeconds > 0)
                AddFrame(timeFrame);
        }

        /// <summary>
        /// Updates the total values with summary values
        /// </summary>
        /// <param name="summary"></param>
        public void SetSummary(ILapSummary summary)
        {
            // update with valid values from summary
            if (summary.AvgSpeed.HasValue && !FitExtensions.InvalidSpeed.Equals(summary.AvgSpeed.Value))
                AvgSpeed = summary.AvgSpeed;
            if (summary.MaxSpeed.HasValue && !FitExtensions.InvalidSpeed.Equals(summary.MaxSpeed.Value))
                MaxSpeed = summary.MaxSpeed;
            if (summary.AvgCadence != FitExtensions.InvalidCadence && summary.AvgCadence != 0)
                AvgCadence = summary.AvgCadence;
            if (summary.MaxCadence != FitExtensions.InvalidCadence && summary.MaxCadence != 0)
                MaxCadence = summary.MaxCadence;
            if (summary.AvgHeartRate != FitExtensions.InvalidHeartRate && summary.AvgHeartRate != 0)
                AvgHeartRate = summary.AvgHeartRate;
            if (summary.MaxHeartRate != FitExtensions.InvalidHeartRate && summary.MaxHeartRate != 0)
                MaxHeartRate = summary.MaxHeartRate;
            if (summary.AvgPower != FitExtensions.InvalidPower && summary.AvgPower != 0)
                AvgPower = summary.AvgPower;
            if (summary.MaxPower != FitExtensions.InvalidPower && summary.MaxPower != 0)
                MaxPower = summary.MaxPower;
            if (summary.Distance.HasValue && summary.Distance.Value != Distance.InvalidValue)
                _distance = summary.Distance;
            if (summary.StartTime != DateTime.MinValue)
                StartTime = summary.StartTime;
            if (summary.ElapsedTime != 0)
                _elapsedTime = summary.ElapsedTime;
            if (summary.MovingTime != 0)
                _movingTime = summary.MovingTime;
            if (summary.Ascent != 0)
                Ascent = summary.Ascent;
            if (summary.Descent != 0)
                Descent = summary.Descent;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        private double _powerHeartRateDecoupling;

        public double GetPowerHeartRateDecoupling(bool calculate = false)
        {
            if (calculate || _powerHeartRateDecoupling == 0 || double.IsNaN(_powerHeartRateDecoupling))
                CalculateDecoupling();

            return _powerHeartRateDecoupling;
        }

        private double _paceHeartRateDecoupling;

        public double GetPaceHeartRateDecoupling(bool calculate = false)
        {
            if (calculate || _paceHeartRateDecoupling == 0 || double.IsNaN(_paceHeartRateDecoupling))
                CalculateDecoupling();

            return _paceHeartRateDecoupling;
        }

        /// <summary>
        /// Calculates decoupling values
        /// </summary>
        private void CalculateDecoupling()
        {
            if (TimeFrames.Count < 2)
            {
                _powerHeartRateDecoupling = 0;
                _paceHeartRateDecoupling = 0;
            }
            else
            {
                var firstHalf = new MemoryLap(); // first half
                var secondHalf = new MemoryLap(); // second half
                foreach (var timeFrame in TimeFrames)
                {
                    if (timeFrame.Timestamp.Subtract(StartTime).TotalSeconds < ElapsedTime * 0.5)
                        firstHalf.AddTimeFrame(timeFrame);
                    else
                    {
                        secondHalf.AddTimeFrame(timeFrame);
                    }
                }

                if (firstHalf.AvgHeartRate == 0 || secondHalf.AvgHeartRate == 0 || firstHalf.AvgPower == 0)
                    _powerHeartRateDecoupling = double.NaN;
                else
                    _powerHeartRateDecoupling = ((double)firstHalf.AvgPower / firstHalf.AvgHeartRate - (double)secondHalf.AvgPower / secondHalf.AvgHeartRate) /
                                                ((double)firstHalf.AvgPower / firstHalf.AvgHeartRate);
                const SpeedUnit speedUnit = SpeedUnit.MeterPerSecond;
                if (firstHalf.AvgHeartRate == 0 || secondHalf.AvgHeartRate == 0 || firstHalf.AvgSpeed.GetValueAs(speedUnit) == 0)
                    _paceHeartRateDecoupling = double.NaN;
                else
                    _paceHeartRateDecoupling = (firstHalf.AvgSpeed.GetValueAs(speedUnit) / firstHalf.AvgHeartRate - secondHalf.AvgSpeed.GetValueAs(speedUnit) / secondHalf.AvgHeartRate) /
                                                                (firstHalf.AvgSpeed.GetValueAs(speedUnit) / firstHalf.AvgHeartRate);
            }
        }
    }
}
