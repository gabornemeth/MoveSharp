using Dynastream.Fit;
using MoveSharp.Models;
using System;

namespace MoveSharp.Extensions
{
    public static class FitExtensions
    {
        public const uint InvalidDistance = 0xFFFFFFFF;
        public const float InvalidSpeed = 65535f;//0xFFFFFFFF;
        public const byte InvalidHeartRate = 0xFF;
        public const ushort InvalidPower = 0xFFFF;
        public const byte InvalidCadence = 0xFF;
        public const int InvalidPosition = 0x7FFFFFFF;
        public static readonly System.DateTime MinTime = new System.DateTime(1989, 12, 31);

        private static int Degree2SemiCircles(double degree)
        {
            // The correct formula is semicircles = degrees * ( 2^31 / 180 ), or semicircles = degrees * 11930464.71 (approximate)
            return (int)(degree * 11930464.71);
        }

        private static float SemiCircles2Degree(int semiCircles)
        {
            // The correct formula is semicircles = degrees * ( 2^31 / 180 ), or semicircles = degrees * 11930464.71 (approximate)
            return semiCircles / 11930464.71f;
        }

        public static void SetPositionLatInDegrees(this RecordMesg mesg, double lat)
        {
            mesg.SetPositionLat(Degree2SemiCircles(lat));
        }

        public static void SetPositionLongInDegrees(this RecordMesg mesg, double lon)
        {
            mesg.SetPositionLong(Degree2SemiCircles(lon));
        }

        public static float GetPositionLatInDegrees(this RecordMesg mesg)
        {
            return SemiCircles2Degree(mesg.GetPositionLat().GetValueOrDefault());
        }

        public static float GetPositionLongInDegrees(this RecordMesg mesg)
        {
            return SemiCircles2Degree(mesg.GetPositionLong().GetValueOrDefault());
        }

        public static bool HasPosition(this RecordMesg mesg)
        {
            return mesg.GetPositionLat().HasValue && mesg.GetPositionLat().Value != InvalidPosition;
        }

        public static float GetValidSpeed(float speed)
        {
            float scale = 1000.0f;
            var max = 0xFFFF;
            if (speed * scale > max)
                return max / scale;

            return speed;
        }

        public static ActivitySummary ToSummary(this SessionMesg msg)
        {
            ActivitySummary summary = new ActivitySummary();
            if (msg != null)
            {
                var startTime = msg.GetStartTime();
                if (startTime != null && startTime.GetDateTime() != MinTime)
                    summary.StartTime = startTime.GetDateTime();
                summary.ElapsedTime = Convert.ToInt32(msg.GetTotalElapsedTime().GetValueOrDefault(0));
                summary.MovingTime = Convert.ToInt32(msg.GetTotalTimerTime().GetValueOrDefault(0));
                summary.Distance = new Distance(msg.GetTotalDistance().GetValueOrDefault(Distance.InvalidValue), DistanceUnit.Meter);
                summary.Sport = msg.GetSport().GetValueOrDefault(Sport.Generic);
                summary.AvgHeartRate = msg.GetAvgHeartRate().GetValueOrDefault(FitExtensions.InvalidHeartRate);
                summary.MaxHeartRate = msg.GetMaxHeartRate().GetValueOrDefault(FitExtensions.InvalidHeartRate);
                summary.AvgCadence = msg.GetAvgCadence().GetValueOrDefault(FitExtensions.InvalidCadence);
                summary.MaxCadence = msg.GetMaxCadence().GetValueOrDefault(FitExtensions.InvalidCadence);
                summary.AvgPower = msg.GetAvgPower().GetValueOrDefault(FitExtensions.InvalidPower);
                summary.MaxPower = msg.GetMaxPower().GetValueOrDefault(FitExtensions.InvalidPower);
                summary.AvgSpeed = new Speed { Value = msg.GetAvgSpeed().GetValueOrDefault(FitExtensions.InvalidSpeed), Unit = SpeedUnit.MeterPerSecond };
                summary.MaxSpeed = new Speed { Value = msg.GetMaxSpeed().GetValueOrDefault(FitExtensions.InvalidSpeed), Unit = SpeedUnit.MeterPerSecond };
                summary.Ascent = msg.GetTotalAscent().GetValueOrDefault(0);
                summary.Descent = msg.GetTotalDescent().GetValueOrDefault(0);
            }

            return summary;
        }

        public static LapSummary ToSumary(this LapMesg msg)
        {
            var summary = new LapSummary
            {
                Distance = new Distance(msg.GetTotalDistance().GetValueOrDefault(0), DistanceUnit.Meter),
                ElapsedTime = Convert.ToInt32(msg.GetTotalElapsedTime().GetValueOrDefault(0)),
                MovingTime = Convert.ToInt32(msg.GetTotalTimerTime().GetValueOrDefault(0)),
                StartTime = msg.GetStartTime().GetDateTime(),
                AvgHeartRate = msg.GetAvgHeartRate().GetValueOrDefault(0),
                MaxHeartRate = msg.GetMaxHeartRate().GetValueOrDefault(0),
                AvgSpeed = new Speed(msg.GetAvgSpeed().GetValueOrDefault(0), SpeedUnit.MeterPerSecond),
                MaxSpeed = new Speed(msg.GetMaxSpeed().GetValueOrDefault(0), SpeedUnit.MeterPerSecond),
                AvgCadence = msg.GetAvgCadence().GetValueOrDefault(0),
                MaxCadence = msg.GetMaxCadence().GetValueOrDefault(0),
                AvgPower = msg.GetAvgPower().GetValueOrDefault(0),
                MaxPower = msg.GetMaxPower().GetValueOrDefault(0),
                Ascent = msg.GetTotalAscent().GetValueOrDefault(0),
                Descent = msg.GetTotalDescent().GetValueOrDefault(0)
            };

            return summary;
        }

        public static byte? GetValidHeartRate(this RecordMesg msg)
        {
            var hr = msg.GetHeartRate();
            if (hr.HasValue && hr.Value != InvalidHeartRate)
                return hr;

            return null;
        }

        public static float? GetValidDistance(this RecordMesg msg)
        {
            var distance = msg.GetDistance();
            if (distance.HasValue && Convert.ToUInt32(distance.Value) != InvalidDistance / 100)
                return distance;

            return null;
        }

        public static byte? GetValidCadence(this RecordMesg msg)
        {
            var cadence = msg.GetCadence();
            if (cadence.HasValue && cadence.Value != InvalidCadence)
                return cadence;

            return null;
        }

        public static Speed? GetValidSpeed(this RecordMesg msg)
        {
            var speed = msg.GetSpeed();
            if (speed.HasValue && speed.Value != InvalidSpeed)
                return new Speed(speed.Value, SpeedUnit.MeterPerSecond);

            return null;
        }

        public static ushort? GetValidPower(this RecordMesg msg)
        {
            var power = msg.GetPower();
            if (power.HasValue && power.Value != InvalidPower)
                return power.Value;

            return null;
        }

        public static bool IsCycling(this Sport sport)
        {
            return sport == Sport.Cycling || sport == Sport.EBiking;
        }
    }
}
