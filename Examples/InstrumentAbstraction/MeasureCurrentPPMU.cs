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
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked.
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
            var measurements = ppmuPins.MeasureAndPublishCurrent("RawMeasurement");
            var gainAndOffsetApplied = measurements.Add(staticGain).Multiply(staticOffset);

            tsmContext.PublishResults(gainAndOffsetApplied, "MeasurementWithGainAndOffsetApplied");
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
