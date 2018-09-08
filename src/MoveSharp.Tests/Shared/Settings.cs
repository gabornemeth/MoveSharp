//
// Settings.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MoveSharp.Tests
{
    public class OAuth2Settings
    {
        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }
    }

    public class UserPasswordSettings
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }

    public class Settings
    {
        private string _rootFolder;

        /// <summary>
        /// Root folder for the test files
        /// </summary>
        public string RootFolder
        {
            get { return _rootFolder ?? (_rootFolder = Path.Combine("Data")); }
        }

        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                return _instance ?? (_instance = new Settings());
            }
        }

        public Settings()
        {
            var settingsFilePath = Environment.GetEnvironmentVariable("MOVESHARP_TESTSETTINGS");
            if (File.Exists(settingsFilePath))
            {
                var settings = LoadSettings(settingsFilePath);
                SetupServicesFromJson(settings);
            }
            else
            {
                SetupServicesAsDefault();
            }
        }

        public UserPasswordSettings PolarPersonalTrainer { get; private set; }

        public OAuth2Settings Strava { get; private set; }

        public OAuth2Settings Runkeeper { get; private set; }

        JObject LoadSettings(string settingsFilePath)
        {
            using (var fs = File.Open(settingsFilePath, FileMode.Open))
            {
                using (var reader = new StreamReader(fs))
                {
                    var settingsAsJson = reader.ReadToEnd();
                    return (JObject)JsonConvert.DeserializeObject(settingsAsJson);
                }
            }
        }

        void SetupServicesFromJson(JObject settings)
        {
            PolarPersonalTrainer = new UserPasswordSettings
            {
                UserName = (string)settings["PolarPersonalTrainer"]["UserName"],
                Password = (string)settings["PolarPersonalTrainer"]["Password"]
            };

            Strava = new OAuth2Settings
            {
                AccessToken = (string)settings["Strava"]["AccessToken"]
            };

            Runkeeper = new OAuth2Settings
            {
                AccessToken = (string)settings["HealthGraph"]["AccessToken"]
            };
        }

        void SetupServicesAsDefault()
        {
            PolarPersonalTrainer = new UserPasswordSettings
            {
                UserName = "unknown",
                Password = "unknown"
            };

            Strava = new OAuth2Settings
            {
                AccessToken = "unknown"
            };

            Runkeeper = new OAuth2Settings
            {
                AccessToken = "unknown"
            };
        }
    }
}
