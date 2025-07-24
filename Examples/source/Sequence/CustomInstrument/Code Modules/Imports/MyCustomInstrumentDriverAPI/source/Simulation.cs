using System.Collections.Generic;

namespace MyCompany.MyCustomInstrumentDriverAPI
{
    internal static class Simulation
    {
        private static Dictionary<string, SimulatedInstrument> SimulatedInstruments = new Dictionary<string, SimulatedInstrument>();

        internal static void InitInstrument(string instrumentName)
        {
            if (!SimulatedInstruments.ContainsKey(instrumentName))
            {
                SimulatedInstruments.Add(instrumentName, new SimulatedInstrument(instrumentName));
            }
            
            
        }

        internal static void ClearInstruments()
        {
            SimulatedInstruments = new Dictionary<string, SimulatedInstrument>();
        }

        internal static void WriteDigitalChannelData(string instrumentName, string channelName, double data)
        {
            SimulatedInstruments[instrumentName].DigitalInputChannelData[channelName] = data;
        }

        internal static double ReadAnalogChannel(string instrumentName, string channelName)
        {
            var digitalCh1 = SimulatedInstruments[instrumentName].AnalogOutputChannelsToDigitalLookUp[channelName][0];
            var digitalCh2 = SimulatedInstruments[instrumentName].AnalogOutputChannelsToDigitalLookUp[channelName][1];
            var digitalVal1 = SimulatedInstruments[instrumentName].DigitalInputChannelData[digitalCh1];
            var digitalVal2 = SimulatedInstruments[instrumentName].DigitalInputChannelData[digitalCh2];

            return (2 * digitalVal2 + digitalVal1) * 5 / 3;
        }
    }
}