//
// FitTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using Dynastream.Fit;
using MoveSharp.Extensions;
using MoveSharp.Format;
using MoveSharp.Models;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using MoveSharp.Tests.Helpers;
using XTools.Diagnostics;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Tests with FIT file format
    /// </summary>
    [TestFixture]
    public class FitTest
    {
        internal static readonly string Test1FileName = Path.Combine("Data", "Fit", "2014-09-21-07-13-00.fit");
        private readonly string Test2FileName = Path.Combine("Data", "Fit", "2014-03-21-14-44-56.fit");
        private readonly string CyclingHrPowerFileName = Path.Combine("Data", "Fit", "cycling_hr_power.fit");
        private readonly string CyclingHrFileName = Path.Combine("Data", "Fit", "cycling_hr.fit");
        private readonly string CyclingPowerFileName = Path.Combine("Data", "Fit", "cycling_power.fit");
        private readonly string DamagedFileName = Path.Combine("Data", "activity.rec");
        private readonly string BadFileName = Path.Combine("Data", "Fit", "2015-06-25-175214.fit");
        private readonly string MovescountRunningFileName = Path.Combine("Data", "Fit", "Move_2015_12_11_16_26_41_Running.fit");
        private readonly string MovescountCyclingFileName = Path.Combine("Data", "Fit", "Move_2016_04_14_17_39_38_Cycling.fit");
        private readonly string MovescountCycling2FileName = Path.Combine("Data", "Fit", "Move_2016_04_16_13_47_40_Cycling.fit");
        private readonly string CorruptTimeFileName = Path.Combine("Data", "Fit", "2016-09-20-14-30-34_invalid_time.fit");
        private readonly string GarminConnectRunningFileName = Path.Combine("Data", "Fit", "2356405020.fit");

        [OneTimeSetUp]
        public void SetupFixture()
        {
            Log.Listeners.Add(new TestLogListener());
        }

        [OneTimeTearDown]
        public void TeardownFixture()
        {
            Log.Listeners.Clear();
        }

        [Test]
        public async Task Import()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(Test2FileName))
            {
                var importer = new FitImporter(activity);
                importer.MessageBroadcaster.SessionMesgEvent += (sender, args) =>
                    {
                        var msg = args.mesg as SessionMesg;
                        var summary = msg.ToSummary();
                    };
                importer.Load(input);
                Assert.AreEqual(activity.AvgHeartRate, 153);
                Assert.AreEqual(activity.MaxHeartRate, 180);
            }
        }

        [Test]
        public async Task DamagedFile()
        {
            var activity = await LoadAsync(DamagedFileName);
            Assert.AreEqual(4 * 60 + 36, activity.MovingTime, 3);
            Assert.AreEqual(2, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.02f);
            //Assert.AreEqual(153, activity.AvgHeartRate);
        }

        [Test]
        public async Task BadFile()
        {
            var activity = await LoadAsync(BadFileName);
        }

        [Test]
        public async Task FixCorruptTime()
        {
            var activity = await LoadAsync(CorruptTimeFileName);
            var timeValid = new System.DateTime(2016, 09, 20, 14, 30, 34);
            var seconds = 0;
            var summary = new ActivitySummary { StartTime = timeValid };
            activity.SetSummary(summary);
            foreach (var timeFrame in activity.TimeFrames)
            {
                timeFrame.Timestamp = timeValid.AddSeconds(seconds++);
            }
            await SaveAsync(activity, CorruptTimeFileName + ".fixed");
            Assert.AreEqual(timeValid, activity.StartTime);
        }

        [Test]
        public async Task MovescountRunning()
        {
            var activity = await LoadAsync(MovescountRunningFileName);
            //var idx = 0;
            //foreach (var timeFrame in activity.TimeFrames)
            //{
            //    if (timeFrame.Speed.HasValue)
            //        Debug.WriteLine($"{idx}. speed: {timeFrame.Speed.Value.Value}");
            //    idx++;
            //}
            Assert.AreEqual(Sport.Running, activity.Sport);
            Assert.AreEqual(140, activity.AvgHeartRate);
            Assert.AreEqual(5 + 43.0 / 60, activity.AvgSpeed.GetValueAs(SpeedUnit.MinutePerKilometer), 0.1);
            Assert.AreEqual(11.77, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1);
            //Assert.AreEqual(151, activity.Ascent, 5);
        }

        [Test]
        public async Task MovescountCycling()
        {
            var activity = await LoadAsync(MovescountCycling2FileName);
            Assert.AreEqual(Sport.Cycling, activity.Sport);
            Assert.AreEqual(51.59, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1);
            Assert.AreEqual(147, activity.AvgHeartRate);
            Assert.AreEqual(17.4, activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.2);
            // 2017.08.31 - Movescount FIT files does not have session message about totals
            // Ascent calculation is different from ours
            //Assert.AreEqual(1215, activity.Ascent, 5);
        }

        [Test]
        public async Task GarminConnectRunning() {
            var activity = await LoadAsync(GarminConnectRunningFileName);
            Assert.AreEqual(Sport.Running, activity.Sport);
            //Assert.AreEqual(51.59, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1);
            //Assert.AreEqual(147, activity.AvgHeartRate);
            //Assert.AreEqual(17.4, activity.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.2);
        }

        private async Task<MemoryActivity> LoadAsync(string fileName)
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(fileName))
            {
                var importer = new FitImporter(activity);
                await importer.LoadAsync(input);
            }

            return activity;
        }

        private async Task SaveAsync(MemoryActivity activity, string fileName)
        {
            using (var output = await TestFileHelper.OpenForWriteAsync(fileName))
            {
                var exporter = new FitExporter(activity);
                await exporter.SaveAsync(output);
            }
        }
    }
}
