using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples
{
    /// <summary>
    /// This class contains examples of how to use the Semiconductor Test Library to write test methods.
    /// It is intended for example purposes only and are not meant to be ran standalone.
    /// It assume a hypothetical test program with any dependent instrument sessions have already initiated and configured prior.
    /// </summary>
    public static class WorkFlowExamples
    {
        /// <summary>
        /// Simple example to demonstrate the work flow for writing a test method with the Semiconductor Test Library.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module Context object.</param>
        public static void SimpleWorkFlowExample(ISemiconductorModuleContext tsmContext)
        {
            // 1. Create a new TSMSessionManager object and any other local variables required for the test.
            // Note that values are hard coded for demonstration purposes and would otherwise be replaced with appropriate parameter inputs.
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            string[] sumPinNames = new string[] { "VDD", "VSS" };
            string[] digitalPinNames = new string[] { "SDI", "SDO" };
            string relayConfigurationSetup = "SetupExampleTest";
            string relayConfigurationCleanup = "Cleanup";
            string patternName = "ExamplePattern";
            string patternResultsID = "PatternResults";
            double voltageLevel = 3.3;
            double currentLimit = 0.01;
            double settlingTime = 0.001;
            DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.001,
                Sense = DCPowerMeasurementSense.Remote
            };

            // 2. Use the TSMSessionManager to query session for target pins.
            DCPowerSessionsBundle smuPins = sessionManager.DCPower(sumPinNames);
            DigitalSessionsBundle digitalPins = sessionManager.Digital(digitalPinNames);

            // 3. Configure the instrumentation connected to the queried pins and set any relay configuration required before testing.
            smuPins.ConfigureMeasureSettings(measureSettings);
            tsmContext.ApplyRelayConfiguration(relayConfigurationSetup, settlingTime);

            // 4. Burst patterns, source and / or measuring the desired signals, and repeat as necessary to accomplish the test method.
            smuPins.ForceVoltage(voltageLevel, currentLimit);
            PreciseWait(settlingTime);
            PinSiteData<double> currentBefore = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentBefore");
            digitalPins.BurstPatternAndPublishResults(patternName, publishedDataId: patternResultsID);
            PreciseWait(settlingTime);
            PinSiteData<double> currentAfter = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentAfter");

            // 5. Publish any results collected.
            PinSiteData<double> currentDifference = currentBefore.Subtract(currentAfter).Abs();
            tsmContext.PublishResults(currentDifference, publishedDataId: "CurrentDifference");

            // 6. Clean up relay configurations and place the instrument in a safe state, as it makes sense for any proceeding test.
            smuPins.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
            smuPins.ConfigureOutputEnabled(false);
            PreciseWait(settlingTime);
            tsmContext.ApplyRelayConfiguration(relayConfigurationCleanup, settlingTime);
        }

        /// <summary>
        /// Simple example to demonstrate the work flow for writing a test method with the Semiconductor Test Library.
        /// It is similar to <see cref="SimpleWorkFlowExample"/> but executes steps 5 and 6 concurrently,
        /// using the InvokeInParallel method from the NationalInstruments.SemiconductorTestLibrary.Common.Utilities class.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module Context object.</param>
        public static void SimpleWorkFlowExampleWithInvokeInParallel(ISemiconductorModuleContext tsmContext)
        {
            // 1. Create a new TSMSessionManager object and any other local variables required for the test.
            // Note that values are hard coded for demonstration purposes and would otherwise be replaced with appropriate parameter inputs.
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            string[] sumPinNames = new string[] { "VDD", "VSS" };
            string[] digitalPinNames = new string[] { "SDI", "SDO" };
            string relayConfigurationSetup = "SetupExampleTest";
            string relayConfigurationCleanup = "Cleanup";
            string patternName = "ExamplePattern";
            string patternResultsID = "PatternResults";
            double voltageLevel = 3.3;
            double currentLimit = 0.01;
            double settlingTime = 0.001;
            DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
            {
                ApertureTime = 0.001,
                Sense = DCPowerMeasurementSense.Remote
            };

            // 2. Use the TSMSessionManager to query session for target pins.
            DCPowerSessionsBundle smuPins = sessionManager.DCPower(sumPinNames);
            DigitalSessionsBundle digitalPins = sessionManager.Digital(digitalPinNames);

            // 3. Configure the instrumentation connected to the queried pins and set any relay configuration required before testing.
            smuPins.ConfigureMeasureSettings(measureSettings);
            tsmContext.ApplyRelayConfiguration(relayConfigurationSetup, settlingTime);

            // 4. Burst patterns, source and / or measuring the desired signals, and repeat as necessary to accomplish the test method.
            smuPins.ForceVoltage(voltageLevel, currentLimit);
            PreciseWait(settlingTime);
            PinSiteData<double> currentBefore = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentBefore");
            digitalPins.BurstPatternAndPublishResults(patternName, publishedDataId: patternResultsID);
            PreciseWait(settlingTime);
            PinSiteData<double> currentAfter = smuPins.MeasureAndPublishCurrent(publishedDataId: "CurrentAfter");

            // It is more efficient to invoke the following steps in parallel, as they are independent of each other.
            // This can be done using the InvokeInParallel() method
            // from the NationalInstruments.SemiconductorTestLibrary.Common.Utilities class.
            InvokeInParallel(
                () =>
                {
                    // 5. Publish any results collected.
                    PinSiteData<double> currentDifference = currentBefore.Subtract(currentAfter).Abs();
                    tsmContext.PublishResults(currentDifference, publishedDataId: "CurrentDifference");
                },
                () =>
                {
                    // 6. Clean up relay configurations and place the instrument in a safe state, as it makes sense for any proceeding test.
                    smuPins.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
                    smuPins.ConfigureOutputEnabled(false);
                    PreciseWait(settlingTime);
                    tsmContext.ApplyRelayConfiguration(relayConfigurationCleanup, settlingTime);
                });
        }
    }
}
