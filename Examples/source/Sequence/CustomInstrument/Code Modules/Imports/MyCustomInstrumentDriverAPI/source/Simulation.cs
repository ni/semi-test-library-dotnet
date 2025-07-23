using System.Collections.Generic;

namespace MyCompany.MyCustomInstrumentDriverAPI
{
    internal static class Simulation
    {
        private static readonly Dictionary<string, double> digitalInputChannel = new Dictionary<string, double>
            {
                { "dio0", 0 },
                { "dio1", 0 },
                { "dio2", 0 },
                { "dio3", 0 },
                { "dio4", 0 },
                { "dio5", 0 },
                { "dio6", 0 },
                { "dio7", 0 }
            };
        private static readonly Dictionary<string, List<string>> analogOutputChannels = analogOutputChannels = new Dictionary<string, List<string>>
            {
                { "ai0", new List<string> { "dio0", "dio1" } },
                { "ai1", new List<string> { "dio2", "dio3" } },
                { "ai2", new List<string> { "dio4", "dio5" } },
                { "ai3", new List<string> { "dio6", "dio7" } }
            };

        internal static void WriteDigitalChannelData(string channelName, double data)
        {
            digitalInputChannel[channelName] = data;
        }

        internal static double ReadAnalogChannel(string channelName)
        {
            return (2 * digitalInputChannel[analogOutputChannels[channelName][1]] + digitalInputChannel[analogOutputChannels[channelName][0]]) * 5/3;
        }
    }
}
