using NUnit.Framework;
using PolarPersonalTrainerLib;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoveSharp.Tests
{
    /// <summary>
    /// Test for Polar Personal Trainer web export interface
    /// </summary>
    [TestFixture]
    public class PolarPersonalTrainerTest
    {
        UserPasswordSettings Settings => MoveSharp.Tests.Settings.Instance.PolarPersonalTrainer;

        [Test(Description ="Retrieve data from PolarPersonalTrainer.com")]
#if NETCORE
        [Ignore("Does not work on .NET Core")]
#endif
        public async Task GetData()
        {
            var client = new PPTExport(Settings.UserName, Settings.Password);
            var result = await client.GetExercises(DateTime.Now.AddYears(-1), DateTime.Now);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() > 0);
        }
    }
}
