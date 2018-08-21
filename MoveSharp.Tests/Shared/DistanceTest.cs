//
// DistanceTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Models;
using NUnit.Framework;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Distance conversion tests
    /// </summary>
    [TestFixture]
    public class DistanceTest
    {
        /// <summary>
        /// Floating point error treshold
        /// </summary>
        private const double Error = 1e-3;

        [Test]
        public void ConvertKilometer()
        {
            Distance dist = new Distance { Unit = DistanceUnit.Kilometer, Value = 1 };
            var distInMeters = dist.GetValueAs(DistanceUnit.Meter);
            var distInMiles = dist.GetValueAs(DistanceUnit.Mile);
            var distInYards = dist.GetValueAs(DistanceUnit.Yard);
            var distInFoot = dist.GetValueAs(DistanceUnit.Foot);
            Assert.AreEqual(1000.0f, distInMeters, Error);
            Assert.AreEqual(0.6214f, distInMiles, Error);
            Assert.AreEqual(1094f, distInYards, 1);
            Assert.AreEqual(3281, distInFoot, 1);
        }
    }
}
