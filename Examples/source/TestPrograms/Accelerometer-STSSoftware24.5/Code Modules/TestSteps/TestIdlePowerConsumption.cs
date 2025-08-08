using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
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
        /// Test the idle power computation of the DUT by measuring the current of both the DUT's supply and ground pins.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="vccPinName">The DUT's Vcc supply pin name, as defined in the pin map file.</param>
        /// <param name="gndPinName">The DUT's GND pin name, as defined in the pin map file.</param>
        public static void TestIdlePowerConsumption(ISemiconductorModuleContext semiconductorModuleContext, string vccPinName, string gndPinName)
        {
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DCPowerSessionsBundle vccAndGndPins = sessionManager.DCPower(new[] { vccPinName, gndPinName });

            vccAndGndPins.MeasureAndPublishCurrent("");
        }
    }
}