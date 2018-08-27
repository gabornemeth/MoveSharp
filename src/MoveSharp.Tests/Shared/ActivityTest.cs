//
// ActivityTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using NUnit.Framework;
using System.Threading.Tasks;
using MoveSharp.Format;
using MoveSharp.Models;

namespace MoveSharp.Tests
{
    [TestFixture("Data/Fit/2014-09-21-07-13-00.fit")]
    public class ActivityTest : FileTest
    {
        public ActivityTest(string fileName) : base(fileName)
        {
        }

        [Test]
        public async Task NewLapDoesNotTouchDistance()
        {
            var src = await LoadAsync(FileName, a => new FitImporter(a));

            var activity = new RecordingActivity();
            //var a = new ActivityViewModel(activity, Platform.Current.ServiceLocator.Settings);
            var i = 1;
            foreach (var frame in src.TimeFrames)
            {
                frame.Distance = null;
                var dist = activity.Distance.Value;
                activity.AddTimeFrame(frame);
                Assert.IsTrue(activity.Distance.Value >= dist);
                if (i++ % 200 == 0)
                {
                    activity.NewLap();
                }
            }
        }
    }
}
