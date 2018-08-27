using MoveSharp.Geolocation;
using NUnit.Framework;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Ascent calculation tests
    /// </summary>
    [TestFixture]
    public class AscentCalculatorTest
    {
        private AscentCalculator _calculator = new AscentCalculator();

        [Test]
        public void AscentRollingHills()
        {
            _calculator.Reset();
            _calculator.AddAltitude(140);
            _calculator.AddAltitude(149);
            _calculator.AddAltitude(123);
            _calculator.AddAltitude(127);
            _calculator.AddAltitude(128);
            _calculator.AddAltitude(110);
            Assert.True(_calculator.HasTresholdReached);
            Assert.AreEqual(14, _calculator.Ascent);
        }

        [Test]
        public void AscentUphill()
        {
            _calculator.Reset();
            _calculator.AddAltitude(140);
            _calculator.AddAltitude(149);
            _calculator.AddAltitude(165);
            _calculator.AddAltitude(172);
            _calculator.AddAltitude(172);
            _calculator.AddAltitude(178);
            Assert.True(_calculator.HasTresholdReached);
            Assert.AreEqual(38, _calculator.Ascent);
        }

    }
}
