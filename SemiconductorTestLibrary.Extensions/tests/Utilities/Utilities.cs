using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
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

        internal static void AssertPublishedDataCountPerPins(int expectedCount, IPublishedDataReader publishedDataReader, params string[] pins)
        {
            var publishedData = publishedDataReader.GetAndClearPublishedData();
            foreach (var pinName in pins)
            {
                Assert.Equal(expectedCount, publishedData.Where(d => d.Pin == pinName).Count());
            }
        }
    }
}
