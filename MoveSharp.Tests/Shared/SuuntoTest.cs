using MoveSharp.Format;
using MoveSharp.Models;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using MoveSharp.Tests.Helpers;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Tests with TCX file format
    /// </summary>
    [TestFixture("2017-03-05T10_03_31-0.sml")]
    public class SuuntoTest
    {
        private readonly string _fileName;

        public SuuntoTest(string fileName)
        {
            _fileName = Path.Combine(Settings.Instance.RootFolder, "Sml", fileName);
        }

        [Test]
        public async Task SmlImport()
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(_fileName))
            {
                var importer = new SmlImporter(activity);
                await importer.LoadAsync(input);
            }

            Assert.True(activity.TimeFrames.Count > 0);
            Assert.AreEqual(70.263f, activity.Distance.GetValueAs(DistanceUnit.Kilometer), 0.1f);
        }
    }
}
