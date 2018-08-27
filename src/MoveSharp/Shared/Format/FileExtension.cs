using System;

namespace MoveSharp.Format
{
    public static class FileExtension
    {
        /// <summary>
        /// thisisant FIT
        /// </summary>
        public static string Fit => ".fit";
        /// <summary>
        /// GPX
        /// </summary>
        public static string Gpx => ".gpx";
        /// <summary>
        /// Garmin TCX
        /// </summary>
        public static string Tcx => ".tcx";
        /// <summary>
        /// Polar HRM
        /// </summary>
        public static string Hrm => ".hrm";

        public static bool AreTheSame(string ext1, string ext2)
        {
            return ext1.Equals(ext2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
