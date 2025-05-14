using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.CustomInstrument.Common;
using TSMSessionManager = SemiconductorTestLibrary.Examples.CustomInstrument.Common.MyTSMSessionManager;

namespace SemiconductorTestLibrary.Examples.CustomInstrument
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Initializes the Custom Instrumentation sessions associated with the pin map with the type id of "MyCustomInstrument".
        /// /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="measurementRange">The measurement range.</param>
        public static void SetupMyCustomInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            double measurementRange = 1)
        {
            try
            {
                MyCustomInstrumentInitializeAndClose.Initialize(tsmContext, resetDevice);

                tsmContext.GetPins(MyCustomInstrument.InstrumentTypeId, out var dutPins, out var systemPins);
                var sessionManager = new TSMSessionManager(tsmContext);
                var customInstrument = sessionManager.MyCustomInstrument(dutPins.Concat(systemPins).ToArray());
                customInstrument.Configure(measurementRange);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}