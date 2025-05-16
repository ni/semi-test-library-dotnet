using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.Common
{
    internal partial class MyTSMSessionManager : TSMSessionManager
    {
        public new CustomInstrumentCSessionsBundle CustomInstrumentC(string pin)
        {
            return CustomInstrumentC(new string[] { pin });
        }

        public new CustomInstrumentCSessionsBundle CustomInstrumentC(string[] pins)
        {
            _tsmContext.PinsToCustomInstrumentCSessions(
                pins,
                out var sessions,
                out var channelGroupIds,
                out var channelLists);
            return new CustomInstrumentCSessionsBundle(
                _tsmContext,
                sessions.Select((s) => new CustomInstrumentCSessionInformation(s)).ToList());
        }
    }
}
