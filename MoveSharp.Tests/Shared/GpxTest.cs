//
// GpxTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using MoveSharp.Format;
using MoveSharp.Models;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MoveSharp.Tests.Helpers;

namespace MoveSharp.Tests
{
    [TestFixture("THNY_40.gpx")]
    [TestFixture("PolarPersonalTrainer.gpx")]
    public class GpxTest
    {
        private readonly string _fileName;

        public GpxTest(string fileName)
        {
            _fileName = Path.Combine(Settings.Instance.RootFolder, "Gpx", fileName);
        }

        private async Task<MemoryActivity> Import(string fileName)
        {
            using (var input = await TestFileHelper.OpenForReadAsync(fileName))
            {
                var activity = new MemoryActivity();
                var importer = new GpxImporter(activity);
                importer.Load(input);
                //var exporter = new FitExporter(activity);
                //using (var output = await TestFileHelper.OpenForWriteAsync(_fileName + ".fit"))
                //{
                //    exporter.Save(output);
                //}
                return activity;
            }
        }

        private string GetExportedFileName()
        {
            var folderName = Path.GetDirectoryName(_fileName) ?? "";
            var path = Path.Combine(folderName, Path.GetFileNameWithoutExtension(_fileName) + "_exported.gpx");
            return path;
        }

        private async Task Export(MemoryActivity activity)
        {
            using (var output = await TestFileHelper.OpenForWriteAsync(GetExportedFileName()))
            {
                var exporter = new GpxExporter(activity);
                await exporter.SaveAsync(output);
            }
        }

        [Test]
        public async Task GpxImport()
        {
            var activity = await Import(_fileName);
            Assert.NotNull(activity);
            Assert.IsTrue(activity.TimeFrames.Any());
        }

        [Test]
        public async Task GpxImportExport()
        {
            var activity = await Import(_fileName);
            Assert.NotNull(activity);
            Assert.IsTrue(activity.TimeFrames.Any());
            await Export(activity);
            var activityExported = await Import(GetExportedFileName());
            Assert.NotNull(activityExported);
            Assert.AreEqual(activity.TimeFrames.Count, activityExported.TimeFrames.Count);
        }
    }
}
