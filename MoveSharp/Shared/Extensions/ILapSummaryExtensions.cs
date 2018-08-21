using MoveSharp.Models;
using System;

namespace MoveSharp
{
    public static class ILapSummaryExtensions
    {
        public static bool HasPower(this ILapSummary summary) => summary.AvgPower != 0 || summary.MaxPower != 0;
    }
}
