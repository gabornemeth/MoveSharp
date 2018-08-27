using System;

namespace MoveSharp
{
    public interface ITimer
    {
        event EventHandler<DateTime> Tick;

        TimeSpan Interval { get; set; }

        void Start();

        void Stop();
    }
}
