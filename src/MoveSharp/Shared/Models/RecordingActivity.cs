using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    /// <summary>
    /// Activity which is currently recording
    /// </summary>
    public class RecordingActivity : MemoryActivity
    {
        public bool IsRecording
        {
            get;
            set;
        }
    }
}
