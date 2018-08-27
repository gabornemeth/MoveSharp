//
// FormatTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using MoveSharp.Format;
using MoveSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace MoveSharp.Tests
{
    /// <summary>
    /// Base class for different file format tests
    /// Provides functionality to export and reimport FIT file and this activity can be checked
    /// </summary>
    public class FormatTest
    {
        protected MemoryActivity ExportAndReload(MemoryActivity activity)
        {
            var exporter = new FitExporter(activity);
            var stream = new MemoryStream();
            exporter.Save(stream);
            // reimport the activity
            stream.Seek(0, SeekOrigin.Begin);
            var activityReloaded = new MemoryActivity();
            var importer = new FitImporter(activityReloaded);
            importer.Load(stream);
            return importer.Activity;
        }
    }
}
