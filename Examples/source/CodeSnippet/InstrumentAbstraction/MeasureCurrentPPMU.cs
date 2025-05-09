using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to measure current for pins mapped to Digital Pattern, using the instrument's PPMU function mode.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class MeasureCurrentPPMU
    {
        internal static void SimpleMeasureCurrentPpmu(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(ppmuPinNames);

            // Assumes that the SMU is already configured.
            var measurements = ppmuPins.MeasureCurrent();
        }

        internal static void MeasureAndPublishCurrentPpmu(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames, string publishDataID)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(ppmuPinNames);

            // Assumes that the SMU is already configured.
            var measurements = ppmuPins.MeasureAndPublishCurrent(publishDataID);
        }

        internal static void MeasureCurrentDoMathThenPublishPpmu(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(ppmuPinNames);
            var staticOffset = 2.5;
            var staticGain = 0.15;

            // Assumes that the SMU is already configured.
            var measurements = ppmuPins.MeasureAndPublishCurrent(publishedDataId: "RawMeasurement");
            var gainAndOffsetApplied = measurements.Add(staticGain).Multiply(staticOffset);

            tsmContext.PublishResults(gainAndOffsetApplied, publishedDataId: "MeasurementWithGainAndOffsetApplied");
        }

        internal static void MeasureCurrentTwoPinsPublishDifferencePpmu(ISemiconductorModuleContext tsmContext)
        {
            var ppmuPinNames = new string[] { "PinA", "PinB" };
            var publishDataIDs = new string[] { "Measurement", "Difference" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.Digital(ppmuPinNames);

            // Assumes that the SMU is already configured.
            var measurements = smuPins.MeasureAndPublishCurrent(publishDataIDs[0]);

            var pinAMeasurement = measurements.ExtractPin(ppmuPinNames[0]);
            var pinBMeasurement = measurements.ExtractPin(ppmuPinNames[1]);
            var difference = pinAMeasurement.Subtract(pinBMeasurement);
            tsmContext.PublishResults(difference, publishDataIDs[1]);
        }
    }
}
