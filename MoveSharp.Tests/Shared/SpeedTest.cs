using MoveSharp.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Speed conversion tests
    /// </summary>
    [TestFixture]
    public class SpeedTest
    {
        /// <summary>
        /// Floating point error treshold
        /// </summary>
        private const double Error = 1e-1;

        [Test]
        public void ConvertKilometerPerHour()
        {
            Speed speed = new Speed { Unit = SpeedUnit.KilometerPerHour, Value = 1 };
            var mph = speed.GetValueAs(SpeedUnit.MilePerHour);
            var ms = speed.GetValueAs(SpeedUnit.MeterPerSecond);
            Assert.AreEqual(mph, 0.6214, Error);
            Assert.AreEqual(ms, 0.2778, Error);
        }

        [Test]
        public void ConvertMilePerHour()
        {
            Speed speed = new Speed { Unit = SpeedUnit.MilePerHour, Value = 1 };
            var kmh = speed.GetValueAs(SpeedUnit.KilometerPerHour);
            var ms = speed.GetValueAs(SpeedUnit.MeterPerSecond);
            Assert.AreEqual(kmh, 1.609, Error);
            Assert.AreEqual(ms, 0.447, Error);
        }


        [Test]
        public void ConvertMeterPerSecond()
        {
            Speed speed = new Speed { Unit = SpeedUnit.MeterPerSecond, Value = 128 };
            var kmh = speed.GetValueAs(SpeedUnit.KilometerPerHour);
            var mph = speed.GetValueAs(SpeedUnit.MilePerHour);
            Assert.AreEqual(460.8, kmh, Error);
            Assert.AreEqual(286.3, mph, Error);
        }

        [Test]
        public void ConvertPace()
        {
            Speed speed = new Speed { Unit = SpeedUnit.KilometerPerHour, Value = 15 };
            var pace = speed.GetValueAs(SpeedUnit.MinutePerKilometer);
            Assert.AreEqual(4.0f, pace);
        }
    }
}
