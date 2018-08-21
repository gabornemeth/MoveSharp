//
// GpxExporter.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using MoveSharp.Models;
using System.Xml.Serialization;

namespace MoveSharp.Format
{
    public class GpxExporter : ActivityExporter
    {
        public GpxExporter(MemoryActivity activity)
            : base(activity)
        {
        }

        public override void Save(System.IO.Stream dest)
        {
            var exercise = new GpxExercise();
            var segment = new GpxTrackSegment();
            exercise.Track.Segments = new[] { segment };
            foreach (var frame in Activity.TimeFrames)
            {
                if (!frame.Position.HasValue)
                    continue;

                var trackPoint = new GpxTrackpoint
                {
                    Latitude = frame.Position.Value.Latitude,
                    Longitude = frame.Position.Value.Longitude,
                    Elevation = frame.Position.Value.Altitude,
                    Time = frame.Timestamp
                };
                // correct altitude if present
                if (frame.Altitude.HasValue)
                    trackPoint.Elevation = frame.Altitude.Value.GetValueAs(DistanceUnit.Meter);
                segment.Trackpoints.Add(trackPoint);
            }

            var serializer = new XmlSerializer(typeof(GpxExercise));
            serializer.Serialize(dest, exercise);
        }
    }
}
