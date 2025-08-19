using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
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
        /// Configures the DCPower instruments that will source and measure the DUT's Vcc pin.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsAndPinGroups">The name of the DUT's supply pin(s) or pin group(s), as defined in the pin map file.</param>
        /// <exception cref="System.ArgumentException">This exception will be thrown if any of the pins do not map to a DCPower instrument.</exception>
        public static void SetupVcc(ISemiconductorModuleContext semiconductorModuleContext, string[] pinsAndPinGroups)
        {
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DCPowerSessionsBundle supplyPinsSessions = sessionManager.DCPower(pinsAndPinGroups, filterPins: true);
            // Expected to be null if calling code does passes pins or pin groups that do not map to DCPower pins.
            if (supplyPinsSessions == null)
            {
                throw new System.ArgumentException("Pin(s) must map to a DCPower instrument.", nameof(pinsAndPinGroups));
            }
            DCPowerSourceSettings sourceSettings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LevelRange = 100e-3,
                Level = 0,
                LimitRange = 5,
                Limit = 5,
                TransientResponse = DCPowerSourceTransientResponse.Normal,
                SourceDelayInSeconds = 250e-6,
            };
            DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
            {
                Sense = DCPowerMeasurementSense.Remote,
                ApertureTime = 30e-6,
                ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds
            };

            // There is no high-level extension method to perform the driver Reset function at the time of writing this.
            // Therefore, the Do method is being used to invoke the low-level driver method instead.
            supplyPinsSessions.Do(sessionInfo => sessionInfo.Session.Utility.Reset(sessionInfo.AllChannelsString));
            supplyPinsSessions.ConfigureSourceSettings(sourceSettings);
            supplyPinsSessions.ConfigureMeasureSettings(measureSettings);
            supplyPinsSessions.Initiate();
        }
    }
}
