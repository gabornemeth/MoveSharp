//
// CscCalculator.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using MoveSharp.Models;
using System;
using XTools.Diagnostics;

namespace MoveSharp.Sensors.Bluetooth
{
    /// <summary>
    /// Cycling speed and cadence calculator
    /// </summary>
    public class CscCalculator : MeasurementCalculator<CscMeasurement, SpeedAndCadence>
    {
        private CscMeasurement _lastMeasurement;
        private DateTime _lastCadMeasurementTimestamp, _lastWheelMeasurementTimestamp;
        private SpeedAndCadence _validSpeedAndCadence, _speedAndCadence;
        private IBikeSpeedAndCadenceSensor _sensor;

        public CscCalculator(IBikeSpeedAndCadenceSensor sensor)
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
            var measurement = (CscMeasurement)m;
            Log.Diagnostics("Bluetooth CSC data received:\n\t{0} {1}\n\t{2} {3}", measurement.WheelRevolutions, measurement.WheelEventTimestamp,
                measurement.CrankRevolutions, measurement.CrankEventTimestamp);
//            Log.Diagnostics("Last CSC data:\n\t{0} {1}\n\t{2} {3}", _lastMeasurement.WheelRevolutions, _lastMeasurement.WheelEventTimestamp,
//                _lastMeasurement.CrankRevolutions, _lastMeasurement.CrankEventTimestamp);

            if (_lastMeasurement == null)
            {
                // this is the first measurement
                _lastMeasurement = measurement;
                _lastWheelMeasurementTimestamp = _lastCadMeasurementTimestamp = DateTime.Now;
                return;
            }

            // Speed or cadence has been changed since last time
            var diff = measurement.GetDifference(_lastMeasurement); // get changes
            if (diff.HasCrankChange)
            {
                _lastMeasurement.CrankRevolutions = measurement.CrankRevolutions;
                _lastMeasurement.CrankEventTimestamp = measurement.CrankEventTimestamp;
                _speedAndCadence.Cadence = Convert.ToByte(System.Math.Min(255, Convert.ToInt32(diff.CrankEventTimestamp == 0 ? 0 : diff.CrankRevolutions / (diff.CrankEventTimestamp / 1024.0f) * 60)));
                _lastCadMeasurementTimestamp = DateTime.Now;
            }
            else if (DateTime.Now.Subtract(_lastCadMeasurementTimestamp).TotalSeconds > MaxAge)
            {
                _speedAndCadence.Cadence = 0;
            }

            if (diff.HasWheelChange)
            {
                _lastMeasurement.WheelRevolutions = measurement.WheelRevolutions;
                _lastMeasurement.WheelEventTimestamp = measurement.WheelEventTimestamp;
                _speedAndCadence.Speed = new Speed(diff.WheelEventTimestamp == 0 ? 0 : diff.WheelRevolutions * _sensor.WheelSize * 0.001f / (diff.WheelEventTimestamp / 1024.0f), SpeedUnit.MeterPerSecond);
                _lastWheelMeasurementTimestamp = DateTime.Now;
            }
            else if (DateTime.Now.Subtract(_lastWheelMeasurementTimestamp).TotalSeconds > MaxAge)
            {
                _speedAndCadence.Speed = new Speed(0, SpeedUnit.MeterPerSecond);
            }

            // if data has changed, notify the subscribers
            if (_validSpeedAndCadence != _speedAndCadence)
            {
                _validSpeedAndCadence = _speedAndCadence;
                OnChanged(_speedAndCadence);
            }

        }
    }
}
