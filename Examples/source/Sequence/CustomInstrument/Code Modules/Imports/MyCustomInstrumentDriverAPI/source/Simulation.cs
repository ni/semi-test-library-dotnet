using System.Collections.Generic;

namespace MyCompany.MyCustomInstrumentDriverAPI
{
    internal static class Simulation
    {
        private static readonly Dictionary<string, Dictionary<string, double>> digitalInputChannel = new Dictionary<string, Dictionary<string, double>>
        {
            {
                "dio0", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio1", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio2", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio3", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio4", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio5", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio6", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
            {
                "dio7", new Dictionary<string, double>
                {
                    { "dev1", 0 },
                    { "dev2", 0 }
                }
            },
        };
        private static readonly Dictionary<string, List<string>> analogOutputChannels = analogOutputChannels = new Dictionary<string, List<string>>
            {
                { "ai0", new List<string> { "dio0", "dio1" } },
                { "ai1", new List<string> { "dio2", "dio3" } },
                { "ai2", new List<string> { "dio4", "dio5" } },
                { "ai3", new List<string> { "dio6", "dio7" } }
            };

        internal static void WriteDigitalChannelData(string instrumentName, string channelName, double data)
        {
            digitalInputChannel[channelName][instrumentName] = data;
        }

        internal static double ReadAnalogChannel(string instrumentName, string channelName)
        {
            return (2 * digitalInputChannel[analogOutputChannels[channelName][1]][instrumentName] + digitalInputChannel[analogOutputChannels[channelName][0]][instrumentName]) * 5/3;
        }
    }
}
