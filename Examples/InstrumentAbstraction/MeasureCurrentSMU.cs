using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to measure current for pins mapped to DCPower Instruments.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked.
    /// </summary>
    internal static class MeasureCurrentSMU
    {
        internal static void SimpleMeasureCurrentSmu(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(smuPinNames);

            // Assumes that the SMU is already configured.
            var measurements = smuPins.MeasureCurrent();
        }

        internal static void MeasureAndPublishCurrentSmu(ISemiconductorModuleContext tsmContext, string[] smuPinNames, string publishDataID)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(smuPinNames);

            // Assumes that the SMU is already configured.
            var measurements = smuPins.MeasureAndPublishCurrent(publishDataID);
        }

        internal static void MeasureCurrentDoMathThePublishSmu(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(smuPinNames);
            var staticOffset = 2.5;
            var staticGain = 0.15;

            // Assumes that the SMU is already configured.
            var measurements = smuPins.MeasureAndPublishCurrent("RawMeasurement");
            var gainAndOffsetApplied = measurements.Add(staticGain).Multiply(staticOffset);

            tsmContext.PublishResults(gainAndOffsetApplied, "MeasurementWithGainAndOffsetApplied");
        }

        internal static void MeasureCurrentTwoPinsPublishDifferenceSmu(ISemiconductorModuleContext tsmContext)
        {
            var smuPinNames = new string[] { "PinA", "PinB" };
            var publishDataIDs = new string[] { "Measurement", "Difference" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(smuPinNames);

            // Assumes that the SMU is already configured.
            var measurements = smuPins.MeasureAndPublishCurrent(publishDataIDs[0]);

            var pinAMeasurement = measurements.ExtractPin(smuPinNames[0]);
            var pinBMeasurement = measurements.ExtractPin(smuPinNames[1]);
            var difference = pinAMeasurement.Subtract(pinBMeasurement);
            tsmContext.PublishResults(difference, publishDataIDs[1]);
        }
    }
}
