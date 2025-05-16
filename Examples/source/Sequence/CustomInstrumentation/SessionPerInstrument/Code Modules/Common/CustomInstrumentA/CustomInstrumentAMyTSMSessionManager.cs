using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.Common
{
    internal partial class MyTSMSessionManager: TSMSessionManager
    {
        public new CustomInstrumentASessionsBundle CustomInstrumentA(string pin)
        {
            return CustomInstrumentA(new string[] { pin });
        }

        public new CustomInstrumentASessionsBundle CustomInstrumentA(string[] pins)
        {
            _tsmContext.PinsToCustomInstrumentASessions(
                pins, 
                out var sessions,
                out var channelLists);
            return new CustomInstrumentASessionsBundle(
                _tsmContext, 
                sessions.Select((s) => new CustomInstrumentASessionInformation(s)).ToList());
        }
    }
}
