//
// RscMeasurement.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//
        
using System;
using System.Collections.Generic;
using System.Text;

namespace MoveSharp.Sensors.Bluetooth
{
    /// <summary>
    /// Running speed and cadence measurement data
    /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.rsc_measurement.xml
    /// </summary>
    public class RscMeasurement : Measurement
    {
        /// <summary>
        /// Instantaneous Speed
        /// Unit is in m/s with a resolution of 1/256 s
        /// </summary>
        public UInt16 InstantaneousSpeed { get; set; }

        /// <summary>
        /// Instantaneous Cadence
        /// Unit is in 1/minute (or RPM) with a resolutions of 1 1/min (or 1 RPM)
        /// </summary>
        public byte InstantaneousCadence { get; set; }

        /// <summary>
        /// Instantaneous Stride Length
        /// C1: Field exists if the key of bit 0 of the Flags field is set to 1.
        /// Unit is in meter with a resolution of 1/100 m (or centimeter).
        /// </summary>
        public UInt16 InstantaneousStrideLength { get; set; }

        /// <summary>
        /// Total Distance
        /// C2: Field exists if the key of bit 1 of the Flags field is set to 1.
        /// Unit is in meter with a resolution of 1/10 m (or decimeter).
        /// </summary>
        public UInt32 TotalDistance { get; set; }

        /// <summary>
        /// Running Speed and Cadence profile defined flag values
        /// </summary>
        [Flags]
        public enum RscMeasurementFlags
        {
            None = 0x00,
            InstantaneousStrideLengthPresent = 0x01,
            TotalDistancePresent = 0x02,
            WalkingOrRunningStatus = 0x04
        }

        public RscMeasurement(byte[] data)
        {
            // get flags
            var currentOffset = 0;
            var flags = (RscMeasurementFlags)data[currentOffset];
            currentOffset++;

            // Read instantaneous speed and cadence
            InstantaneousSpeed = BitConverter.ToUInt16(data, currentOffset);
            currentOffset += 2;
            InstantaneousCadence = data[currentOffset];
            currentOffset++;

            if ((flags & RscMeasurementFlags.InstantaneousStrideLengthPresent) != RscMeasurementFlags.None)
            {
                if (currentOffset + 2 > data.Length)
                    return; // invalid data

                // speed data is present
                InstantaneousStrideLength = BitConverter.ToUInt16(data, currentOffset);
                currentOffset += 2;
            }

            if ((flags & RscMeasurementFlags.TotalDistancePresent) != RscMeasurementFlags.None)
            {
                if (currentOffset + 4 > data.Length)
                    return; // invalid data
                
                // cadence data is present
                TotalDistance = BitConverter.ToUInt32(data, currentOffset);
                currentOffset += 4;
            }

            IsValid = true;
        }

        public bool IsEmpty
        {
            get { return InstantaneousSpeed == 0 && InstantaneousCadence == 0 && InstantaneousStrideLength == 0; }
        }

        public bool Equals(RscMeasurement measurement)
        {
            return InstantaneousSpeed == measurement.InstantaneousSpeed &&
            InstantaneousCadence == measurement.InstantaneousCadence &&
            TotalDistance == measurement.TotalDistance &&
            InstantaneousStrideLength == measurement.InstantaneousStrideLength;
        }

//        public static bool operator ==(RscMeasurement p1, RscMeasurement p2)
//        {
//            return p1.Equals(p2);
//        }
//
//        public static bool operator !=(RscMeasurement p1, RscMeasurement p2)
//        {
//            return !p1.Equals(p2);
//        }

        public bool HasTotalDistance
        {
            get { return TotalDistance != 0; }
        }

        public bool HasInstantaneousStride
        {
            get { return InstantaneousStrideLength != 0; }
        }
    }
}
