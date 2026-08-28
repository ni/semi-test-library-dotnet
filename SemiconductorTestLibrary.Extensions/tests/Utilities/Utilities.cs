using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    internal static class Utilities
    {
        /// <summary>
        /// Returns the trigger name for a given site pin, leader channel string, and trigger type.
        /// </summary>
        /// <param name="sitePinInfo">The site pin information object.</param>
        /// <param name="leaderChannelString">Channel string of the leader channel used for ganging.</param>
        /// <param name="triggerType">The type of trigger to generate the name for. Defaults to "Source".</param>
        /// <returns>
        /// The trigger name string for the specified site pin and trigger type, or an empty string if not applicable.
        /// </returns>
        public static string GetTriggerName(SitePinInfo sitePinInfo, string leaderChannelString, string triggerType = "Source")
        {
            var channel = sitePinInfo.IndividualChannelString;
            var leaderChannel = leaderChannelString.Split('/');
            var leaderChannelSlot = leaderChannel[0];
            var leaderChannelNumber = leaderChannel[leaderChannel.Length - 1];

            if (sitePinInfo.CascadingInfo is GangingInfo gangingInfo && gangingInfo.IsFollower)
            {
                return $"/{leaderChannelSlot}/Engine{leaderChannelNumber}/{triggerType}Trigger";
            }
            if (channel.Contains("SMU_4147") && (triggerType == "Source"))
            {
                return $"/{channel.Remove(channel.Length - 2)}/Immediate";
            }
            return string.Empty;
        }
        internal static void AssertInitiateBehaviorMatchesUpdateMode(DCPowerSessionsBundle sessionsBundle, UpdateMode updateMode)
        {
            void InitiateTest()
            {
                sessionsBundle.Initiate();
            }

            if (updateMode == UpdateMode.Immediate)
            {
                var exception = Assert.Throws<NISemiconductorTestException>(InitiateTest);
                Assert.Contains("The session is already running.", exception.Message);
            }
            else
            {
                sessionsBundle.Initiate(); // Should not throw exception for Deferred or Commit update modes
            }
        }

        internal static void AssertPublishedDataCountPerPins(int expectedCount, IPublishedDataReader publishedDataReader, params string[] pins)
        {
            AssertPublishedDataCountPerPins(expectedCount, publishedDataReader.GetAndClearPublishedData(), pins);
        }

        internal static void AssertPublishedDataCountPerPins(int expectedCount, IPublishedData[] publishedData, params string[] pins)
        {
            foreach (var pinName in pins)
            {
                Assert.Equal(expectedCount, publishedData.Where(d => d.Pin == pinName).Count());
            }
        }

        internal static void AssertEqualForDoubleArrays(double[] expected, double[] actual, int precision = 3)
        {
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i], precision);
            }
        }
    }
}
