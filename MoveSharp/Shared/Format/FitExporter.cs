//
// FitExporter.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using Dynastream.Fit;
using MoveSharp.Extensions;
using MoveSharp.Models;
using DateTime = Dynastream.Fit.DateTime;

namespace MoveSharp.Format
{
    /// <summary>
    /// Activity saving to FIT file format
    /// </summary>
    public class FitExporter : ActivityExporter
    {
        private Encode _encoder;

        public FitExporter(MemoryActivity activity)
            : base(activity)
        {
        }

        public void Open(System.IO.Stream stream)
        {
            _encoder = new Encode(stream, ProtocolVersion.V20);
        }

        public void Close()
        {
            if (_encoder != null)
            {
                _encoder.Close();
                _encoder = null;
            }
        }

        public override void Save(System.IO.Stream dest)
        {
            Open(dest);
            try
            {
                WriteHeader();
                WriteTimeFrames();
                WriteLaps();
                WriteSummary();
            }
            finally
            {
                Close();
            }
        }

        public void WriteHeader()
        {
            var fileIdMesg = new FileIdMesg();
            fileIdMesg.SetType(File.Activity);
            _encoder.Write(fileIdMesg);

            // TODO: identify smartphone
            //fileIdMesg.SetProduct(1000);
            //fileIdMesg.SetSerialNumber(12345);
            //encoder.Write(fileIdMesg);

            //var athlete = app.Athlete;
            //if (athlete != null)
            //{
            //    // User profile does not matter, because Strava uses its own attributes
            //    // TODO: Maybe not needed at all?
            //    var userProfile = new Fit.UserProfileMesg();
            //    userProfile.SetGender(athlete.StravaAthlete.Sex == "M" ? Fit.Gender.Male : Fit.Gender.Female);
            //    userProfile.SetWeight(70.0f);
            //    userProfile.SetAge(30);
            //    userProfile.SetFriendlyName(Encoding.UTF8.GetBytes(athlete.FullName));
            //    encoder.Write(userProfile);
            //}
        }

        public void WriteTimeFrames()
        {
            // write each timeframe as a record
            foreach (var timeFrame in Activity.TimeFrames)
            {
                WriteTimeFrame(timeFrame);
            }
        }

        public void WriteTimeFrame(ActivityTimeFrame timeFrame)
        {
            if (_encoder == null)
                return;

            if (timeFrame.Type == ActivityTimeFrameType.Start)
            {
                // resumed
                var startMsg = new EventMesg();
                startMsg.SetEventType(EventType.Start);
                startMsg.SetEvent(Event.OffCourse);
                startMsg.SetTimestamp(new Dynastream.Fit.DateTime(timeFrame.Timestamp));
                _encoder.Write(startMsg);
            }
            else if (timeFrame.Type == ActivityTimeFrameType.Stop)
            {
                // stopped
                var stopMsg = new EventMesg();
                stopMsg.SetEventType(EventType.Stop);
                stopMsg.SetEvent(Event.OffCourse);
                stopMsg.SetTimestamp(new Dynastream.Fit.DateTime(timeFrame.Timestamp));
                _encoder.Write(stopMsg);
            }
            else
            {
                var recordMsg = new RecordMesg();
                recordMsg.SetTimestamp(new Dynastream.Fit.DateTime(timeFrame.Timestamp));
                if (timeFrame.HasPosition)
                {
                    recordMsg.SetPositionLatInDegrees(timeFrame.Position.Value.Latitude);
                    recordMsg.SetPositionLongInDegrees(timeFrame.Position.Value.Longitude);
                    recordMsg.SetAltitude(timeFrame.Position.Value.Altitude);
                }
                if (timeFrame.HeartRate.HasValue)
                    recordMsg.SetHeartRate(timeFrame.HeartRate.Value);
                if (timeFrame.Power.HasValue)
                    recordMsg.SetPower(timeFrame.Power.Value);
                if (timeFrame.Cadence.HasValue)
                    recordMsg.SetCadence(timeFrame.Cadence.Value);
                if (timeFrame.Speed.HasValue)
                {
                    recordMsg.SetSpeed(timeFrame.Speed.Value.GetValueAs(SpeedUnit.MeterPerSecond));
                }
                if (timeFrame.Distance.HasValue)
                    recordMsg.SetDistance(timeFrame.Distance.Value.GetValueAs(DistanceUnit.Meter));
                _encoder.Write(recordMsg);

                // Write R-R values
                if (timeFrame.HasRRValues)
                {
                    var hrvMsg = new HrvMesg();
                    for (int i = 0; i < timeFrame.RRValues.Length; i++)
                        hrvMsg.SetTime(i, timeFrame.RRValues[i] / 1000.0f);
                    _encoder.Write(hrvMsg);
                }
            }
        }

        public void WriteLaps()
        {
            // write each lap's information
            foreach (var lap in Activity.Laps)
                WriteLap(lap);
        }

        public void WriteLap(ILapSummary lap)
        {
            if (lap == null || lap.ElapsedTime == 0)
                return;

            var lapMsg = new LapMesg();
            lapMsg.SetStartTime(new DateTime(lap.StartTime));
            lapMsg.SetTotalElapsedTime(lap.ElapsedTime);
            lapMsg.SetTotalMovingTime(lap.MovingTime);
            lapMsg.SetTotalDistance(lap.Distance.GetValueAs(DistanceUnit.Meter));
            lapMsg.SetAvgSpeed(FitExtensions.GetValidSpeed(lap.AvgSpeed.GetValueAs(SpeedUnit.MeterPerSecond)));
            lapMsg.SetMaxSpeed(FitExtensions.GetValidSpeed(lap.MaxSpeed.GetValueAs(SpeedUnit.MeterPerSecond)));
            lapMsg.SetAvgHeartRate((byte)lap.AvgHeartRate);
            lapMsg.SetMaxHeartRate((byte)lap.MaxHeartRate);
            lapMsg.SetAvgPower((ushort)lap.AvgPower);
            lapMsg.SetMaxPower((ushort)lap.MaxPower);
            lapMsg.SetAvgCadence((byte)lap.AvgCadence);
            lapMsg.SetMaxCadence((byte)lap.MaxCadence);
            lapMsg.SetTotalAscent(Convert.ToUInt16(lap.Ascent));
            lapMsg.SetTotalDescent(Convert.ToUInt16(lap.Descent));
            _encoder.Write(lapMsg);
        }

        public void WriteSummary()
        {
            // Session message
            var sessionMsg = new SessionMesg();
            sessionMsg.SetStartTime(new DateTime(Activity.StartTime));
            sessionMsg.SetTotalElapsedTime(Activity.ElapsedTime);
            sessionMsg.SetTotalMovingTime(Activity.MovingTime);
            sessionMsg.SetSport(Activity.Sport);
            sessionMsg.SetTotalDistance(Activity.Distance.GetValueAs(DistanceUnit.Meter));
            sessionMsg.SetAvgSpeed(FitExtensions.GetValidSpeed(Activity.AvgSpeed.GetValueAs(SpeedUnit.MeterPerSecond)));
            sessionMsg.SetMaxSpeed(FitExtensions.GetValidSpeed(Activity.MaxSpeed.GetValueAs(SpeedUnit.MeterPerSecond)));
            sessionMsg.SetAvgHeartRate((byte)Activity.AvgHeartRate);
            sessionMsg.SetMaxHeartRate((byte)Activity.MaxHeartRate);
            sessionMsg.SetAvgPower((ushort)Activity.AvgPower);
            sessionMsg.SetMaxPower((ushort)Activity.MaxPower);
            sessionMsg.SetAvgCadence((byte)Activity.AvgCadence);
            sessionMsg.SetMaxCadence((byte)Activity.MaxCadence);
            sessionMsg.SetTotalAscent(Convert.ToUInt16(Activity.Ascent));
            sessionMsg.SetTotalDescent(Convert.ToUInt16(Activity.Descent));
            _encoder.Write(sessionMsg);
        }

    }
}
