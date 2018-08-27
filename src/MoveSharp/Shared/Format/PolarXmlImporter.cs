using MoveSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MoveSharp.Format
{
    /// <summary>
    /// XML node types of Polar Personal Trainer XML
    /// </summary>
    internal enum PolarXmlNodeType
    {
        None,
        Exercise,
        HeartRate,
        Speed,
        Cadence,
        Distance,
        Samples,
        SamplesType,
        Note
    }

    /// <summary>
    /// Imports activity from XML exported from Polar Personal Trainer
    /// </summary>
    public class PolarXmlImporter : ActivityImporter
    {
        public PolarXmlImporter(MemoryActivity activity)
            : base(activity)
        {
        }

        private ActivityTimeFrame GetFrame(int index)
        {
            while (Activity.TimeFrames.Count <= index)
                Activity.TimeFrames.Add(new ActivityTimeFrame());

            return Activity.TimeFrames[index];
        }

        public override void Load(System.IO.Stream source)
        {
            PolarXmlNodeType nodeType = PolarXmlNodeType.None;
            using (XmlReader reader = XmlReader.Create(source))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "exercise")
                            {
                                //// getting sport from the Sport attribute of Activity
                                //var sport = reader.GetAttribute("Sport");
                                //if (!string.IsNullOrEmpty(sport)) // sport
                                //{
                                //    if (sport == "Biking")
                                //        Activity.Sport = Dynastream.Fit.Sport.Cycling;
                                //    else if (sport == "Running")
                                //        Activity.Sport = Dynastream.Fit.Sport.Running;
                                //    else if (sport == "Multisport")
                                //        Activity.Sport = Dynastream.Fit.Sport.Multisport;
                                //}
                                //_node = TcxNodeType.Activity;
                            }
                            if (reader.Name == "samples")
                            {
                                nodeType = PolarXmlNodeType.Samples;
                            }
                            else if (reader.Name == "type")
                            {
                                if (nodeType == PolarXmlNodeType.Samples)
                                    nodeType = PolarXmlNodeType.SamplesType;
                            }
                            else if (reader.Name == "note")
                                nodeType = PolarXmlNodeType.Note;
                            //else if (reader.Name == "Position")
                            //    _position = new XDotNet.Geolocation.Position(); // position info is coming...
                            //else if (reader.Name == "LatitudeDegrees")
                            //    _node = TcxNodeType.Latitude;
                            //else if (reader.Name == "LongitudeDegrees")
                            //    _node = TcxNodeType.Longitude;
                            //else if (reader.Name == "HeartRateBpm")
                            //    _node = TcxNodeType.HeartRate;
                            //else if (reader.Name == "Watts")
                            //    _node = TcxNodeType.Power;
                            break;
                        case XmlNodeType.Text:
                            switch (nodeType)
                            {
                                case PolarXmlNodeType.Note:
                                    Activity.Name = reader.Value;
                                    break;
                                case PolarXmlNodeType.SamplesType:
                                    if (reader.Value == "HEARTRATE")
                                        nodeType = PolarXmlNodeType.HeartRate;
                                    else if (reader.Value == "SPEED")
                                        nodeType = PolarXmlNodeType.Speed;
                                    else if (reader.Value == "CADENCE")
                                        nodeType = PolarXmlNodeType.Cadence;
                                    break;
                                case PolarXmlNodeType.HeartRate:
                                    {
                                        var hrValues = reader.Value.Split(',');
                                        for (int i = 0; i < hrValues.Length; i++)
                                        {
                                            var frame = GetFrame(i);
                                            frame.HeartRate = Convert.ToByte(hrValues[i]);
                                        }
                                    }
                                    break;
                                case PolarXmlNodeType.Speed:
                                    {
                                        var values = reader.Value.Split(',');
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            var frame = GetFrame(i);
                                            frame.Speed = new Speed
                                            {
                                                Value = Convert.ToSingle(values[i], CultureInfo.InvariantCulture),
                                                Unit = SpeedUnit.KilometerPerHour
                                            };
                                        }
                                    }
                                    break;
                            }
                            //    case TcxNodeType.Longitude:
                            //        _position.Longitude = Convert.ToSingle(reader.Value, CultureInfo.InvariantCulture);
                            //        break;
                            //    case TcxNodeType.HeartRate:
                            //        _frame.HeartRate = Convert.ToByte(reader.Value);
                            //        break;
                            //    case TcxNodeType.Time:
                            //        if (DateTime.TryParseExact(reader.Value, "yyyy-MM-ddTHH:mm:ssZ", null, DateTimeStyles.None, out time))
                            //            _frame.Timestamp = time;
                            //        //_frame.Time = DateTime.ParseExact(reader.Value, "yyyy-MM-ddTHH:mm:ssZ", null);
                            //        break;
                            //    case TcxNodeType.Power:
                            //        _frame.Power = Convert.ToUInt16(reader.Value);
                            //        break;
                            //}
                            break;
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.SignificantWhitespace:
                            break;
                        case XmlNodeType.CDATA:
                            break;
                        case XmlNodeType.EntityReference:
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            break;
                        case XmlNodeType.DocumentType:
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "sample")
                                nodeType = PolarXmlNodeType.SamplesType;
                            //if (reader.Name == "Position")
                            //    _frame.Position = _position; // position has been parsed
                            //else if (reader.Name == "Trackpoint")
                            //    EndFrame();
                            //if (reader.Name != "Value")
                            //    _node = TcxNodeType.None;
                            break;
                    }
                }
            }
        }
    }
}
