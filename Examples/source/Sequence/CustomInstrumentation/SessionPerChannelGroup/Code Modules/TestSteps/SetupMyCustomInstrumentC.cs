using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using TSMSessionManager = SemiconductorTestLibrary.Examples.CustomInstrumentation.Common.MyTSMSessionManager;
using static SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC.CustomInstrumentCTSMExtensions;
using SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Initializes the Custom Instrumentation sessions associated with the pin map with the type id of "CustomInstrumentC".
        /// /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="measurementRange">The measurement range.</param>
        public static void SetupCustomInstrumentC(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            double measurementRange = 1)
        {
            try
            {
                CustomInstrumentCInitializeAndClose.Initialize(tsmContext, resetDevice);

                tsmContext.GetPins(InstrumentTypeId, out var dutPins, out var systemPins);
                var sessionManager = new TSMSessionManager(tsmContext);
                var customInstrument = sessionManager.CustomInstrumentC(dutPins.Concat(systemPins).ToArray());
                customInstrument.Configure(measurementRange);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}