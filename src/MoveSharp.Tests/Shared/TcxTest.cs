//
// TcxTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Format;
using MoveSharp.Models;
using MoveSharp.Strippers;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MoveSharp.Tests.Helpers;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Tests with TCX file format
    /// </summary>
    [TestFixture("2014-04-05-165650.TCX")]
    public class TcxTest
    {
        private readonly string _fileName;

        public TcxTest(string fileName)
        {
            _fileName = Path.Combine(Settings.Instance.RootFolder, "Tcx", fileName);
        }

        private string GetStrippedFileName(string fileName, string postfix = null)
        {
            return Path.Combine(Path.GetDirectoryName(fileName) ?? "", Path.GetFileNameWithoutExtension(fileName) + (postfix ?? "_stripped") + ".tcx");
        }

        [Test]
        public async Task TcxCopy()
        {
            using (var input = await TestFileHelper.OpenForReadAsync(_fileName))
            {
                var stripper = new TcxStripper();
                using (var output = await TestFileHelper.OpenForWriteAsync(GetStrippedFileName(_fileName)))
                {
                    stripper.Strip(input, output, StripOptions.None);
                    output.Seek(0, SeekOrigin.Begin);

                    var doc = XElement.Load(output);
                    Assert.IsTrue(doc.Nodes().Any());
                }
            }
        }

        [Test]
        public async Task TcxStripHeartRate()
        {
            using (var input = await TestFileHelper.OpenForReadAsync(_fileName))
            {
                var stripper = new TcxStripper();
                using (var output = await TestFileHelper.OpenForWriteAsync(GetStrippedFileName(_fileName, "_stripped_hr")))
                {
                    stripper.Strip(input, output, StripOptions.HeartRate);
                    Assert.True(output.Length > 0);
                }
            }
        }

        /// <summary>
        /// Importing TCX file
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TcxImport()
        {
            using (var input = await TestFileHelper.OpenForReadAsync(_fileName))
            {
                var activity = new MemoryActivity();
                var importer = new TcxImporter(activity);
                importer.Load(input);
                Assert.IsTrue(activity.TimeFrames.Count > 0);
            }
        }
    }
}
