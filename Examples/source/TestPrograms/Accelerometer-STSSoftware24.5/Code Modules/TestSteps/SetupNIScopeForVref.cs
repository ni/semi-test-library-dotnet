using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Sets up the NI Scope instrument to measure the DUT's VRef signal.
        /// This code module is intended to be called only once during test program execution, when the lot starts.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinName">The scope pin name, as defined in the pin map file (default: Vref_OScope).</param>
        public static void SetupNIScopeForVref(ISemiconductorModuleContext semiconductorModuleContext, string pinName = "Vref_OScope")
        {
            var sessionManger = new TSMSessionManager(semiconductorModuleContext);
            var scopePin = sessionManger.Scope(pinName);

            // There are no high-level extension methods for Scope instruments at the time of writing this.
            // Therefore, the Do method is being used to invoke the low-level driver methods instead.
            scopePin.Do(sessionInfo =>
            {
                // Use empty string to access all channels.
                sessionInfo.Session.Channels[""].Configure(10, 0, ScopeVerticalCoupling.DC, 1, true);
                sessionInfo.Session.Channels[""].ConfigureCharacteristics(1e6, -1);
                sessionInfo.Session.Timing.ConfigureTiming(50e6, 5000, 50, 1, true);
                sessionInfo.Session.Trigger.ConfigureTriggerImmediate();
            });
        }
    }
}
