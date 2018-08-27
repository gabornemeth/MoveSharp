//
// Profile.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveSharp.Models
{
    /// <summary>
    /// Athlete profile
    /// </summary>
    public interface IProfile
    {
        /// <summary>
        /// Name of the athlete
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description (this should reveal provider's details)
        /// </summary>
        string Description { get; }
    }
}
