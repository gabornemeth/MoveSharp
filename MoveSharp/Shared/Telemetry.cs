using System;
using System.Collections.Generic;

namespace MoveSharp
{
    public enum TelemetryOption
    {
        NotSet,
        Disabled,
        Enabled
    }

    public static class TelemetryEvents
    {
        public const string RecordActivity = "Recording new activity";
        public const string RecordActivityContinue = "Recording new activity (continue)";
        public const string Download = "Downloading activity";
        public const string Upload = "Uploading activity";
        public const string ExportActivityAsGpx = "Exporting activity as GPX";
        public const string ExportActivityAsFit = "Exporting activity as FIT";
        public const string ExportSegmentAsGpx = "Exporting segment as GPX";
    }

    /// <summary>
    /// Telemetry client interface
    /// </summary>
    public interface ITelemetry
    {
        bool IsEnabled { get; set; }

        /// <summary>
        /// Starts tracking
        /// </summary>
        void StartTracking();

        /// <summary>
        /// Stops tracking
        /// </summary>
        void StopTracking();

        /// <summary>
        /// Tracking visit of a page
        /// </summary>
        /// <param name="pageType">Type of the visited page</param>
        void TrackPageView(string pageType);

        /// <summary>
        /// Tracks an event
        /// </summary>
        /// <param name="name"></param>
        void TrackEvent(string name, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Tracks an exception
        /// </summary>
        /// <param name="ex">Exception to track</param>
        void TrackException(Exception ex);
    }
}
