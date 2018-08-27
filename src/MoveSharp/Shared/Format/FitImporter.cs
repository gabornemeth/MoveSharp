using Dynastream.Fit;
using XTools.Diagnostics;
using MoveSharp.Extensions;
using MoveSharp.Models;
using System.IO;
using MoveSharp.Geolocation;
using System;

namespace MoveSharp.Format
{
    public class FitImporter : ActivityImporter
    {
        public Decode Decoder { get; private set; }
        public MesgBroadcaster MessageBroadcaster { get; private set; }
        private System.DateTime _lastTime;

        public FitImporter(MemoryActivity activity)
            : base(activity)
        {
            Decoder = new Decode();
            MessageBroadcaster = new MesgBroadcaster();

            // Connect the Broadcaster to our event (message) source (in this case the Decoder)
            Decoder.MesgEvent += MessageBroadcaster.OnMesg;
            Decoder.MesgDefinitionEvent += MessageBroadcaster.OnMesgDefinition;

            // Subscribe to message events of interest by connecting to the Broadcaster
            MessageBroadcaster.SessionMesgEvent += MessageBroadcaster_SessionMesgEvent;
            MessageBroadcaster.RecordMesgEvent += MessageBroadcaster_RecordMesgEvent;
            MessageBroadcaster.EventMesgEvent += MessageBroadcaster_EventMesgEvent;
            MessageBroadcaster.LapMesgEvent += MessageBroadcaster_LapMesgEvent;
            MessageBroadcaster.DeviceInfoMesgEvent += MessageBroadcaster_DeviceInfoMesgEvent;
            MessageBroadcaster.FileIdMesgEvent += MessageBroadcaster_FileIdMesgEvent;
        }

        private void MessageBroadcaster_FileIdMesgEvent(object sender, MesgEventArgs e)
        {
            var msg = e.mesg as FileIdMesg;
            if (msg == null)
                return;
            
            var manufacturer = msg.GetManufacturer();
            if (manufacturer.HasValue)
            {
                switch (manufacturer.Value)
                {
                    case Manufacturer.Garmin:
                        Activity.Device = "Garmin";
                        var garminProduct = msg.GetGarminProduct();
                        if (garminProduct.HasValue)
                        {
                            switch (garminProduct.Value)
                            {
                                case GarminProduct.Edge500:
                                case GarminProduct.Edge500China:
                                case GarminProduct.Edge500Japan:
                                case GarminProduct.Edge500Korea:
                                case GarminProduct.Edge500Taiwan:
                                    Activity.Device += " Edge 500";
                                    break;
                                case GarminProduct.Edge510:
                                case GarminProduct.Edge510Asia:
                                case GarminProduct.Edge510Japan:
                                case GarminProduct.Edge510Korea:
                                    Activity.Device += " Edge 510";
                                    break;
                                case GarminProduct.Edge800:
                                case GarminProduct.Edge800China:
                                case GarminProduct.Edge800Japan:
                                case GarminProduct.Edge800Korea:
                                case GarminProduct.Edge800Taiwan:
                                    Activity.Device += " Edge 800";
                                    break;
                                case GarminProduct.Edge810:
                                case GarminProduct.Edge810China:
                                case GarminProduct.Edge810Japan:
                                case GarminProduct.Edge810Taiwan:
                                    Activity.Device += " Edge 810";
                                    break;
                                case GarminProduct.Edge1000:
                                case GarminProduct.Edge1000China:
                                case GarminProduct.Edge1000Japan:
                                case GarminProduct.Edge1000Korea:
                                case GarminProduct.Edge1000Taiwan:
                                    Activity.Device += " Edge 1000";
                                    break;
                            }
                        }
                        break;
                    case Manufacturer.Suunto:
                        Activity.Device = "Suunto";
                        break;
                    default:
                        break;
                }
            }
        }

        private void MessageBroadcaster_DeviceInfoMesgEvent(object sender, MesgEventArgs e)
        {
            var msg = e.mesg as DeviceInfoMesg;
            if (msg == null)
                return;
        }

        private void MessageBroadcaster_LapMesgEvent(object sender, MesgEventArgs e)
        {
            var msg = e.mesg as LapMesg;
            if (msg == null)
                return;

            var lap = msg.ToSumary();
            // we can only add MemoryLap to the MemoryActivity's laps
            var lapToAdd = new MemoryLap();
            lapToAdd.SetSummary(lap);
            Activity.Laps.Add(lapToAdd);
        }

        public override void Load(Stream input)
        {
            Activity.Reset();
            bool status = Decoder.IsFIT(input);
            status &= Decoder.CheckIntegrity(input);
            // Process the file
            if (status)
            {
                Log.Write("Decoding...");
                Decoder.Read(input);
                Log.Write("Decoded FIT file");
            }
            else
            {
                Log.Write("Integrity check failed.");
                Log.Write("Attempting to decode...");
                Decoder.Read(input);
            }
        }

        void MessageBroadcaster_SessionMesgEvent(object sender, MesgEventArgs e)
        {
            var msg = e.mesg as SessionMesg;
            if (msg != null)
            {
                ActivitySummary summary = msg.ToSummary();
                Activity.SetSummary(summary);
            }
        }

        void MessageBroadcaster_EventMesgEvent(object sender, MesgEventArgs e)
        {
            var msgEvent = e.mesg as EventMesg;
            var type = msgEvent.GetEventType();
            if (type.HasValue)
            {
                if (type.Value == EventType.Start)
                {
                    Activity.AddTimeFrame(new ActivityTimeFrame { Timestamp = msgEvent.GetTimestamp().GetDateTime(), Type = ActivityTimeFrameType.Start });
                }
                else if (type.Value == EventType.Stop)
                {
                    Activity.AddTimeFrame(new ActivityTimeFrame { Timestamp = msgEvent.GetTimestamp().GetDateTime(), Type = ActivityTimeFrameType.Stop });
                }
            }
        }

        void MessageBroadcaster_RecordMesgEvent(object sender, MesgEventArgs e)
        {
            var msgRecord = e.mesg as RecordMesg;
            if (msgRecord == null)
                return;

            var timeFrame = new ActivityTimeFrame();
            timeFrame.Timestamp = msgRecord.GetTimestamp().GetDateTime();
            _lastTime = timeFrame.Timestamp;
            timeFrame.HeartRate = msgRecord.GetValidHeartRate();
            timeFrame.Cadence = msgRecord.GetValidCadence();
            timeFrame.Power = msgRecord.GetValidPower();
            timeFrame.Timestamp = msgRecord.GetTimestamp().GetDateTime();
            timeFrame.Speed = msgRecord.GetValidSpeed();
            var distance = FitExtensions.GetValidDistance(msgRecord);
            if (distance.HasValue)
                timeFrame.Distance = new Distance(distance.Value, DistanceUnit.Meter);

            var alt = msgRecord.GetEnhancedAltitude();

            if (msgRecord.HasPosition())
            {
                timeFrame.Position = new SharpGeo.Position
                {
                    Latitude = msgRecord.GetPositionLatInDegrees(),
                    Longitude = msgRecord.GetPositionLongInDegrees(),
                    Altitude = msgRecord.GetAltitude().GetValueOrDefault()
                };
            }

            Activity.AddTimeFrame(timeFrame);
        }
    }
}
