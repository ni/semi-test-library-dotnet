using System;
using System.Globalization;
using System.Linq;
using Ivi.Visa;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.Visa;

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
        /// it does not demonstrate cross-chassis trigger routing.
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
            var timeoutInSeconds = 10.0;
            var triggerLine = DmmTriggerSource.Ttl0; // TTL0 terminal names used for DMM resources
            var triggerLineNumber = int.Parse(triggerLine.ToString().Last().ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            // 2. Use the TSMSessionManager to query sessions for target pins.
            var digitalPins = sessionManager.Digital(digitalPinNames);
            var dmmPins = sessionManager.DMM(dmmPinNames);

            // 3a. Configure the DMM Sessions and configure trigger source.
            dmmPins.ConfigureMeasurementDigits(DmmMeasurementFunction.DCVolts, range: 5.0, resolutionDigits: 7.5);
            dmmPins.ConfigureApertureTime(apertureTime: 0.001);
            dmmPins.ConfigureAutoZero(DmmAuto.On);
            dmmPins.ConfigureTrigger(triggerLine, 0.0);
            dmmPins.Do(x =>
            {
                x.Session.Trigger.Slope = DmmSlope.Negative;
            });
            // 3b. Get Fully Qualified Terminal Names to direct triggers
            var sourceResource = digitalPins.InstrumentSessions.ElementAt(0).Session.DriverOperation.IOResourceDescriptor;
            var destinationResources = dmmPins.InstrumentSessions.Select(x => x.Session.DriverOperation.IOResourceDescriptor).ToArray();
            // 3c. Export the pattern event0 to generate the digital trigger
            digitalPins.ExportSignal(SignalType.PatternOpcodeEvent, $"patternOpcodeEvent{patternOpcodeEvent}", $"PXI_Trig{triggerLineNumber}");
            // 3d. Connect terminals from exported terminal to DMM trigger line - Assumes the DMM(s) and the Digital Instrument(s) are all in the same PXIChassis.
            // This manual step is required to properly route exported trigger signals to DMM instruments, since the niDMM driver does not support dynamic string-based trigger routing.
            RoutePXITriggerAcrossChassisSegments(triggerLineNumber, sourceResource, destinationResources, out var triggerSession, out var sourceSegment, out var destinationSegment);

            // 4a. Initiate the DMM (non-blocking), the DMM will await the configured trigger before measuring.
            dmmPins.Initiate();

            // 4b. Burst the pattern that generates the digital trigger (event0)
            digitalPins.BurstPattern(triggerPatternName);

            // 4c. Fetch
            var measurements = dmmPins.Fetch(maximumTimeInMilliseconds: timeoutInSeconds * 1000);

            // 5. Publish Results
            tsmContext.PublishResults(measurements, publishedDataID);

            // 6. Cleanup - disconnect routed terminals
            UnroutePXITriggerAcrossChassisSegments(triggerLineNumber, triggerSession, sourceSegment, destinationSegment);
        }

        private static void RoutePXITriggerAcrossChassisSegments(
            int triggerLineNumber,
            string sourceDeviceResource,
            string[] destinationDeviceResources,
            out PxiBackplane pxiSession,
            out short sourceSegment,
            out short destinationSegment)
        {
            sourceSegment = -1;
            destinationSegment = -1;
            GetDeviceInfo(
                sourceDeviceResource,
                destinationDeviceResources,
                out int sourceChassisNumber,
                out sourceSegment,
                out int[] destinationChassisNumbers,
                out short[] destinationSegments);
            for (int i = 0; i < destinationDeviceResources.Length; i++)
            {
                if (destinationChassisNumbers[i] != sourceChassisNumber)
                {
                    throw new NotSupportedException("Devices are not in same chassis. Routing triggers across chassis is not current supported by this code");
                }
            }

            using (var rmSession = new ResourceManager())
            {
                // Chassis identifier as Visa Resource Name. Can be seen in NI MAX.
                pxiSession = (PxiBackplane)rmSession.Open($"PXI0::{sourceChassisNumber}::BACKPLANE");
            }
            // The following logic will determine which bus segments to map together.
            // It assumes all devices are in the same chassis, and will choose the worse case route (i.e. segment 1 to 3).
            if (sourceSegment < destinationSegments.Max())
            {
                destinationSegment = destinationSegments.Max();
            }
            if (sourceSegment >= destinationSegments.Max())
            {
                destinationSegment = destinationSegments.Min();
            }
            pxiSession.MapTrigger(sourceSegment, (TriggerLine)triggerLineNumber, destinationSegment, (TriggerLine)triggerLineNumber);
        }

        private static void UnroutePXITriggerAcrossChassisSegments(int triggerLineNumber, PxiBackplane pxiSession, short sourceSegment, short destinationSegment)
        {
            pxiSession.UnmapTrigger(sourceSegment, (TriggerLine)triggerLineNumber, destinationSegment, (TriggerLine)triggerLineNumber);
            pxiSession.Dispose();
        }

        private static void GetDeviceInfo(
            string sourceDeviceResourceString,
            string[] destinationDeviceResourceStrings,
            out int sourceChassisNumber,
            out short sourceSegment,
            out int[] destinationChassisNumbers,
            out short[] destinationSegments)
        {
            sourceChassisNumber = -1;
            sourceSegment = -1;
            destinationChassisNumbers = new int[destinationDeviceResourceStrings.Length];
            destinationSegments = new short[destinationDeviceResourceStrings.Length];
            var modularInstrumentsSystem = new ModularInstrumentsSystem();
            foreach (DeviceInfo deviceInfo in modularInstrumentsSystem.DeviceCollection)
            {
                if (deviceInfo.Name.Equals(sourceDeviceResourceString))
                {
                    sourceChassisNumber = deviceInfo.ChassisNumber;
                    sourceSegment = GetChassisSegment(deviceInfo.SlotNumber);
                }
                if (destinationDeviceResourceStrings.Contains(deviceInfo.Name))
                {
                    int index = Array.IndexOf(destinationDeviceResourceStrings, deviceInfo.Name);
                    destinationChassisNumbers[index] = deviceInfo.ChassisNumber;
                    destinationSegments[index] = GetChassisSegment(deviceInfo.SlotNumber);
                }
            }
        }

        private static short GetChassisSegment(int slotNumber)
        {
            if (slotNumber <= 6)
            {
                return 1;
            }
            if (slotNumber <= 12)
            {
                return 2;
            }
            if (slotNumber <= 18)
            {
                return 3;
            }
            return -1;
        }
    }
}
