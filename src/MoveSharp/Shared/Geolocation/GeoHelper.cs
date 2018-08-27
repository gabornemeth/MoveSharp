//
// GeoHelper.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using SharpGeo;
using static System.Math;

namespace MoveSharp.Geolocation
{
    public class GeoHelper
    {
        public static double Rad2Deg(double x)
        {
            // 180 degrees = PI radian
            return x * 180.0 / PI;
        }

        public static double Deg2Rad(double x)
        {
            // 180 degrees = PI radian
            return x * PI / 180;
        }

        public static float Distance(Position pos1, Position pos2)
        {
            return Distance(pos1.Latitude, pos1.Longitude, pos2.Latitude, pos2.Longitude);
        }

        public static float Distance(float lat1, float lon1, float lat2, float lon2)
        {
            const int radius = 6378137; // Radius of earth in m
            var dLat = Deg2Rad(lat2 - lat1);
            var dLong = Deg2Rad(lon2 - lon1);

            var a = Sin(dLat / 2) * Sin(dLat / 2) + Cos(Deg2Rad(lat1)) * Cos(Deg2Rad(lat2)) * Sin(dLong / 2) * Sin(dLong / 2);
            var c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
            var d = radius * c;

            return (float)d;
        }

        public static Bound CreateBound(Position center, Position radiusInDegrees)
        {
            return new Bound(center.Latitude + radiusInDegrees.Latitude, center.Longitude - radiusInDegrees.Longitude,
                center.Latitude - radiusInDegrees.Latitude, center.Longitude + radiusInDegrees.Longitude);
        }
    }
}
