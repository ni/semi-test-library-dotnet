using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Sync.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.InstrumentAbstraction
{
    public static class SyncTests
    {
        public static TSMSessionManager TestSetup(string pinMapFileName, out ISemiconductorModuleContext tsmContext)
        {
            return TestSetup(pinMapFileName, out tsmContext, out _);
        }

        public static TSMSessionManager TestSetup(string pinMapFileName, out ISemiconductorModuleContext tsmContext, out IPublishedDataReader publishedDataReader)
        {
            tsmContext = CreateTSMContext(pinMapFileName, out publishedDataReader);
            Initialize(tsmContext);
            return new TSMSessionManager(tsmContext);
        }

        public static void TestCleanup(ISemiconductorModuleContext tsmContext)
        {
            Close(tsmContext);
        }
    }
}
