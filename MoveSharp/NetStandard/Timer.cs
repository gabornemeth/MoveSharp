//
// Timer.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Threading.Tasks;

namespace MoveSharp
{
    /// <summary>
    /// Timer implementation on Android systems
    /// </summary>
    public class Timer : ITimer
    {
        public event EventHandler<DateTime> Tick;
        private Task _task;
        private bool _isRunning;

        public TimeSpan Interval
        {
            get;
            set;
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _task = Task.Factory.StartNew(async() =>
            {
                while (_isRunning)
                {
                    await Task.Delay(Interval);
                    Tick?.Invoke(this, DateTime.Now);
                }
            });
        }

        public void Stop()
        {
            if (!_isRunning)
                return;
            _isRunning = false;
            _task.Wait();
        }
    }
}
