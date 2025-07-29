using System.Collections.Generic;

namespace MyCompany.MyCustomInstrumentDriverAPI
{
    internal class SimulatedInstrument
    {
        internal readonly string InstrumentName;

        internal Dictionary<string, double> DigitalInputChannelData = new Dictionary<string, double>
        {
            ["dio0"] = 0,
            ["dio1"] = 0,
            ["dio2"] = 0,
            ["dio3"] = 0,
            ["dio4"] = 0,
            ["dio5"] = 0,
            ["dio6"] = 0,
            ["dio7"] = 0,
        };

        internal Dictionary<string, List<string>> AnalogOutputChannelsToDigitalLookUp = new Dictionary<string, List<string>>
        {
            { "ai0", new List<string> { "dio0", "dio1" } },
            { "ai1", new List<string> { "dio2", "dio3" } },
            { "ai2", new List<string> { "dio4", "dio5" } },
            { "ai3", new List<string> { "dio6", "dio7" } }
        };

        internal SimulatedInstrument(string instrumentName)
        {
            InstrumentName = instrumentName;
        }
    }
}