//
// ILap.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using System.Collections.Generic;

namespace MoveSharp.Models
{
    /// <summary>
    /// Summary information about an activity
    /// </summary>
    public interface ILap : ILapSummary
    {
        /// <summary>
        /// Each frame
        /// </summary>
        IList<ActivityTimeFrame> TimeFrames { get; }

        event EventHandler<ActivityTimeFrame> FrameAdded;
        event EventHandler Reseted;
    }
}
