using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples
{
    /// <summary>
    /// This class contains examples of how to use the Semiconductor Test Library to write test methods.
    /// It is intended for example purposes only and are not meant to be ran standalone.
    /// It assume a hypothetical test program with any dependent instrument sessions have been already initiated and configured.
    /// </summary>
    public static partial class PatternTriggeredInstrumentControl
    {
        /// <summary>
        /// This example demonstrates how to make a SMU Measurement from a trigger sent from a pattern.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module Context object.</param>
        /// <param name="digitalPinNames">Name of digital pins used in the target pattern</param>
        /// <param name="smuPinNames">Name of SMU Pins to be measured</param>
        /// <param name="triggerPatternName">Name of the pattern to burst that will trigger the measurements</param>
        /// <param name="patternOpcodeEvent">The pattern opcode event number, 0-4 (default: 0)</param>
        /// <param name="publishedDataID">The publishedDataID to use for the published measurement results (default: SmuMeasurements).</param>
        public static void PatternTriggeredSmuMeasurement(
            ISemiconductorModuleContext tsmContext,
            string[] digitalPinNames,
            string[] smuPinNames,
            string triggerPatternName,
            int patternOpcodeEvent = 0,
            string publishedDataID = "SmuMeasurements")
        {
            // 1. Create a new TSMSessionManager object and any other local variables required for the test.
            // Note that values maybe hard coded for demonstration purposes and should otherwise be replaced with appropriate parameter inputs.
            var sessionManager = new TSMSessionManager(tsmContext);
            var timeoutInSeconds = 1.0;
            var voltageLevel = 5;
            var currentLimit = 0.001;

            // 2. Use the TSMSessionManager to query sessions for target pins.
            var digitalPins = sessionManager.Digital(digitalPinNames);
            var smuPins = sessionManager.DCPower(smuPinNames);

            // 3a. Get Fully Qualified Terminal Names to direct triggers
            var sourcefullyQualifiedTerminalName = digitalPins.InstrumentSessions.ElementAt(0).Session.Event.PatternOpcodeEvents[patternOpcodeEvent].TerminalName;
            // 3b. Configure the SMU Sessions and set measurement trigger to the exported pattern event trigger source.
            // Upon the next commit/initiate operation, the trigger terminals will automatically be routed and connected on the PXI backplane(s), even across PXI chassis if necessary.
            smuPins.ConfigureTriggerDigitalEdge(TriggerType.MeasureTrigger, sourcefullyQualifiedTerminalName);
            smuPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.OnMeasureTrigger);

            // 4a. Force Voltage with the SMU will start sourcing but await the configured trigger before measuring.
            smuPins.ForceVoltage(voltageLevel, currentLimit);
            // 4b. Burst the pattern that generates the digital trigger (event0)
            digitalPins.BurstPattern(triggerPatternName);
            // 4c. Fetch the current measurement from the first sample only.
            var currentMeasurements = smuPins.FetchMeasurement(timeoutInSeconds: timeoutInSeconds).Select(x => x[0].CurrentMeasurement);

            // 5. Publish Results
            tsmContext.PublishResults(currentMeasurements, publishedDataID);
        }
    }
}
