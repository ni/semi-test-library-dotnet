using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using TSMSessionManager = SemiconductorTestLibrary.Examples.CustomInstrumentation.Common.MyTSMSessionManager;
using static SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB.CustomInstrumentBTSMExtensions;
using SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Initializes the Custom Instrumentation sessions associated with the pin map with the type id of "CustomInstrumentB".
        /// /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="measurementRange">The measurement range.</param>
        public static void SetupCustomInstrumentB(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            double measurementRange = 1)
        {
            try
            {
                CustomInstrumentBInitializeAndClose.Initialize(tsmContext, resetDevice);

                tsmContext.GetPins(InstrumentTypeId, out var dutPins, out var systemPins);
                var sessionManager = new TSMSessionManager(tsmContext);
                var customInstrument = sessionManager.CustomInstrumentB(dutPins.Concat(systemPins).ToArray());
                customInstrument.Configure(measurementRange);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}