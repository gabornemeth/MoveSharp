//
// BluetoothHeartRateHelper.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;

namespace MoveSharp.Sensors.Bluetooth
{
    public static class BluetoothHeartRateHelper
    {
        public static HeartRateMeasurement GetHeartRate(byte[] data)
        {
            // Heart Rate profile defined flag values
            const byte HEART_RATE_VALUE_FORMAT = 0x01;
            const byte ENERGY_EXPANDED_STATUS = 0x08;
            const byte RR = 0x10;

            // get flags
            byte currentOffset = 0;
            byte flags = data[currentOffset];
            currentOffset++;

            var measurment = new HeartRateMeasurement();

            // get heart rate value
            if ((flags & HEART_RATE_VALUE_FORMAT) != 0)
            {
                measurment.HeartRateValue = BitConverter.ToUInt16(data, currentOffset);// (ushort)((data[currentOffset + 1] << 8) + data[currentOffset]);
                currentOffset += 2;
            }
            else
            {
                measurment.HeartRateValue = data[currentOffset];
                currentOffset++;
            }

            // get expended energy
            if ((flags & ENERGY_EXPANDED_STATUS) != 0)
            {
                measurment.HasExpendedEnergy = true;
                measurment.ExpendedEnergy = (ushort)((data[currentOffset + 1] << 8) + data[currentOffset]);
                currentOffset += 2;
            }
            else
            {
                measurment.HasExpendedEnergy = false;
            }

            if ((flags & RR) != 0)
            {
                // R-R information
                int rrCount = (data.Length - currentOffset) / 2;
                measurment.RRValues = new int[rrCount];
                for (int i = 0; i < rrCount; i++)
                {
                    measurment.RRValues[i] = BitConverter.ToUInt16(data, currentOffset);
                    currentOffset += 2;
                }
            }

            // The Heart Rate Bluetooth profile can also contain sensor contact status information,
            // and R-Wave interval measurements, which can also be processed here. 
            // For the purpose of this sample, we don't need to interpret that data.

            return measurment;

        }
    }
}

