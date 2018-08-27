using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    /// <summary>
    /// Errors during upload
    /// </summary>
    public class UploadException : Exception
    {
        public UploadException(string message) : base(message)
        {
        }
    }
}
