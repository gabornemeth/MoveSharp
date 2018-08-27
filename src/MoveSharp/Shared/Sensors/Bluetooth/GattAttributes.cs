//
// GattAttributes.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;

namespace MoveSharp.Sensors.Bluetooth
{
    public class GattServiceGuids
    {
        public static Guid HeartRate
        {
            get
            {
                return new Guid("0000180d-0000-1000-8000-00805f9b34fb");
            }
        }

        public static Guid CyclingSpeedAndCadence
        {
            get
            {
                return new Guid("00001816-0000-1000-8000-00805f9b34fb");
            }
        }

        public static Guid CyclingPower
        {
            get
            {
                return new Guid("00001818-0000-1000-8000-00805f9b34fb");
            }
        }

        public static Guid RunningSpeedAndCadence
        {
            get
            {
                return new Guid("00001814-0000-1000-8000-00805f9b34fb");
            }
        }
    }

    public class GattCharacteristicGuids
    {
        public static Guid CscMeasurement
        {
            get
            {
                return new Guid("00002a5b-0000-1000-8000-00805f9b34fb");
            }
        }

        public static Guid HeartRateMeasurement
        {
            get
            {
                return new Guid("00002a37-0000-1000-8000-00805f9b34fb");
            }
        }
        //"00002a66-0000-1000-8000-00805f9b34fb":"Cycling Power Control Point",
        //"00002a65-0000-1000-8000-00805f9b34fb":"Cycling Power Feature",

        /// <summary>
        /// Running speed and cadence measurement
        /// </summary>
        /// <value>The rsc measurement.</value>
        public static Guid RscMeasurement
        {
            get
            { 
                return new Guid("00002a53-0000-1000-8000-00805f9b34fb");
            }
        }

        public static Guid CyclingPowerMeasurement
        {
            get
            {
                return new Guid("00002a63-0000-1000-8000-00805f9b34fb");
            }
        }

        //"00002a64-0000-1000-8000-00805f9b34fb":"Cycling Power Vector",
    }

    public class GattDescriptorGuids
    {
        public static Guid ClientCharacteristicConfig = new Guid("00002902-0000-1000-8000-00805f9b34fb");
    }
}
