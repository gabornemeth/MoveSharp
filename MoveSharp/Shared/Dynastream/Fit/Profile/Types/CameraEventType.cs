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
    /// Implements the profile CameraEventType type as an enum
    /// </summary>
    public enum CameraEventType : byte
    {
        VideoStart = 0,
        VideoSplit = 1,
        VideoEnd = 2,
        PhotoTaken = 3,
        VideoSecondStreamStart = 4,
        VideoSecondStreamSplit = 5,
        VideoSecondStreamEnd = 6,
        VideoSplitStart = 7,
        VideoSecondStreamSplitStart = 8,
        VideoPause = 11,
        VideoSecondStreamPause = 12,
        VideoResume = 13,
        VideoSecondStreamResume = 14,
        Invalid = 0xFF


    }
}

