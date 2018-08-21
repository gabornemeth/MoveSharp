//
// BluetoothTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Sensors.Bluetooth;
using NUnit.Framework;

namespace MoveSharp.Tests
{
    [TestFixture]
    public class BluetoothTest
    {
        [Test]
        public void Guid()
        {
            var guid = GattServiceGuids.HeartRate;
            //Assert.AreEqual(guidAsString, guid.ToString());
        }
    }
}
