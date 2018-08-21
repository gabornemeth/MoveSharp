using MoveSharp.Models;

namespace MoveSharp
{
    public static class IActivitySummaryExtensions
    {
        public static string GetNameFromStartTime(this IActivitySummary activity)
        {
            return activity.StartTime.ToString("yyyy-MM-dd-HH-mm-ss");
        }
    }
}
