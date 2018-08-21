//
// ICadenceSensor.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Interface has to be implemented by cadence sensors.
    /// </summary>
    public interface ICadenceSensor : ISensor
    {
        Cadence Cadence { get; }
    }
}
