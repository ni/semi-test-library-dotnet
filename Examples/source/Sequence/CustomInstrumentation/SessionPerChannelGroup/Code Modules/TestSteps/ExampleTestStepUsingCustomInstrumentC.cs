using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC;
using TSMSessionManager = SemiconductorTestLibrary.Examples.CustomInstrumentation.Common.MyTSMSessionManager;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Call this method from the test executive to execute the test code.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        /// <param name="pins">The pins to operate on.</param>
        public static void ExampleTestStepUsingCustomInstrumentC(ISemiconductorModuleContext tsmContext, string[] pins)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var customInstrumentC = sessionManager.CustomInstrumentC(pins);

            customInstrumentC.Configure(5);
            PinSiteData<double> measurement = customInstrumentC.Measure();

            tsmContext.PublishResults(measurement, "");
        }
    }
}