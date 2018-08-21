//
// SmlImporter.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Helpers;
using MoveSharp.Models;
using SharpGeo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XTools.Math;

namespace MoveSharp.Format
{
    public class SmlImporter : ActivityImporter
    {
        public enum SmlNodeType
        {
            None,
            /// <summary>
            /// Sample
            /// </summary>
            Sample,
            Time,
            /// <summary>
            /// R-R data
            /// </summary>
            RR,
        }

        private ActivityTimeFrame _frame;

        public SmlImporter(MemoryActivity activity) : base(activity)
        {
        }

        private void BeginNewFrame()
        {
            _frame = new ActivityTimeFrame();
        }

        private DateTime RemoveMilliseconds(DateTime dateTime)
        {
            return dateTime.AddMilliseconds(dateTime.Millisecond);
        }

        public override void Load(Stream source)
        {
            var node = SmlNodeType.None;
            var frames = new List<ActivityTimeFrame>();
            var summary = new ActivitySummary();
            // SML files can be quite big (10 megabytes), so just read it sequentially instead of loading the whole file into memory
            using (XmlReader reader = XmlReader.Create(source))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Header")
                            {
                                var xHeader = reader.ReadAsXElement();
                                Activity.StartTime = xHeader.GetFirstDescendantValue<DateTime>("DateTime");
                                var distance = xHeader.GetFirstDescendantValue<int>("Distance");
                                if (distance != 0)
                                {
                                    summary.Distance = new Distance(distance, DistanceUnit.Meter);
                                }
                            }
                            else if (reader.Name == "Sample")
                            {
                                var xSample = reader.ReadAsXElement();
                                DateTime utcTime = xSample.GetFirstDescendantValue<DateTime>("UTC");
                                var frame = frames.FirstOrDefault(frm => frm.Timestamp == utcTime);
                                if (frame == null)
                                {
                                    frame = new ActivityTimeFrame { Timestamp = utcTime };
                                    frames.Add(frame);
                                }
                                var sampleType = xSample.GetFirstDescendantValue<string>("SampleType");
                                if (sampleType == "gps-tiny")
                                {
                                    frame.Position = new Position(
                                        (float)MathEx.Rad2Deg(xSample.GetFirstDescendantValue<double>("Longitude")),
                                        (float)MathEx.Rad2Deg(xSample.GetFirstDescendantValue<double>("Latitude"))
                                        );
                                }
                                else if (sampleType == "periodic")
                                {
                                    var distance = xSample.GetFirstDescendantValue<int>("Distance");
                                    if (distance != 0)
                                    {
                                        frame.Distance = new Distance(distance, DistanceUnit.Meter);
                                    }
                                    var speed = xSample.GetFirstDescendantValue<float>("Speed");
                                    if (speed != 0.0f)
                                    {
                                        frame.Speed = new Speed(speed, SpeedUnit.MeterPerSecond);
                                    }
                                    var altitude = xSample.GetFirstDescendantValue<int>("Altitude");
                                    if (altitude != 0)
                                    {
                                        frame.Altitude = new Distance(altitude, DistanceUnit.Meter);
                                    }
                                }
                            }
                            else if (reader.Name == "Time")
                            {
                                node = SmlNodeType.Time;
                            }
                            else if (reader.Name == "R-R")
                            {
                                node = SmlNodeType.RR;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (node)
                            {
                                case SmlNodeType.Time:
                                    break;
                            }
                            break;
                    }
                }

                // have to add timeframes in order (by timestamp)
                foreach (var frame in frames.OrderBy(frm => frm.Timestamp))
                {
                    Activity.AddTimeFrame(frame);
                }
                Activity.SetSummary(summary);
            }
        }
    }
}
