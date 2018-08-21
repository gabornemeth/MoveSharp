//
// GoogleDriveActivity.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Models;
using System;
using Dynastream.Fit;
using System.Threading.Tasks;

namespace MoveSharp.GoogleDrive
{
    internal class GoogleDriveActivity : IActivitySummary<string>
    {
        internal GoogleDriveActivity(Google.Apis.Drive.v3.Data.File file)
        {
            Id = file.Id;
            Name = file.Name;
            ParseFileInfo(file);
        }

        public Sport Sport { get; private set; }

        public string Name { get; private set; }

        public LapSummaryCollection Laps { get; private set; }

        public Distance Distance { get; private set; }

        public float Ascent { get; private set; }

        public float Descent { get; private set; }

        public int AvgHeartRate { get; private set; }

        public int MaxHeartRate { get; private set; }

        public int AvgPower { get; private set; }

        public int MaxPower { get; private set; }

        public Speed AvgSpeed { get; private set; }

        public Speed MaxSpeed { get; private set; }

        public int AvgCadence { get; private set; }

        public int MaxCadence { get; private set; }

        public System.DateTime StartTime { get; private set; }

        public int ElapsedTime { get; private set; }

        public int MovingTime { get; private set; }

        public string Id { get; private set; }

        public void CopyFrom(IActivitySummary source)
        {
            throw new NotImplementedException();
        }

        public Task GetPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        void ParseFileInfo(Google.Apis.Drive.v3.Data.File file)
        {
            if (file.Name.TryParseAsDateTime(out System.DateTime startTime))
            {
                StartTime = startTime;
            }
            else
            {
                StartTime = file.CreatedTime.Value;
            }
        }
    }

    public class GoogleDriveMemoryActivity : MemoryActivity, IActivitySummary<string>
    {
        public string Id { get; internal set; }
    }
}
