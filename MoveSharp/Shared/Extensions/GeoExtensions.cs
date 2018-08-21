using System;

namespace MoveSharp.Extensions
{
    public static class GeoExtensions
    {
        public static bool IsNaN(this SharpGeo.Position pos)
        {
            return float.IsNaN(pos.Longitude) || float.IsNaN(pos.Latitude) || float.IsNaN(pos.Altitude);
        }

        public static bool IsNaN(this SharpGeo.Bound bound)
        {
            return float.IsNaN(bound.North) || float.IsNaN(bound.South) || float.IsNaN(bound.West) || float.IsNaN(bound.East);
        }
    }
}

