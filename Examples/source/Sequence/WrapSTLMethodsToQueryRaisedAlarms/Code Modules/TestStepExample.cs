using NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using TSMSessionManager = NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms.STLMethodWrappersWithAlarmQuery.TSMSessionManagerWithAlarmQuery;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.WrapSTLMethodsToQueryRaisedAlarms
{
    /// <summary>
    /// This class contains examples of how to use the Semiconductor Test Library to write test methods.
    /// It is intended for example purposes only and are not meant to be ran standalone.
    /// It assume a hypothetical test program with any dependent instrument sessions have been already initiated and configured.
    /// </summary>
    public static class WorkFlowExamples
    {
        /// <summary>
        /// Simple example to demonstrate the workflow for writing a test method with the Semiconductor Test Library.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">Names of pins mapped to a Source Measure Unit.</param>
        /// <param name="digitalPinNames">Names of pins mapped to a Digital Pattern Instrument.</param>
        /// <param name="patternName">Name of the pattern to burst.</param>
        /// <param name="relayConfigBeforeTest">Relay configuration defined in pin map to be applied before testing.</param>
        /// <param name="relayConfigAfterTest">Relay configuration defined in pin map to be applied after testing.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "NI1704:Identifiers should be spelled correctly", Justification = "SMU is a valid acronym and Config is an approved abbreviation")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "VSSpell001:Spell Check", Justification = "SMU is a valid acronym")]
        public static void SimpleWorkFlowExample(
                ISemiconductorModuleContext semiconductorModuleContext,
                string[] smuPinNames,
                string[] digitalPinNames,
                string patternName,
                string relayConfigBeforeTest,
                string relayConfigAfterTest)
        {
            // 1. Create a new TSMSessionManager object and other local variables for the test.
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            double voltageLevel = 3.3;
            double currentLimit = 0.01;
            double settlingTime = 0.001;
            var measureSettings = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.001,
                Sense = DCPowerMeasurementSense.Remote
            };

            // 2. Use the TSMSessionManager object to query the session for the target pins.
            var smuPins = sessionManager.DCPower(smuPinNames);
            var digitalPins = sessionManager.Digital(digitalPinNames);

            // 3. Configure the instrumentation connected to the target pins and configure relays.
            smuPins.ConfigureMeasureSettings(measureSettings);
            semiconductorModuleContext.ApplyRelayConfiguration(relayConfigBeforeTest, waitSeconds: settlingTime);

            // 4. Source and/or measure the signals.
            smuPins.ForceVoltage(voltageLevel, currentLimit);
            PreciseWait(timeInSeconds: settlingTime);
            var currentBefore = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentBefore", clearAlarm: true);

            // 5. Burst the patterns required to configure the DUT.
            digitalPins.BurstPatternAndPublishResults(patternName, publishedDataId: "PatternResults");
            PreciseWait(timeInSeconds: settlingTime);
            var currentAfter = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentAfter", clearAlarm: true);

            // 6. Calculate and/or publish the required test results.
            var currentDifference = currentBefore.Subtract(currentAfter).Abs();
            semiconductorModuleContext.PublishResults(currentDifference, publishedDataId: "CurrentDifference", clearAlarm: true);

            // 7. Clean up and restore the state of the instrumentation after finishing the test.
            smuPins.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
            smuPins.PowerDown();
            PreciseWait(timeInSeconds: settlingTime);
            semiconductorModuleContext.ApplyRelayConfiguration(relayConfigAfterTest, waitSeconds: settlingTime);
        }
    }
}
