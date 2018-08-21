//
// TestLogListener.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using XTools.Diagnostics;
using System;
using System.Diagnostics;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Logger Trace listener for tests
    /// </summary>
    class TestLogListener : ILogListener
    {
        public void Write(LogLevel logLevel, string message)
        {
            Debug.WriteLine(message);
        }

        public void Write(Exception ex)
        {
            Debug.WriteLine(Log.FormatException(ex));
        }
    }
}
