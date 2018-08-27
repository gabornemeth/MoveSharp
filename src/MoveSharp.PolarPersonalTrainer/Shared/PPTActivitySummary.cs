//
// PPtActivitySummary.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using System.Threading.Tasks;
using PolarPersonalTrainerLib;
using MoveSharp.Models;

namespace MoveSharp.PolarPersonalTrainer
{
    /// <summary>
    /// Activity exported from PolarPersonalTrainer.com
    /// </summary>
    public class PPTActivitySummary : IActivitySummary
    {
        public PPTExercise Exercise { get; private set; }

        public PPTActivitySummary(PPTExercise exercise)
        {
            Exercise = exercise;
            Laps = new LapSummaryCollection();
            Laps.Add(this);
        }

        public Dynastream.Fit.Sport Sport
        {
            get
            {
                if (Exercise.Sport == "Cycling")
                    return Dynastream.Fit.Sport.Cycling;
                if (Exercise.Sport == "Running")
                    return Dynastream.Fit.Sport.Running;

                return Dynastream.Fit.Sport.Generic;
            }
        }

        public string Name
        {
            get { return Exercise.StartTime.ToString(); }
        }

        public Distance Distance
        {
            get { return new Distance(Exercise.Distance, DistanceUnit.Meter); }
        }

        public int AvgHeartRate
        {
            get { return Exercise.HeartRate.Average; }
        }

        public int MaxHeartRate
        {
            get { return Exercise.HeartRate.Maximum; }
        }

        public int AvgPower
        {
            get { return 0; }
        }

        public int MaxPower
        {
            get { return 0; }
        }

        private Speed _avgSpeed = new Speed();
        public Speed AvgSpeed
        {
            get { return _avgSpeed; }
        }

        private Speed _maxSpeed = new Speed();
        public Speed MaxSpeed
        {
            get { return _maxSpeed; }
        }

        public int AvgCadence
        {
            get { return 0; }
        }

        public int MaxCadence
        {
            get { return 0; }
        }

        public DateTime StartTime
        {
            get { return Exercise.StartTime; }
        }

        public int ElapsedTime
        {
            get { return (int)Exercise.Duration.TotalSeconds; }
        }

        public int MovingTime
        {
            get
            {
                // No support of moving time in PolarPersonalTrainerLib
                return ElapsedTime;
            }
        }

        public async Task GetPropertiesAsync()
        {
        }

        public void CopyFrom(IActivitySummary source)
        {
            throw new NotImplementedException();
        }

        public LapSummaryCollection Laps
        {
            get; private set;
        }

        public float Ascent
        {
            get { return Exercise.Ascent; }
        }


        public float Descent
        {
            get { return Exercise.Descent; }
        }
    }
}
