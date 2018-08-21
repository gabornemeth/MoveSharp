//
// FileLogListener.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using System;
using System.IO;
using MoveSharp.Tests.Helpers;
using XTools.Diagnostics;
using System.Threading.Tasks;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Writing log to a file.
    /// </summary>
    class FileLogListener : ILogListener
    {
        private string _fileName;
        private Stream _stream;
        private StreamWriter _writer;
        
        public FileLogListener(string fileName)
        {
            _fileName = fileName;
        }

        public async Task OpenAsync()
        {
            _stream = await TestFileHelper.OpenForWriteAsync(_fileName);
            _writer = new StreamWriter(_stream);
        }

        public void Write(LogLevel logLevel, string message)
        {
            if (_writer != null)
                _writer.WriteLine(message);
        }

        public void Write(Exception ex)
        {
            if (_writer != null)
                _writer.WriteLine(Log.FormatException(ex));
        }
    }
}
