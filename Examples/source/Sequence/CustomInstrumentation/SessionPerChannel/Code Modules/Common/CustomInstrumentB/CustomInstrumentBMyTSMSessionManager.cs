using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.Common
{
    internal partial class MyTSMSessionManager : TSMSessionManager
    {
        public new CustomInstrumentBSessionsBundle CustomInstrumentB(string pin)
        {
            return CustomInstrumentB(new string[] { pin });
        }

        public new CustomInstrumentBSessionsBundle CustomInstrumentB(string[] pins)
        {
            _tsmContext.PinsToCustomInstrumentBSessions(
                pins,
                out var sessions,
                out var channelLists);
            return new CustomInstrumentBSessionsBundle(
                _tsmContext,
                sessions.Select((s) => new CustomInstrumentBSessionInformation(s)).ToList());
        }
    }
}
