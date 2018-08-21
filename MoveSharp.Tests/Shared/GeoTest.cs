//
// GeoTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using NUnit.Framework;
using MoveSharp.Geolocation;
using SharpGeo;

namespace MoveSharp.Tests
{
    [TestFixture]
    public class GeoTest
    {
        [Test]
        public void Distance()
        {
            var pos1 = new Position(46.83749695f, 16.84976191f);
            var pos2 = new Position(46.83749276f, 16.84976201f);

            var distance = GeoHelper.Distance(pos1, pos2);
            Assert.AreEqual(0.4, distance, 0.01);
        }
    }
}
