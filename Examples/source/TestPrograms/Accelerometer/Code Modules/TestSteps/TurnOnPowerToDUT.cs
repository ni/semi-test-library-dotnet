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
        /// Turns on power to the DUT by forcing voltage at the typical Vcc value, as defined in the specifications file.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="powerSupplyPinNames">The name of the DUT's supply pin(s) or pin group(s), as defined in the pin map file.</param>
        public static void TurnOnPowerToDUT(ISemiconductorModuleContext semiconductorModuleContext, string[] powerSupplyPinNames)
        {
            double vccVoltage = semiconductorModuleContext.GetSpecificationsValue(VccTypicalSpecSymbol);
            double vccMaxCurrent = semiconductorModuleContext.GetSpecificationsValue(DcVccTypicalMaxCurrentSpecSymbol);
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DCPowerSessionsBundle supplyPins = sessionManager.DCPower(powerSupplyPinNames);

            supplyPins.ConfigureSourceDelay(250e-6);
            supplyPins.ForceVoltage(vccVoltage, vccMaxCurrent, waitForSourceCompletion: true);
        }
    }
}