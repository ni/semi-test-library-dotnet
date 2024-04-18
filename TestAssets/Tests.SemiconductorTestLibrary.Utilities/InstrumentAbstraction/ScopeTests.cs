using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.InstrumentAbstraction
{
    public static class ScopeTests
    {
        public static TSMSessionManager TestSetup(string pinMapFileName, out ISemiconductorModuleContext tsmContext)
        {
            tsmContext = CreateTSMContext(pinMapFileName);
            Initialize(tsmContext);
            return new TSMSessionManager(tsmContext);
        }

        public static void TestCleanup(ISemiconductorModuleContext tsmContext)
        {
            Close(tsmContext);
        }
    }
}
