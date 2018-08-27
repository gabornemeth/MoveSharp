using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MoveSharp.Format
{
    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    [XmlRoot(ElementName = "gpx", Namespace = "http://www.topografix.com/GPX/1/1", IsNullable = false)]
    public class GpxExercise
    {
        [XmlElement("metadata")]
        public GpxMetadata MetaData
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlElement("trk")]
        public GpxTrack Track
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlAttribute("creator")]
        public string Creator
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlAttribute("version")]
        public string Version
        {
            get;
            set;
        }

        public GpxExercise()
        {
            MetaData = new GpxMetadata();
            Track = new GpxTrack();
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxMetadata
    {
        /// <remarks/>
        [XmlElement("name")]
        public string Name
        {
            get; set;
        }

        /// <remarks/>
        [XmlElement("author")]
        public GpxMetadataAuthor Author
        {
            get; set;
        }

        /// <remarks/>
        [XmlElement("time")]
        public DateTime Time
        {
            get; set;
        }

        /// <remarks/>
        [XmlElement("bounds")]
        public GpxMetadataBounds Bounds
        {
            get; set;
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxMetadataAuthor
    {
        [XmlElement("name")]
        public string Name
        {
            get; set;
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxMetadataBounds
    {
        /// <remarks/>
        [XmlAttribute("maxlon")]
        public decimal MaxLongitude { get; set; }

        /// <remarks/>
        [XmlAttribute("maxlat")]
        public decimal MaxLatitude
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlAttribute("minlon")]
        public decimal MinLongitude
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlAttribute("minlat")]
        public decimal MinLatitude { get; set; }
    }

    /// <summary>
    /// GPX track
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxTrack
    {
        /// <summary>
        /// Track point segments
        /// </summary>
        [XmlElement("trkseg")]
        //[XmlArrayItemAttribute("trkpt", IsNullable = false)]
        public GpxTrackSegment[] Segments
        {
            get;
            set;
        }

        public GpxTrackpoint GetTrackPoint(int index)
        {
            if (Segments == null)
                return null;
            
            foreach (GpxTrackSegment segment in Segments)
            {
                if (index < segment.Trackpoints.Count)
                    return segment.Trackpoints[index];

                index -= segment.Trackpoints.Count;
            }

            return null;
        }
    }

    [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxTrackSegment
    {
        [XmlElement("trkpt")]
        public List<GpxTrackpoint> Trackpoints
        {
            get;
            set;
        }

        public GpxTrackSegment()
        {
            Trackpoints = new List<GpxTrackpoint>();
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/1")]
    public class GpxTrackpoint
    {
        /// <summary>
        /// Altitude
        /// </summary>
        [XmlElement("ele")]
        public double Elevation
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlElement("time")]
        public DateTime Time
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlElement("sat")]
        public byte Satellites
        {
            get;
            set;
        }

        /// <summary>
        /// Longitude
        /// </summary>
        [XmlAttribute("lon")]
        public double Longitude
        {
            get;
            set;
        }

        /// <summary>
        /// Latitude
        /// </summary>
        [XmlAttribute("lat")]
        public double Latitude
        {
            get;
            set;
        }
    }
}
