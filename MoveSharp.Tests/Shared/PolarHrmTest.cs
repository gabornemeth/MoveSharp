using MoveSharp.Format;
using MoveSharp.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynastream.Fit;
using MoveSharp.Helpers;
using MoveSharp.Tests.Helpers;
using DateTime = System.DateTime;

namespace MoveSharp.Tests
{
    [TestFixture]
    public class PolarHrmTest : FormatTest
    {
        private readonly string Hrm106Cycling = Path.Combine(Settings.Instance.RootFolder, "Hrm", "09052101.hrm");
        private readonly string Hrm2FileName = Path.Combine(Settings.Instance.RootFolder, "Hrm", "09050901.hrm");
        private readonly string Hrm106CyclingCadence = Path.Combine(Settings.Instance.RootFolder, "Hrm", "06052801.hrm");
        private readonly string Hrm106Running = Path.Combine(Settings.Instance.RootFolder, "Hrm", "07123101.hrm");
        private readonly string Hrm106CyclingPower = Path.Combine(Settings.Instance.RootFolder, "Hrm", "09010301.hrm");

        private void ExportReloadCheck(MemoryActivity activity)
        {
            var activityResult = ExportAndReload(activity);
            Assert.AreEqual(activity.StartTime, activityResult.StartTime);
            Assert.AreEqual(activity.ElapsedTime, activityResult.ElapsedTime);
            Assert.AreEqual(activity.Distance, activityResult.Distance);
            Assert.AreEqual(activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), activityResult.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
        }

        [Test]
        public async Task ImportCycling()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(Hrm106Cycling))
            {
                var importer = new HrmImporter(activity);
                importer.Load(input);
                Assert.AreEqual(Dynastream.Fit.Sport.Cycling, activity.Sport);
                Assert.AreEqual(new DateTime(2009, 05, 21, 06, 58, 41), activity.StartTime);
                Assert.AreNotEqual(0, activity.TimeFrames.Count);
                Assert.AreEqual(new TimeSpan(11, 59, 41).TotalSeconds, activity.ElapsedTime, 30); // treshold = 2 x recording interval
                Assert.AreEqual(239.1f, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1f);
                Assert.AreEqual(20.1f, activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                Assert.AreEqual(53.8f, activity.MaxSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                Assert.AreEqual(477, activity.TimeFrames[4].Altitude.Value.GetValueAs(DistanceUnit.Meter));
            }
            // now check exporting it
            ExportReloadCheck(activity);
        }

        [Test]
        public async Task ImportCyclingCadence()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(Hrm106CyclingCadence))
            {
                var importer = new HrmImporter(activity);
                importer.Load(input);
                Assert.AreEqual(Dynastream.Fit.Sport.Cycling, activity.Sport);
                Assert.AreEqual(new DateTime(2006, 05, 28, 13, 44, 57), activity.StartTime);
                Assert.AreNotEqual(0, activity.TimeFrames.Count);
                Assert.AreEqual(new TimeSpan(05, 09, 16).TotalSeconds, activity.ElapsedTime, 30); // treshold = 2 x recording interval
                Assert.AreEqual(135.6f, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1f);
                Assert.AreEqual(26.9f, activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                Assert.AreEqual(56.9f, activity.MaxSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                Assert.AreEqual(94, activity.TimeFrames[3].Cadence.Value);
                Assert.AreEqual(191, activity.TimeFrames[3].Altitude.Value.GetValueAs(DistanceUnit.Meter));
            }
            // now check exporting it
            ExportReloadCheck(activity);
        }

        [Test]
        public async Task ImportRunning()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(Hrm106Running))
            {
                var importer = new HrmImporter(activity);
                importer.Load(input);
                Assert.AreEqual(Dynastream.Fit.Sport.Running, activity.Sport);
                Assert.AreEqual(new DateTime(2007, 12, 31, 09, 05, 29), activity.StartTime);
                Assert.AreNotEqual(0, activity.TimeFrames.Count);
                Assert.AreEqual(new TimeSpan(03, 58, 59).TotalSeconds, activity.ElapsedTime, 30); // treshold = 2 x recording interval
                //Assert.AreEqual(135.6f, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1f);
                //Assert.AreEqual(26.9f, activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                //Assert.AreEqual(56.9f, activity.MaxSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                //Assert.AreEqual(94, activity.TimeFrames[3].Cadence.Value);
                //Assert.AreEqual(191, activity.TimeFrames[3].Altitude.Value.GetValueAs(DistanceUnit.Meter));
            }
            // now check exporting it
            ExportReloadCheck(activity);
        }


        [Test]
        public async Task ImportCyclingPower()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(Hrm106CyclingPower))
            {
                var importer = new HrmImporter(activity);
                importer.Load(input);
                Assert.AreEqual(Dynastream.Fit.Sport.Cycling, activity.Sport);
                Assert.AreEqual(new DateTime(2009, 01, 03, 11, 54, 23), activity.StartTime);
                Assert.AreNotEqual(0, activity.TimeFrames.Count);
                Assert.AreEqual(new TimeSpan(02, 43, 30).TotalSeconds, activity.ElapsedTime, 30); // treshold = 2 x recording interval
                Assert.AreEqual(66.8f, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1f);
                Assert.AreEqual(25.3f, activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                Assert.AreEqual(51.8f, activity.MaxSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.1f);
                Assert.True(activity.TimeFrames.Count > 3);
                Assert.AreEqual(66, activity.TimeFrames[3].Cadence.Value);
                Assert.AreEqual(174, activity.TimeFrames[3].Altitude.Value.GetValueAs(DistanceUnit.Meter));
                Assert.AreEqual(16, activity.TimeFrames[3].Power.Value);
            }
            // now check exporting it
            ExportReloadCheck(activity);
        }

    }
}
