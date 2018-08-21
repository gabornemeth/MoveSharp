//
// CadenceData.cs
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
    /// Cadence data
    /// </summary>
    public struct Cadence
    {
        public static Cadence Empty
        {
            get
            {
                return new Cadence(0);
            }
        }

        /// <summary>
        /// Cadence value [rpm]
        /// </summary>
        public byte RevolutionsPerMinute { get; set; }

        public Cadence(byte cadence)
            : this()
        {
            RevolutionsPerMinute = cadence;
        }

        public bool HasValue
        {
            get
            {
                return RevolutionsPerMinute != 0;
            }
        }
    }
}
