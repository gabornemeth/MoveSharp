//
// FileTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using System.IO;
using System.Threading.Tasks;
using MoveSharp.Format;
using MoveSharp.Models;
using MoveSharp.Tests.Helpers;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Base class for tests working with files as activity
    /// </summary>
    public class FileTest
    {
        protected string FileName { get; private set; }

        public FileTest(string fileName)
        {
            FileName = fileName.Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Loads an <see cref="MemoryActivity"/> from a file
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        /// <param name="funcImporter">The importer to be created for loading the activity.</param>
        /// <returns>The imported <see cref="MemoryActivity"/>.</returns>
        protected async Task<MemoryActivity> LoadAsync(string fileName, Func<MemoryActivity, ActivityImporter> funcImporter)
        {
            var activity = new MemoryActivity();
            using (var input = await TestFileHelper.OpenForReadAsync(fileName))
            {
                var importer = funcImporter(activity);
                importer.Load(input);
            }

            return activity;
        }
    }
}
