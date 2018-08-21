//
// Setup.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using NUnit.Framework;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MoveSharp.Storage;
using ICSharpCode.SharpZipLib.Zip;
using MoveSharp.Tests.Helpers;

namespace MoveSharp.Tests
{
    [SetUpFixture]
    public class Setup
    {
        private const string TestDataUrl = "http://gabornemeth.asphostpage.net/Test/Data.zip";
        private const string TestZipName = "Data.zip";

        [OneTimeSetUp]
        public void SetupTest()
        {
            try
            {
                var settings = Settings.Instance;

                var task = Task.Run(async () =>
                {
                    if (!await IsTestDataDownloaded())
                    {
                        await DownloadTestData();
                    }
                });
                task.Wait();
            }
            catch (AggregateException aggregateException)
            {
                throw aggregateException.InnerException;
            }
        }

        private async Task<ILocalFolder> GetFolder()
        {
            var folder = await TestFileHelper.Storage.GetFolderAsync("Data") ?? await TestFileHelper.Storage.CreateFolderAsync("Data");
            return folder;
        }

        /// <summary>
        /// Download test data
        /// </summary>
        /// <returns></returns>
        private async Task DownloadTestData()
        {
            var client = new HttpClient();

            var response = await client.GetAsync(TestDataUrl);
            if (response.IsSuccessStatusCode)
            {
                using (var streamDownloaded = await response.Content.ReadAsStreamAsync())
                {
                    var folderRoot = await GetFolder();
                    var file = await folderRoot.CreateFileAsync(TestZipName);
                    using (var streamDest = await file.OpenForWriteAsync())
                    {
                        await streamDownloaded.CopyToAsync(streamDest);
                        streamDest.Seek(0, SeekOrigin.Begin);
                        await ExtractTestData(streamDest);
                    }
                }
            }
        }

        private async Task ExtractTestData(Stream stream = null)
        {
            var folderRoot = await GetFolder();
            if (stream == null)
            {
                // No stream specified, open the downloaded zip file
                var file = await folderRoot.GetFileAsync(TestZipName);
                stream = await file.OpenForReadAsync();
            }
            var zip = new ZipFile(stream);
            foreach (ZipEntry zipEntry in zip)
            {
                var streamZipEntry = zip.GetInputStream(zipEntry);
                if (zipEntry.IsDirectory)
                    await folderRoot.CreateFolderAsync(zipEntry.Name);
                else if (zipEntry.IsFile)
                {
                    var outputFile = await folderRoot.CreateFileAsync(zipEntry.Name);
                    using (var outputStream = await outputFile.OpenForWriteAsync())
                    {
                        await streamZipEntry.CopyToAsync(outputStream);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the test data has already been downloaded
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsTestDataDownloaded()
        {
            var folderRoot = await GetFolder();
            if (folderRoot == null)
                return false;

            var file = await folderRoot.GetFileAsync(TestZipName);
            return file != null;
        }
    }
}
