//
// AscentCalculator.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Text;

namespace MoveSharp.Geolocation
{
    /// <summary>
    /// Ascent calculator
    /// </summary>
    public class AscentCalculator
    {
        private float _treshold;
        private float _ascent;
        /// <summary>
        /// Last measured altitude value in meters
        /// </summary>
        private float _lastAltitude;

        /// <summary>
        /// Initializes a new instance of <see cref="AscentCalculator"/>
        /// </summary>
        /// <param name="treshold">Treshold for ascent/descent to be valid.</param>
        public AscentCalculator(float treshold = 5)
        {
            if (treshold < 0)
                throw new ArgumentException("Treshold cannot be negative!", nameof(treshold));

            _treshold = treshold;
        }

        public void Reset()
        {
            _ascent = 0;
            _lastAltitude = 0;
        }

        public bool HasTresholdReached
        {
            get
            {
                return System.Math.Abs(_ascent) > _treshold;
            }
        }

        public float Ascent
        {
            get
            {
                return _ascent;
            }
        }

        public void AddAltitude(float altitudeInMeters)
        {
            //System.Diagnostics.Debug.WriteLine($"Altitude: {altitudeInMeters}");
            if (_lastAltitude != 0)
            {
                // having already at least one measurement
                if (altitudeInMeters > _lastAltitude)
                    _ascent += (altitudeInMeters - _lastAltitude);
            }
            _lastAltitude = altitudeInMeters;
        }
    }
}
