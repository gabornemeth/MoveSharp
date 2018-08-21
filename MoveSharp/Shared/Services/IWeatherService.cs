//
// WeatherService.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using System;
using SharpGeo;
using System.Threading.Tasks;
using MoveSharp.Models;

namespace MoveSharp.Services
{

    /// <summary>
    /// Weather service using http://openweathermap.org/API
    /// </summary>
    public interface IWeatherService
    {
        Task<Wind> GetWindAsync(Position location);
    }
}
