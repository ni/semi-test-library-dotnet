using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Tests the leakage current for specified DUT pins and/or pin groups.
        /// Test method procedure:
        /// 1. All pins will be forced to zero Volts at start of testing.
        /// 2. For each pin, serially, one-at-a-time:
        /// 2a. Force voltage high on the pin.
        /// 2b. Wait for settling.
        /// 2c. Measure current and publish results (id: Leakage High)
        /// 2d. Force positive current on the pin.
        /// 2e. Wait for settling.
        /// 2f. Measure current and publish results (id: Leakage Low)
        /// 2g. Turn off the output to the pin.
        /// The voltage levels and the max current limit are set based on the values defined by symbols from the specification file.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The name of DUT pins or pin group(s) to test leakage, as defined in the pin map file.</param>
        public static void TestLeakage(ISemiconductorModuleContext semiconductorModuleContext, string[] pinsAndPinGroups)
        {
            string[] digitalPinNames = semiconductorModuleContext.FilterPinsByInstrumentType(pinsAndPinGroups, InstrumentTypeIdConstants.NIDigitalPattern);
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DigitalSessionsBundle allDigitalPins = sessionManager.Digital(digitalPinNames);

            allDigitalPins.TurnOffOutput();

            double voltageHigh = semiconductorModuleContext.GetSpecificationsValue("Leakage.VoltageHigh");
            double voltageLow = semiconductorModuleContext.GetSpecificationsValue("Leakage.VoltageLow");
            double maxCurrent = semiconductorModuleContext.GetSpecificationsValue("Leakage.MaxCurrent");
            double leakageSettlingTimeInSeconds = semiconductorModuleContext.GetSpecificationsValue("SettlingTimes.Leakage");

            foreach (string pinName in digitalPinNames)
            {
                DigitalSessionsBundle singleDigitalPin = allDigitalPins.FilterByPin(pinName);
                // Source voltageHigh.
                singleDigitalPin.ForceVoltage(voltageHigh, maxCurrent, settlingTime: leakageSettlingTimeInSeconds);

                // Measure current.
                singleDigitalPin.MeasureAndPublishCurrent("Leakage High");

                // Source voltageLow.
                singleDigitalPin.ForceVoltage(voltageLow, maxCurrent, settlingTime: leakageSettlingTimeInSeconds);

                // Measure current.
                singleDigitalPin.MeasureAndPublishCurrent("Leakage Low");

                singleDigitalPin.TurnOffOutput();
            }
        }
    }
}