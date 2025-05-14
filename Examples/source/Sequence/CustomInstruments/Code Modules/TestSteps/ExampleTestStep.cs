using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
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
        /// Call this method from the test executive to execute the test code.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        /// <param name="pin">The pin to operate on.</param>
        public static void ExampleTestStep(ISemiconductorModuleContext tsmContext, string pin)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var myCustomInstrument = sessionManager.MyCustomInstrument(pin);

            myCustomInstrument.Configure(5);
            PinSiteData<double> measurement = myCustomInstrument.Measure();

            tsmContext.PublishResults(measurement, "ExampleMeasurement");
        }
    }
}