using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Linq;
using MyCustomInstrumentClass = SemiconductorTestLibrary.Examples.CustomInstrument.Common.MyCustomInstrument;

namespace SemiconductorTestLibrary.Examples.CustomInstrument.Common
{
    internal class MyTSMSessionManager : TSMSessionManager
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        public MyTSMSessionManager(ISemiconductorModuleContext tsmContext) : base(tsmContext)
        {
            _tsmContext = tsmContext;
        }

        public new MyCustomInstrumentSessionsBundle MyCustomInstrument(string pin)
        {
            return MyCustomInstrument(new string[] { pin });
        }

        public new MyCustomInstrumentSessionsBundle MyCustomInstrument(string[] pins)
        {
            _tsmContext.GetCustomSessions(
                MyCustomInstrumentClass.InstrumentTypeId,
                pins, out var sessions,
                out var channelGroupIds,
                out var channelLists);
            return new MyCustomInstrumentSessionsBundle(
                _tsmContext, 
                sessions.Select((s) => new MyCustomInstrumentSessionInformation(s as MyCustomInstrumentClass)).ToList());
        }
    }
}
