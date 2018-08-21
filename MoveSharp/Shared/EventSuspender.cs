//
// EventSuspender.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
namespace MoveSharp
{
    /// <summary>
    /// Helper class for temporarily suspend event handling
    /// </summary>
    public class EventSuspender
    {
        protected int _suspendEvent;

        /// <summary>
        /// Gets whether it is currently suspended
        /// </summary>
        public bool IsSuspended
        {
            get
            {
                return _suspendEvent != 0;
            }
        }

        /// <summary>
        /// Suspend event handling
        /// </summary>
        public void Suspend()
        {
            _suspendEvent++;;
        }

        /// <summary>
        /// Allow event handling
        /// </summary>
        public void Allow()
        {
            _suspendEvent--;
        }
    }
}
