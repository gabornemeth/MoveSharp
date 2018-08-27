//
// Power.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//
        
using System;
using System.Collections.Generic;
using System.Text;

namespace MoveSharp.Sensors
{
    /// <summary>
    /// Power data
    /// </summary>
    public struct Power
    {
        private static Power _none = new Power(-1);

        public static Power None
        {
            get
            {
                return _none;
            }
        }

        /// <summary>
        /// Power [watts]
        /// </summary>
        public short Watts { get; set; }

        public bool HasValue
        {
            get
            {
                return Watts != -1;
            }
        }

        public Power(short watts)
            : this()
        {
            Watts = watts;
        }

        public bool Equals(Power other)
        {
            return Watts == other.Watts;
        }

        public static bool operator ==(Power p1, Power p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Power p1, Power p2)
        {
            return !p1.Equals(p2);
        }
    }
}
