using System.Collections.Generic;
using System.Linq;

namespace MyCompany.MyCustomInstrumentDriverAPI
{
    internal static class Simulation
    {
        private static Dictionary<string, SimulatedInstrument> _simulatedInstruments = new Dictionary<string, SimulatedInstrument>();

        internal static void InitInstrument(string instrumentName)
        {
            if (!_simulatedInstruments.ContainsKey(instrumentName))
            {
                _simulatedInstruments.Add(instrumentName, new SimulatedInstrument(instrumentName));
            }
        }

        internal static void ClearInstrument(string instrumentName)
        {
            _simulatedInstruments.Remove(instrumentName);
        }

        internal static void WriteDigitalChannelData(string instrumentName, string channelName, double data)
        {
            _simulatedInstruments[instrumentName].DigitalInputChannelData[channelName] = data;
        }

        internal static void WriteDigitalData(string instrumentName, double data)
        {
            foreach (var channelName in _simulatedInstruments[instrumentName].DigitalInputChannelData.Keys)
            {
                _simulatedInstruments[instrumentName].DigitalInputChannelData[channelName] = data;
            }
        }

        internal static void ResetInstrument(string instrumentName)
        {
            foreach (var channelName in _simulatedInstruments[instrumentName].DigitalInputChannelData.Keys)
            {
                _simulatedInstruments[instrumentName].DigitalInputChannelData[channelName] = 0;
            }
        }

        internal static double ReadAnalogChannel(string instrumentName, string channelName)
        {
            var digitalCh1 = _simulatedInstruments[instrumentName].AnalogOutputChannelsToDigitalLookUp[channelName][0];
            var digitalCh2 = _simulatedInstruments[instrumentName].AnalogOutputChannelsToDigitalLookUp[channelName][1];
            var digitalVal1 = _simulatedInstruments[instrumentName].DigitalInputChannelData[digitalCh1];
            var digitalVal2 = _simulatedInstruments[instrumentName].DigitalInputChannelData[digitalCh2];

            return (2 * digitalVal2 + digitalVal1) * 5 / 3;
        }
    }
}