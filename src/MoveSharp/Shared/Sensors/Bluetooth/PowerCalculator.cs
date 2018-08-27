//
// PowerCalculator.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using System;
using XTools.Diagnostics;

namespace MoveSharp.Sensors.Bluetooth
{
    public class PowerCalculator : MeasurementCalculator<PowerMeasurement, Power>
    {
        private PowerMeasurement _lastMeasurement;
        private Power _power;
        private DateTime _lastMeasurementTimestamp;

        public override void Update(Measurement measurement)
        {
            var powerMeasurement = measurement as PowerMeasurement;
            if (powerMeasurement == null)
                return;

            if (powerMeasurement.IsValid == false)
                return;

            Log.Diagnostics("Bike power data received: {0}", powerMeasurement.InstantaneousPower);

            if (!powerMeasurement.IsEmpty)
            {
                // this is the first measurement
                _lastMeasurement = powerMeasurement;
                _lastMeasurementTimestamp = DateTime.Now;

                _power.Watts = powerMeasurement.InstantaneousPower;
            }
            else if (DateTime.Now.Subtract(_lastMeasurementTimestamp).TotalSeconds > MaxAge)
            {
                _power = Power.None;
            }

            // notifiy subscribers
            OnChanged(_power);

        }
    }
}

