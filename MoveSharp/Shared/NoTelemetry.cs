using System;
using System.Collections.Generic;
using System.Text;

namespace MoveSharp
{
    /// <summary>
    /// Dummy telemetry implementation. Does nothing
    /// </summary>
    public class NoTelemetry : ITelemetry
    {
        public bool IsEnabled
        {
            get
            {
                return false;
            }

            set
            {
            }
        }

        public void StartTracking()
        {
        }

        public void StopTracking()
        {
        }

        public void TrackEvent(string name, Dictionary<string, string> parameters = null)
        {
        }

        public void TrackException(Exception ex)
        {
        }

        public void TrackPageView(string pageType)
        {
        }
    }
}
