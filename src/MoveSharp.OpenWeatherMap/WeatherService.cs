//
// WeatherService.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using SharpGeo;
using System.Threading.Tasks;
using OpenWeatherMap;

namespace MoveSharp.Services
{
    /// <summary>
    /// Weather service using http://openweathermap.org/API
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private OpenWeatherMapClient _client = new OpenWeatherMapClient("7d4e0bf0eb86b1a2caed1ba2cdcdb43e");

        public async Task<CurrentWeatherResponse> GetDataAsync(Position location)
        {
            var coordinates = new Coordinates { Latitude = location.Latitude, Longitude = location.Longitude };
            var weather = await _client.CurrentWeather.GetByCoordinates(coordinates);
            return weather;
        }

        public async Task<Models.Wind> GetWindAsync(Position location)
        {
            var weather = await GetDataAsync(location);
            return new Models.Wind { Degree = weather.Wind.Direction.Value, Speed = weather.Wind.Speed.Value };
        }
    }
}
