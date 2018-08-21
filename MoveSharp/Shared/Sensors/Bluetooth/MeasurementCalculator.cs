using System;

namespace MoveSharp
{
    public class MeasurementCalculator<T, V>  where T : Measurement
    {
        public MeasurementCalculator()
        {
            MaxAge = 10;
        }

        /// <summary>
        /// Maximum age of measurement in seconds
        /// </summary>
        public int MaxAge { get; protected set; }

        public event EventHandler<V> Changed;

        protected void OnChanged(V value)
        {
            if (Changed != null)
                Changed(this, value);
        }

        public virtual void Update(Measurement measurement)
        {
        }
    }
}

