//
// UploadOptions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

namespace MoveSharp.Models
{
    /// <summary>
    /// Options for activity upload
    /// </summary>
    public class UploadOptions
    {
        public string FileName { get; set; }
        public Dynastream.Fit.Sport Sport { get; set; }
    }

    public class UploadStatus
    {
        public string Id { get; protected set; }
        public string Status { get; set; }
        public virtual bool IsCompleted { get; protected set; }
    }
}
