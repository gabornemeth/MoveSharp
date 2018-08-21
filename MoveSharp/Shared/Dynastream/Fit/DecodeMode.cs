#region Copyright
////////////////////////////////////////////////////////////////////////////////
// The following FIT Protocol software provided may be used with FIT protocol
// devices only and remains the copyrighted property of Dynastream Innovations Inc.
// The software is being provided on an "as-is" basis and as an accommodation,
// and therefore all warranties, representations, or guarantees of any kind
// (whether express, implied or statutory) including, without limitation,
// warranties of merchantability, non-infringement, or fitness for a particular
// purpose, are specifically disclaimed.
//
// Copyright 2017 Dynastream Innovations Inc.
////////////////////////////////////////////////////////////////////////////////
// ****WARNING****  This file is auto-generated!  Do NOT edit this file.
// Profile Version = 20.43Release
// Tag = production/akw/20.43.00-0-gc29a67f
////////////////////////////////////////////////////////////////////////////////

#endregion

namespace Dynastream.Fit
{
    /// <summary>
    /// Mode used for Read Operations
    /// </summary>
    public enum DecodeMode
    {
        /// <summary>
        /// Indicates that file contains valid Header and CRC data
        /// </summary>
        Normal,

        /// <summary>
        /// Indicates that the Stream Contains a Header that is Corrupt
        /// </summary>
        InvalidHeader,

        /// <summary>
        /// Indicates that the Stream does not contain a Header or CRC
        /// </summary>
        DataOnly
    }
}
