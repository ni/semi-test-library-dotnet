using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
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
        /// This example demonstrates how to make a DMM Measurement from a trigger sent from a pattern.
        /// This example assumes the DMM(s) and the Digital instrument(s) are all in the same PXIChassis,
        /// which supports cross-chassis trigger routing.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module Context object.</param>
        /// <param name="digitalPinNames">Name of digital pins used in the target pattern</param>
        /// <param name="dmmPinNames">Name of DMM Pins to be measured</param>
        /// <param name="triggerPatternName">Name of the pattern to burst that will trigger the measurements</param>
        /// <param name="patternOpcodeEvent">The pattern opcode event number, 0-3 (default: 0)</param>
        /// <param name="publishedDataID">The publishedDataID to use for the published measurement results (Default: DmmMeasurements).</param>
        public static void PatternTriggeredDmmMeasurement(
            ISemiconductorModuleContext tsmContext,
            string[] digitalPinNames,
            string[] dmmPinNames,
            string triggerPatternName,
            int patternOpcodeEvent = 0,
            string publishedDataID = "DmmMeasurements")
        {
            // 1. Create a new TSMSessionManager object and any other local variables required for the test.
            // Note that values maybe hard coded for demonstration purposes and should otherwise be replaced with appropriate parameter inputs.
            var sessionManager = new TSMSessionManager(tsmContext);
            var timeoutInSeconds = 1.0;

            // 2. Use the TSMSessionManager to query sessions for target pins.
            var digitalPins = sessionManager.Digital(digitalPinNames);
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // 3a. Configure the DMM Sessions and configure trigger source.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 5.0, resolutionDigits: 7.5);
            dmmPins.ConfigureApertureTime(apertureTime: 0.001);
            dmmPins.ConfigureAutoZero(DmmAuto.On);
            dmmPins.ConfigureTrigger(DmmTriggerSource.Ttl0, 0.0);
            dmmPins.Do(x =>
            {
                x.Session.Trigger.Slope = DmmSlope.Positive;
            });
            // 3b. Get Fully Qualified Terminal Names to direct triggers
            var sourcefullyQualifiedTerminalName = digitalPins.InstrumentSessions.ElementAt(0).Session.Event.PatternOpcodeEvents[patternOpcodeEvent].TerminalName;
            // 3c. Connect terminals from exported terminal to DMM trigger line - Assumes the DMM(s) and the Digital instrument(s) are all in the same PXIChassis
            DaqSystem.Local.ConnectTerminals(sourcefullyQualifiedTerminalName, "PXITrig_0");

            // 4a. Initiate the DMM (non-blocking), the DMM will await the configured trigger before measuring.
            dmmPins.Initiate();

            // 4b. Burst the pattern that generates the digital trigger (event0)
            digitalPins.BurstPattern(triggerPatternName);

            // 4c. Fetch
            var measurements = dmmPins.Fetch(maximumTimeInMilliseconds: timeoutInSeconds * 1000);

            // 5. Publish Results
            tsmContext.PublishResults(measurements, publishedDataID);

            // 6. Cleanup - disconnect routed terminals
            DaqSystem.Local.DisconnectTerminals(sourcefullyQualifiedTerminalName, "PXITrig_0");
        }

        /// <summary>
        /// This example demonstrates how to make a DMM Measurement from a trigger sent from a pattern.
        /// This example leverages the PXIe-6674T Timing and Synchronization module to connect trigger routes,
        /// which supports cross-chassis trigger routing.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module Context object.</param>
        /// <param name="digitalPinNames">Name of digital pins used in the target pattern</param>
        /// <param name="dmmPinNames">Name of DMM Pins to be measured</param>
        /// <param name="triggerPatternName">Name of the pattern to burst that will trigger the measurements</param>
        /// <param name="patternOpcodeEvent">The pattern opcode event number, 0-3 (default: 0)</param>
        /// <param name="publishedDataID">The publishedDataID to use for the published measurement results (Default: DmmMeasurements).</param>
        public static void PatternTriggeredDmmMeasurementWithSync(
            ISemiconductorModuleContext tsmContext,
            string[] digitalPinNames,
            string[] dmmPinNames,
            string triggerPatternName,
            int patternOpcodeEvent = 0,
            string publishedDataID = "DmmMeasurements")
        {
            // 1. Create a new TSMSessionManager object and any other local variables required for the test.
            // Note that values maybe hard coded for demonstration purposes and should otherwise be replaced with appropriate parameter inputs.
            var sessionManager = new TSMSessionManager(tsmContext);
            var timeoutInSeconds = 1.0;
            var triggerLine = DmmTriggerSource.Ttl0; // TTL0 terminal names for DMM resources

            // 2. Use the TSMSessionManager to query sessions for target pins.
            var digitalPins = sessionManager.Digital(digitalPinNames);
            var dmmPins = sessionManager.DMM(dmmPinNames);
            var syncDevice = sessionManager.Sync();

            // 3a. Configure the DMM Sessions and configure trigger source.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 5.0, resolutionDigits: 7.5);
            dmmPins.ConfigureApertureTime(apertureTime: 0.001);
            dmmPins.ConfigureAutoZero(DmmAuto.On);
            dmmPins.ConfigureTrigger(triggerLine, 0.0);
            dmmPins.Do(x =>
            {
                x.Session.Trigger.Slope = DmmSlope.Positive;
            });
            // 3b. Get Fully Qualified Terminal Names to direct triggers
            var sourcefullyQualifiedTerminalName = digitalPins.InstrumentSessions.ElementAt(0).Session.Event.PatternOpcodeEvents[patternOpcodeEvent].TerminalName;
            var destinationfullyQualifiedTerminalNames = dmmPins.InstrumentSessions.Select(x => $"/{x.Session.DriverOperation.IOResourceDescriptor}/{triggerLine.ToString()}");
            // 3c. Connect terminals from exported terminal to DMM trigger line - Assumes the DMM(s) and the Digital Instrument(s) are all in the same PXIChassis
            foreach (var terminal in destinationfullyQualifiedTerminalNames)
            {
                syncDevice.Do(x => { x.Session.ConnectTriggerTerminals(sourcefullyQualifiedTerminalName, terminal); });
            }

            // 4a. Initiate the DMM (non-blocking), the DMM will await the configured trigger before measuring.
            dmmPins.Initiate();

            // 4b. Burst the pattern that generates the digital trigger (event0)
            digitalPins.BurstPattern(triggerPatternName);

            // 4c. Fetch
            var measurements = dmmPins.Fetch(maximumTimeInMilliseconds: timeoutInSeconds * 1000);

            // 5. Publish Results
            tsmContext.PublishResults(measurements, publishedDataID);

            // 6. Cleanup - disconnect routed terminals
            foreach (var terminal in destinationfullyQualifiedTerminalNames)
            {
                syncDevice.Do(x => { x.Session.DisconnectTriggerTerminals(sourcefullyQualifiedTerminalName, terminal); });
            }
        }
    }
}
