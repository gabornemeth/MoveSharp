using System;
using System.Collections.Generic;
using System.Text;

namespace MoveSharp.Sensors.Bluetooth
{
    /// <summary>
    /// Cycling speed and cadence measurement data
    /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.csc_measurement.xml
    /// </summary>
    public class CscMeasurement : Measurement
    {
        /// <summary>
        /// Cumulative number of wheel revolutions
        /// </summary>
        public UInt32 WheelRevolutions { get; set; }
        public UInt16 WheelEventTimestamp { get; set; }
        /// <summary>
        /// Cumulative number of crank revolutions
        /// </summary>
        public UInt16 CrankRevolutions { get; set; }
        public UInt16 CrankEventTimestamp { get; set; }

        public bool IsEmpty
        {
            get { return WheelRevolutions == 0 && WheelEventTimestamp == 0 && CrankRevolutions == 0 && CrankEventTimestamp == 0; }
        }

        // Cycling Speed and Cadence profile defined flag values
        private const byte WheelRevolutionDataPresent = 0x01;
        private const byte CrankRevolutionDataPresent = 0x02;

        private CscMeasurement()
        {
        }

        public CscMeasurement(byte[] data)
        {
            // get flags
            var currentOffset = 0;
            var flags = data[currentOffset];
            currentOffset++;

            if ((flags & WheelRevolutionDataPresent) != 0)
            {
                // speed data is present
                WheelRevolutions = BitConverter.ToUInt32(data, currentOffset);
                currentOffset += 4;
                WheelEventTimestamp = BitConverter.ToUInt16(data, currentOffset);
                currentOffset += 2;
            }

            if ((flags & CrankRevolutionDataPresent) != 0 && data.Length >= currentOffset + 4)
            {
                // cadence data is present
                CrankRevolutions = BitConverter.ToUInt16(data, currentOffset);
                currentOffset += 2;
                CrankEventTimestamp = BitConverter.ToUInt16(data, currentOffset);
            }
        }

        public bool Equals(CscMeasurement measurement)
        {
            return WheelRevolutions == measurement.WheelRevolutions &&
                   WheelEventTimestamp == measurement.WheelEventTimestamp &&
                   CrankRevolutions == measurement.CrankRevolutions &&
                   CrankEventTimestamp == measurement.CrankEventTimestamp;
        }

//        public static bool operator ==(CscMeasurement p1, CscMeasurement p2)
//        {
//            return p1.Equals(p2);
//        }
//
//        public static bool operator !=(CscMeasurement p1, CscMeasurement p2)
//        {
//            return !p1.Equals(p2);
//        }

        public bool HasWheelChange
        {
            get { return WheelRevolutions != 0; }
        }

        public bool HasCrankChange
        {
            get { return CrankRevolutions != 0; }
        }

        public CscMeasurement GetDifference(CscMeasurement previous)
        {
            var diff = new CscMeasurement
            {
                WheelRevolutions = WheelRevolutions - previous.WheelRevolutions,
                WheelEventTimestamp = (ushort)(WheelEventTimestamp - previous.WheelEventTimestamp),
                CrankRevolutions = (ushort)(CrankRevolutions - previous.CrankRevolutions),
                CrankEventTimestamp = (ushort)(CrankEventTimestamp - previous.CrankEventTimestamp)
            };

            return diff;
        }
    }
}
