using System;
using SharpGeo;
using System.Diagnostics;

namespace MoveSharp.Geolocation
{
    /// <summary>
    /// Avg and current speed calculator
    /// </summary>
    public class MoveCalculator
    {
        private const int NumberOfPositionsToCalc = 5;

        private Position lastPosition;
        private int numPositionsReceived;

        private readonly float[] distances = new float[NumberOfPositionsToCalc];
        private readonly TimeSpan[] elapsedTimes = new TimeSpan[NumberOfPositionsToCalc];
        private float distance;
        private TimeSpan elapsedTime;

        private TimeSpan totalElapsedTime;
        private float avgSpeed;
        private float maxSpeed;

        /// <summary>
        /// Current speed in m/s
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                if (System.Math.Abs(elapsedTime.TotalSeconds) < double.Epsilon)
                    return 0;
                return distance / (float)elapsedTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Average speed in m/s
        /// </summary>
        public float AvgSpeed
        {
            get
            {
                return avgSpeed;
            }
        }

        /// <summary>
        /// Maximum speed in m/s
        /// </summary>
        public float MaxSpeed
        {
            get
            {
                return maxSpeed;
            }
        }

        /// <summary>
        /// Distance covered in meters
        /// </summary>
        public float Distance
        {
            get
            {
                return avgSpeed * (float)totalElapsedTime.TotalSeconds;
            }
        }

        public void Reset()
        {
            numPositionsReceived = 0;
            lastPosition = Position.Empty;
            totalElapsedTime = TimeSpan.FromSeconds(0);
            distance = 0;
            avgSpeed = maxSpeed = 0;
        }

        public void Start()
        {
            lastPosition = Position.Empty;
        }

        public void Add(Position pos, TimeSpan elapsedTimeCurrent)
        {
            try
            {
                var distanceCurrent = lastPosition.IsEmpty ? 0 : GeoHelper.Distance(pos, lastPosition);
                if (numPositionsReceived < NumberOfPositionsToCalc)
                {
                    // there is still space for new position
                    distance += distanceCurrent;
                    this.elapsedTime += elapsedTimeCurrent;
                    numPositionsReceived++;
                }
                else
                {
                    // shift old distance and time values and recalculate sum(distance) and sum(elapsedTime)
                    distance = distanceCurrent;
                    this.elapsedTime = elapsedTimeCurrent;
                    for (int i = 0; i < NumberOfPositionsToCalc - 1; i++)
                    {
                        distances[i] = distances[i + 1];
                        elapsedTimes[i] = elapsedTimes[i + 1];

                        distance += distances[i];
                        this.elapsedTime += elapsedTimes[i];
                    }
                    numPositionsReceived = NumberOfPositionsToCalc;
                }
                
                // store current distance and time
                distances[numPositionsReceived - 1] = distanceCurrent;
                elapsedTimes[numPositionsReceived - 1] = elapsedTimeCurrent;
                if (CurrentSpeed > maxSpeed)
                    maxSpeed = CurrentSpeed;
                if (avgSpeed.Equals(0))
                    avgSpeed = CurrentSpeed;
                else
                    avgSpeed = (avgSpeed * (float)totalElapsedTime.TotalSeconds + distanceCurrent) / (float)(totalElapsedTime.TotalSeconds + elapsedTimeCurrent.TotalSeconds);

                totalElapsedTime += elapsedTimeCurrent;
                lastPosition = pos; // store last position
                //Debug.WriteLine("MoveCalculator add");
                //for (int i = 0; i < numPositionsReceived; i++)
                //{
                //    Debug.WriteLine(string.Format("\t{0}. elapsed time= {1} sec\tdistance= {2} m", i, elapsedTimes[i], distances[i]));

                //}
                //Debug.WriteLine(string.Format("\tTotal elapsed time= {0} sec\n\tAvg. speed= {1} m/s", totalElapsedTime.TotalSeconds, avgSpeed));
            }
            catch (Exception ex)
            {
                // TODO: sometimes "index out of bounds" exception occurs - try to track source of it
                throw ex;
            }
        }
    }
}
