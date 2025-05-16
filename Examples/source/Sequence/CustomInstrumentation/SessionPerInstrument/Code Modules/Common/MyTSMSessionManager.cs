using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.Common
{
    internal partial class MyTSMSessionManager: TSMSessionManager
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        public MyTSMSessionManager(ISemiconductorModuleContext tsmContext) : base(tsmContext)
        {
            _tsmContext = tsmContext;
        }
    }
}
