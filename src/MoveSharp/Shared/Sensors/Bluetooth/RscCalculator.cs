//
// RscCalculator.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using MoveSharp.Models;
using System;
using XTools.Diagnostics;

namespace MoveSharp.Sensors.Bluetooth
{
    /// <summary>
    /// Running speed and cadence calculator
    /// </summary>
    public class RscCalculator : MeasurementCalculator<RscMeasurement, SpeedAndCadence>
    {
        private RscMeasurement _lastMeasurement;
        private DateTime _lastMeasurementTimestamp;
        private SpeedAndCadence _speedAndCadence;
        private IStrideSensor _sensor;

        public RscCalculator(IStrideSensor sensor)
        {
            if (sensor == null)
                throw new ArgumentNullException("sensor");
            _sensor = sensor;
        }

        public void Reset()
        {
            _lastMeasurement = null;
        }

        public override void Update(Measurement m)
        {
            var measurement = m as RscMeasurement;
            if (measurement == null)
                throw new NotSupportedException("Invalid measurement type " + m.GetType());
            
            Log.Diagnostics("RSC data received:\n\t{0} {1}", measurement.InstantaneousSpeed, measurement.InstantaneousCadence);

            if (!measurement.IsEmpty)
            {
                // this is the first measurement
                _lastMeasurement = measurement;
                _lastMeasurementTimestamp = DateTime.Now;

                if (measurement.InstantaneousSpeed != 0)
                    _speedAndCadence.Speed = new Speed(measurement.InstantaneousSpeed / 256.0f, SpeedUnit.MeterPerSecond);
                if (measurement.InstantaneousCadence != 0)
                    _speedAndCadence.Cadence = measurement.InstantaneousCadence;

            }
            else if (_lastMeasurement != null && DateTime.Now.Subtract(_lastMeasurementTimestamp).TotalSeconds > MaxAge)
            {
                _speedAndCadence.Speed = new Speed();
                _speedAndCadence.Cadence = 0;
                _lastMeasurement = null;
            }

            // notifiy subscribers
            OnChanged(_speedAndCadence);
        }
    }
}
