//
// PowerMeasurement.cs
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
    /// Cycling power measurement data
    /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.cycling_power_measurement.xml
    /// </summary>
    public class PowerMeasurement : Measurement
    {
        [Flags]
        private enum PowerMeasurementFlags : ushort
        {
            None = 0x00,
            PedalPowerBalancePresent = 0x01,
            /// <summary>
            /// 0 Unknown
            /// 1 Left 
            /// </summary>
            PedalPowerBalanceReference = 0x02,
            AccumulatedTorquePresent = 0x04,
            /// <summary>
            /// 0 Wheel Based 
            /// 1 Crank Based 
            /// </summary>
            AccumulatedTorqueSource = 0x08,
            WheelRevolutionDataPresent = 0x10,
            CrankRevolutionDataPresent = 0x20,
            ExtremeForceMagnitudesPresent = 0x40,
            ExtremeTorqueMagnitudesPresent = 0x80,
            ExtremeAnglesPresent = 0x100,
            TopDeadSpotAnglePresent = 0x200,
            BottomDeadSpotAnglePresent = 0x400,
            AccumulatedEnergyPresent = 0x800,

            //12 1 Offset Compensation Indicator
        }

        private PowerMeasurementFlags Flags { get; set; }

        /// <summary>
        /// Instantaneous Power 
        /// Unit is in watts with a resolution of 1.  
        /// </summary>
        public Int16 InstantaneousPower { get; private set; }

        /// <summary>
        /// Pedal Power Balance 
        /// Unit is in percentage with a resolution of 1/2.  
        /// </summary>
        public byte PedalPowerBalance { get; private set; }

        public bool IsEmpty
        {
            get { return InstantaneousPower == 0 && PedalPowerBalance == 0; }
        }

        public PowerMeasurement(byte[] data)
        {
            // get flags
            var currentOffset = 0;
            Flags = (PowerMeasurementFlags)BitConverter.ToUInt16(data, currentOffset);
            currentOffset += 2;

            if (currentOffset + 2 > data.Length)
                return;
            InstantaneousPower = BitConverter.ToInt16(data, currentOffset);
            currentOffset += 2;

            if ((Flags & PowerMeasurementFlags.PedalPowerBalancePresent) != PowerMeasurementFlags.None)
            {
                if (currentOffset + 1 > data.Length)
                    return;
                PedalPowerBalance = data[currentOffset++];
            }

            IsValid = true;
        }
    }
}
