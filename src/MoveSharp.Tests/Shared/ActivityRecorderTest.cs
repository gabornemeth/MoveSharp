using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MoveSharp.Format;
using MoveSharp.Models;
using MoveSharp.Tests.Helpers;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Test for activity recording
    /// </summary>
    [TestFixture]
    public class ActivityRecorderTest
    {
        private string GetFileName(string fileName)
        {
            return Path.Combine(Settings.Instance.RootFolder, "Fit", fileName);
        }

        [Test]
        public async Task Recode()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(GetFileName("2015-10-31-100331.fit")))
            {
                var importer = new FitImporter(activity);
                importer.Load(input);
            }

            var activityRepaired = activity.Repair();

            Assert.AreEqual(26.092411, activityRepaired.AvgSpeed.GetValueAs(SpeedUnit.KilometerPerHour), 0.05);
        }
    }
}
