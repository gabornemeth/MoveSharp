//
// IBikePowerMeter.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Interface of bike power meter
    /// </summary>
    public interface IBikePowerMeter : ISensor
    {
        /// <summary>
        /// Power
        /// </summary>
        /// <value>The current power value.</value>
        Power Power { get; }
    }
}
