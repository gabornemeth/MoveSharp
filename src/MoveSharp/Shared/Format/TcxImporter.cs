using MoveSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XTools.Diagnostics;
using SharpGeo;

namespace MoveSharp.Format
{
    public enum TcxNodeType
    {
        None,
        Activity,
        Trackpoint,
        Time,
        Latitude,
        Longitude,
        HeartRate,
        Power,
        Speed
    }

    /// <summary>
    /// Training Center XML (TCX) importer
    /// </summary>
    public class TcxImporter : ActivityImporter
    {
        private const string Trackpoint = "Trackpoint";
        /// <summary>
        /// Current frame
        /// </summary>
        private ActivityTimeFrame _frame;
        private TcxNodeType _node;
        private Position _position;

        public TcxImporter(MemoryActivity activity)
            : base(activity)
        {
            Activity.Sport = Dynastream.Fit.Sport.Generic;
        }

        private void EndFrame()
        {
            if (_frame != null)
                Activity.AddTimeFrame(_frame);
        }

        private void BeginNewFrame()
        {
            _frame = new ActivityTimeFrame();
        }

        public override void Load(System.IO.Stream source)
        {
            DateTime time;
            using (XmlReader reader = XmlReader.Create(source))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Activity")
                            {
                                // getting sport from the Sport attribute of Activity
                                var sport = reader.GetAttribute("Sport");
                                if (!string.IsNullOrEmpty(sport)) // sport
                                {
                                    if (sport == "Biking")
                                        Activity.Sport = Dynastream.Fit.Sport.Cycling;
                                    else if (sport == "Running")
                                        Activity.Sport = Dynastream.Fit.Sport.Running;
                                    else if (sport == "Multisport")
                                        Activity.Sport = Dynastream.Fit.Sport.Multisport;
                                }
                                _node = TcxNodeType.Activity;
                            }
                            if (reader.Name == "Trackpoint")
                            {
                                BeginNewFrame();
                                _node = TcxNodeType.Trackpoint;
                            }
                            else if (reader.Name == "Time")
                                _node = TcxNodeType.Time;
                            else if (reader.Name == "Position")
                                _position = new Position(); // position info is coming...
                            else if (reader.Name == "LatitudeDegrees")
                                _node = TcxNodeType.Latitude;
                            else if (reader.Name == "LongitudeDegrees")
                                _node = TcxNodeType.Longitude;
                            else if (reader.Name == "HeartRateBpm")
                                _node = TcxNodeType.HeartRate;
                            else if (reader.Name == "Watts")
                                _node = TcxNodeType.Power;
                            else if (reader.Name == "Speed")
                                _node = TcxNodeType.Speed;
                            break;
                        case XmlNodeType.Text:
                            try
                            {
                                if (_frame == null)
                                    break;
                                switch (_node)
                                {
                                    case TcxNodeType.Latitude:
                                        _position.Latitude = Convert.ToSingle(reader.Value, CultureInfo.InvariantCulture);
                                        break;
                                    case TcxNodeType.Longitude:
                                        _position.Longitude = Convert.ToSingle(reader.Value, CultureInfo.InvariantCulture);
                                        break;
                                    case TcxNodeType.HeartRate:
                                        _frame.HeartRate = Convert.ToByte(reader.Value);
                                        break;
                                    case TcxNodeType.Time:
                                        if (DateTime.TryParseExact(reader.Value, "yyyy-MM-ddTHH:mm:ssZ", null, DateTimeStyles.None, out time))
                                            _frame.Timestamp = time;
                                        else if (DateTime.TryParseExact(reader.Value, "yyyy-MM-ddTHH:mm:ss.fffZ", null, DateTimeStyles.None, out time))
                                            _frame.Timestamp = time;
                                        break;
                                    case TcxNodeType.Power:
                                        _frame.Power = Convert.ToUInt16(reader.Value);
                                        break;
                                    case TcxNodeType.Speed:
                                        _frame.Speed = new Speed(Convert.ToSingle(reader.Value, CultureInfo.InvariantCulture), SpeedUnit.MeterPerSecond);
                                        break;
                                }
                            }
                            catch
                            {
                                Log.Error("Error parsing: {0}", reader.Value);
                            }
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
                            if (reader.Name == "Position")
                                _frame.Position = _position; // position has been parsed
                            else if (reader.Name == "Trackpoint")
                                EndFrame();
                            if (reader.Name != "Value")
                                _node = TcxNodeType.None;
                            break;
                    }
                }
            }
        }
    }
}
