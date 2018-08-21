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
    /// Implements the profile WorkoutCapabilities type as a class
    /// </summary>
    public static class WorkoutCapabilities 
    {
        public const uint Interval = 0x00000001;
        public const uint Custom = 0x00000002;
        public const uint FitnessEquipment = 0x00000004;
        public const uint Firstbeat = 0x00000008;
        public const uint NewLeaf = 0x00000010;
        public const uint Tcx = 0x00000020; // For backwards compatibility.  Watch should add missing id fields then clear flag.
        public const uint Speed = 0x00000080; // Speed source required for workout step.
        public const uint HeartRate = 0x00000100; // Heart rate source required for workout step.
        public const uint Distance = 0x00000200; // Distance source required for workout step.
        public const uint Cadence = 0x00000400; // Cadence source required for workout step.
        public const uint Power = 0x00000800; // Power source required for workout step.
        public const uint Grade = 0x00001000; // Grade source required for workout step.
        public const uint Resistance = 0x00002000; // Resistance source required for workout step.
        public const uint Protected = 0x00004000;
        public const uint Invalid = (uint)0x00000000;


    }
}

