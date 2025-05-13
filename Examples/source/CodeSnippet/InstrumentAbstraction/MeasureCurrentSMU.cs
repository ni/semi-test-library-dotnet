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
    /// This class, and it's methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
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

        internal static void MeasureCurrentDoMathThenPublishSmu(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(smuPinNames);
            var staticOffset = 2.5;
            var staticGain = 0.15;

            // Assumes that the SMU is already configured.
            var measurements = smuPins.MeasureAndPublishCurrent(publishedDataId: "RawMeasurement");
            var gainAndOffsetApplied = measurements.Add(staticGain).Multiply(staticOffset);

            tsmContext.PublishResults(gainAndOffsetApplied, publishedDataId: "MeasurementWithGainAndOffsetApplied");
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
