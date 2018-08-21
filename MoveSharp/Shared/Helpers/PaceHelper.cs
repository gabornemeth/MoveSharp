using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Helpers
{
    public class PaceHelper
    {
        /// <summary>
        /// Returns pace in minute string format
        /// </summary>
        /// <param name="size">Pace as float. 5.2</param>
        /// <returns>Pace as string 5:12</returns>
        public static string GetPaceAsString(float pace)
        {
            var min = System.Math.Floor(pace); // minute part
            var sec = pace - min; // seconds part
            return string.Format("{0}:{1:00}", Convert.ToInt32(min), Convert.ToInt32(sec * 60));
        }
    }
}
