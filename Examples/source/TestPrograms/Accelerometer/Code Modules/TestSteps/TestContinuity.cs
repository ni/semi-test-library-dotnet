using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
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
        /// Tests the DUT pin connections to verify continuity, checking for both opens and shorted connections.
        /// Pins mapped to either DCPower and Digital instruments are supported.
        /// DCPower pins are operated on first, followed by Digital pins.
        /// Test method procedure:
        /// 1. All pins will be forced to zero Volts at start of testing.
        /// 2. For each pin, serially, one-at-a-time:
        /// 2a. Force negative current on the pin.
        /// 2b. Wait for settling.
        /// 2c. Measure voltage and publish results (id: Continuity Sinking)
        /// 2d. Force positive current on the pin.
        /// 2e. Wait for settling.
        /// 2f. Measure voltage and publish results (id: Continuity Sourcing)
        /// 2g. Forced pin back down to zero Volts.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The name of DUT pins or pin group(s) to test continuity, as defined in the pin map file.</param>
        /// <exception cref="System.ArgumentException">This exception will be thrown if any of the pins do not map to either a DCPower or Digital instrument.</exception>
        public static void TestContinuity(ISemiconductorModuleContext semiconductorModuleContext, string[] pinsAndPinGroups)
        {
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(pinsAndPinGroups, filterPins: true);
            DigitalSessionsBundle digitalPins = sessionManager.Digital(pinsAndPinGroups, filterPins: true);
            // Expected to be null if calling code passes pins or pin groups that do not map to either DCPower or Digital pins.
            if (dcPowerPins == null && digitalPins == null)
            {
                throw new System.ArgumentException("Pin(s) must map to a DCPower or Digital instrument.", nameof(pinsAndPinGroups));
            }

            // Force all pins to zero Volts before testing.
            dcPowerPins?.ConfigureSourceDelay(250e-6);
            dcPowerPins?.ForceVoltage(0, currentLimit: 2e-3, waitForSourceCompletion: true);
            digitalPins?.ForceVoltage(0, currentLimitRange: 32e-3);

            double settlingTimeInSeconds = semiconductorModuleContext.GetSpecificationsValue("SettlingTimes.Continuity");

            TestDCPowerContinuity(dcPowerPins, settlingTimeInSeconds);
            TestDigitalContinuity(digitalPins, settlingTimeInSeconds);
        }

        private static void TestDCPowerContinuity(DCPowerSessionsBundle dcPowerPins, double settlingTimeInSeconds)
        {
            if (dcPowerPins == null)
            {
                return;
            }
            foreach (string pinName in dcPowerPins.AggregateSitePinList.Select(x => x.PinName))
            {
                DCPowerSessionsBundle pin = dcPowerPins.FilterByPin(pinName);
                // Set each site to sink 1mA current.
                pin.ConfigureSourceDelay(settlingTimeInSeconds);
                pin.ForceCurrent(-1e-3, waitForSourceCompletion: true);

                // Measure voltage then set each site to source 1mA current.
                PinSiteData<double> sinkingMeasurement = pin.MeasureAndPublishVoltage("Continuity Sinking");

                pin.ForceCurrent(1e-3, waitForSourceCompletion: true);

                // Measure voltage then reset each site to source 0 volts.
                PinSiteData<double> sourcingMeasurement = pin.MeasureAndPublishVoltage("Continuity Sourcing");

                pin.ForceVoltage(0, waitForSourceCompletion: true);
            }
        }

        private static void TestDigitalContinuity(DigitalSessionsBundle digitalPins, double settlingTimeInSeconds)
        {
            if (digitalPins == null)
            {
                return;
            }
            foreach (string pinName in digitalPins.AggregateSitePinList.Select(x => x.PinName))
            {
                DigitalSessionsBundle pin = digitalPins.FilterByPin(pinName);
                // Set each site to sink 1mA current.
                pin.ForceCurrent(-1e-3, currentLevelRange: 32e-3, voltageLimitLow: -2, voltageLimitHigh: 6, settlingTime: settlingTimeInSeconds);

                // Measure voltage then set each site to source 1mA current.
                PinSiteData<double> sinkingMeasurement = pin.MeasureAndPublishVoltage("Continuity Sinking");

                pin.ForceCurrent(1e-3, currentLevelRange: 32e-3, voltageLimitLow: -2, voltageLimitHigh: 6, settlingTime: settlingTimeInSeconds);

                // Measure voltage then reset each site to source 0 volts.
                PinSiteData<double> sourcingMeasurement = pin.MeasureAndPublishVoltage("Continuity Sourcing");

                pin.ForceVoltage(0, currentLimitRange: 32e-3, settlingTime: settlingTimeInSeconds);
            }
        }
    }
}