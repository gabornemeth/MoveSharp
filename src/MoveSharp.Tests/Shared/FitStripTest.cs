//
// FitStripTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.IO;
using System.Threading.Tasks;
using MoveSharp.Strippers;
using MoveSharp.Tests.Helpers;
using NUnit.Framework;
using MoveSharp.Format;
using MoveSharp.Models;
using XTools.Diagnostics;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Test for stripping FIT files
    /// </summary>
    [TestFixture("Data/Fit/2014-09-21-07-13-00.fit")]
    [TestFixture("Data/Fit/2014-03-21-14-44-56.fit")]
    public class FitStripTest : FileTest
    {
        public FitStripTest(string fileName)
            : base(fileName)
        {
        }

        //[TestFixtureSetUp]
        //public void SetupFixture()
        //{
        //    XDotNet.Diagnostics.Log.Listeners.Add(new TestLogListener());
        //}

        //[TestFixtureTearDown]
        //public void TeardownFixture()
        //{
        //    XDotNet.Diagnostics.Log.Listeners.Clear();
        //}

        private string GetStrippedFileName(string fileName, string postfix = null)
        {
            return Path.Combine(Path.GetDirectoryName(fileName) ?? "", Path.GetFileNameWithoutExtension(fileName) + (postfix ?? "_stripped" + Environment.TickCount) + ".fit");
        }

        /// <summary>
        /// Loading a FIT file
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        /// <returns>Loaded activity.</returns>
        private Task<MemoryActivity> LoadAsync(string fileName)
        {
            return LoadAsync(fileName, activity => new FitImporter(activity));
        }

        private async Task<Tuple<MemoryActivity, MemoryActivity>> StripAsync(StripOptions options, bool log = false)
        {
            var strippedFileName = GetStrippedFileName(FileName);

            using (var input = await TestFileHelper.OpenForReadAsync(FileName))
            {
                var stripper = new FitStripper { LogEnabled = log };
                using (var output = await TestFileHelper.OpenForWriteAsync(strippedFileName))
                {
                    stripper.Strip(input, output, options);
                }
            }
            var activity = await LoadAsync(FileName);
            var strippedActivity = await LoadAsync(strippedFileName);
            return new Tuple<MemoryActivity, MemoryActivity>(activity, strippedActivity);
        }

        /// <summary>
        /// Checks for common properties
        /// </summary>
        /// <param name="activity">Original activity.</param>
        /// <param name="strippedActivity">Stripped activity.</param>
        private void Check(MemoryActivity activity, MemoryActivity strippedActivity)
        {
            Assert.AreEqual(activity.Sport, strippedActivity.Sport);
            Assert.AreEqual(activity.Distance, strippedActivity.Distance);
            Assert.AreEqual(activity.ElapsedTime, strippedActivity.ElapsedTime);
        }

        public async Task Dump()
        {
            var logListener = new FileLogListener(GetStrippedFileName(FileName, "_dump"));
            try
            {
                await logListener.OpenAsync();
                Log.Listeners.Add(logListener);
                await StripAsync(StripOptions.None, true);
            }
            finally
            {
                Log.Listeners.Remove(logListener);
            }
        }

        /// <summary>
        /// Decode then encode again
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Copy()
        {
            var result = await StripAsync(StripOptions.None);
            var activity = result.Item1;
            var strippedActivity = result.Item2;

            Check(activity, strippedActivity);
            Assert.AreEqual(activity.AvgHeartRate, strippedActivity.AvgHeartRate);
            Assert.AreEqual(activity.MaxHeartRate, strippedActivity.MaxHeartRate);
            Assert.AreEqual(activity.AvgSpeed, strippedActivity.AvgSpeed);
            Assert.AreEqual(activity.MaxSpeed, strippedActivity.MaxSpeed);
            Assert.AreEqual(activity.AvgPower, strippedActivity.AvgPower);
            Assert.AreEqual(activity.MaxPower, strippedActivity.MaxPower);
        }

        /// <summary>
        /// Stripping heart rate data
        /// </summary>
        [Test]
        public async Task StripHeartRate()
        {
            var result = await StripAsync(StripOptions.HeartRate);
            var activity = result.Item1;
            var strippedActivity = result.Item2;

            Check(activity, strippedActivity);
            Assert.AreEqual(0, strippedActivity.AvgHeartRate);
            Assert.AreEqual(0, strippedActivity.MaxHeartRate);
            Assert.AreEqual(activity.AvgSpeed, strippedActivity.AvgSpeed);
            Assert.AreEqual(activity.MaxSpeed, strippedActivity.MaxSpeed);
            Assert.AreEqual(activity.AvgPower, strippedActivity.AvgPower);
            Assert.AreEqual(activity.MaxPower, strippedActivity.MaxPower);
        }

        /// <summary>
        /// Stripping heart rate data
        /// </summary>
        [Test]
        public async Task StripPower()
        {
            var result = await StripAsync(StripOptions.Power);
            var activity = result.Item1;
            var strippedActivity = result.Item2;

            Check(activity, strippedActivity);
            Assert.AreEqual(activity.AvgHeartRate, strippedActivity.AvgHeartRate);
            Assert.AreEqual(activity.MaxHeartRate, strippedActivity.MaxHeartRate);
            Assert.AreEqual(activity.AvgSpeed, strippedActivity.AvgSpeed);
            Assert.AreEqual(activity.MaxSpeed, strippedActivity.MaxSpeed);
            Assert.AreEqual(0, strippedActivity.AvgPower);
            Assert.AreEqual(0, strippedActivity.MaxPower);
        }
    }
}
