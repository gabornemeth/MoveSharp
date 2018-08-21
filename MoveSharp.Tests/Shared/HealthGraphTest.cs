//
// HealthGraphTest.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2016, Gabor Nemeth
//

using MoveSharp.Tests.Authentication;
using HealthGraphNet;
using HealthGraphNet.Models;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Tests of using HealthGraph API
    /// </summary>
    [TestFixture]
    public class HealthGraphTest
    {
        private Client _client;

        [SetUp]
        public void PreTestCase()
        {
            var authenticator = new TestAuthenticator
            {
                AccessToken = Settings.Instance.Runkeeper.AccessToken
            };
            _client = new Client(authenticator);
        }

        [Test]
        public async Task UploadActivityThenDelete()
        {
            var users = new UsersEndpoint(_client);
            var user = await users.GetUser();
            var activities = new FitnessActivitiesEndpoint(_client, user);

            var newActivity = new FitnessActivitiesNewModel
            {
                //Type = FitnessActivityType.Cycling
            };

            // upload
            var uri = await activities.CreateActivity(newActivity);
            Assert.IsTrue(!string.IsNullOrEmpty(uri));

            //Read from Feed
            //var activities = await ActivitiesRequest.GetFeedPage();
            //var activitiesItem = activities.Items.FirstOrDefault();
            //Assert.IsNotNull(activitiesItem);
            //Assert.AreEqual(newActivity.Type, activitiesItem.Type);
            //Assert.AreEqual(newActivity.StartTime.ToString(), activitiesItem.StartTime.ToString());
            //Assert.AreEqual(newActivity.TotalDistance, activitiesItem.TotalDistance);
            //Assert.AreEqual(newActivity.Duration, activitiesItem.Duration);
            //Assert.AreEqual(uri, activitiesItem.Uri);


            //Delete
            await activities.DeleteActivity(uri);

        }
    }
}
