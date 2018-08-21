using MoveSharp.Models;
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SharpGeo;

namespace MoveSharp.Format
{
    public class GpxImporter : ActivityImporter
    {
        public GpxImporter(MemoryActivity activity) : base(activity)
        {
        }

        public override void Load(Stream source)
        {
            var serializer = new XmlSerializer(typeof(GpxExercise));
            var exercise = serializer.Deserialize(source) as GpxExercise;
            if (exercise == null)
                return;

            foreach (var segment in exercise.Track.Segments)
            {
                foreach (var trackPoint in segment.Trackpoints)
                {
                    var timeFrame = new ActivityTimeFrame();
                    timeFrame.Timestamp = trackPoint.Time;
                    timeFrame.Position = new Position((float)trackPoint.Longitude, (float)trackPoint.Latitude, (float)trackPoint.Elevation);
                    Activity.TimeFrames.Add(timeFrame);
                }
            }
        }
    }
}
