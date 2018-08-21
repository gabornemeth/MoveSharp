using System;
using NUnit.Framework;

namespace MoveSharp.Tests
{
    [TestFixture]
    public class DateTimeParseTest
    {
        [Test]
        public void ParseDateTime()
        {
            DateTime dateTime;
            Assert.True("2007-11-12-03-24-12.fit".TryParseAsDateTime(out dateTime));
            Assert.AreEqual(new DateTime(2007, 11, 12, 3, 24, 12), dateTime);
        }

        [Test]
        public void ParseDateTime24h()
        {
            DateTime dateTime;
            Assert.True("2007-11-12-18-24-12.fit".TryParseAsDateTime(out dateTime));
            Assert.AreEqual(new DateTime(2007, 11, 12, 18, 24, 12), dateTime);
        }

        [Test]
        public void ParseInvalidDateTime()
        {
            DateTime dateTime;
            Assert.False("2007-19-12-18-24-12.fit".TryParseAsDateTime(out dateTime));
        }

        [Test]
        public void ParseUnknownFormat()
        {
            DateTime dateTime;
            Assert.False("2007/10/12-18-24-12.fit".TryParseAsDateTime(out dateTime));
        }

        [Test]
        public void ParseNoDateTime()
        {
            DateTime dateTime;
            Assert.False("not datetime.fit".TryParseAsDateTime(out dateTime));
        }
    }
}
