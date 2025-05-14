using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.MultiplexedConnections.Common.MyDMM;
using System.Linq;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections.Common
{
    internal class MyTSMSessionManager : TSMSessionManager
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        public MyTSMSessionManager(ISemiconductorModuleContext tsmContext) : base(tsmContext)
        {
            _tsmContext = tsmContext;
        }

        public new MyDMMSessionsBundle DMM(string pin)
        {
            _tsmContext.GetNIDmmSessions(pin, out var sessions);
            return DMM(new string[] { pin });
        }

        public new MyDMMSessionsBundle DMM(string[] pins)
        {
            _tsmContext.GetNIDmmSessions(pins, out var sessions);
            return new MyDMMSessionsBundle(_tsmContext, sessions.Select((s) => new MyDMMSessionInformation(s)).ToList());
        }
    }
}
