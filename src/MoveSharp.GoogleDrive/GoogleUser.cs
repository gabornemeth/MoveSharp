//
// GoogleUser.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Models;

namespace MoveSharp.GoogleDrive
{
    /// <summary>
    /// Google user profile
    /// </summary>
    internal class GoogleUser : IUserProfile
    {
        public string Name { get; internal set; }

        public string Description { get; internal set; }

        public bool IsAuthenticated { get; internal set; }

        public string ProfileImageUrl { get; internal set; }
    }
}
