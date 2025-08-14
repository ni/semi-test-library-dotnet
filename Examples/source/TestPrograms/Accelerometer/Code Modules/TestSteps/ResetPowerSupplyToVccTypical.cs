using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer.Common.Constants;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Resets the DUT power supply to the typical Vcc condition, as defined in the specifications file.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="powerSupplyPinNames">The DUT's power supply pin names, as defined in the pin map file.</param>
        public static void ResetPowerSupplyToVccTypical(ISemiconductorModuleContext semiconductorModuleContext, string[] powerSupplyPinNames)
        {
            // Note that the VccTypicalSpecSymbol constant is defined in the Shared.cs file.
            double vccTypVoltage = semiconductorModuleContext.GetSpecificationsValue(VccTypicalSpecSymbol);

            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DCPowerSessionsBundle supplyPins = sessionManager.DCPower(powerSupplyPinNames);

            supplyPins.ConfigureSourceDelay(250e-6);
            supplyPins.ForceVoltage(vccTypVoltage, waitForSourceCompletion: true);
        }
    }
}