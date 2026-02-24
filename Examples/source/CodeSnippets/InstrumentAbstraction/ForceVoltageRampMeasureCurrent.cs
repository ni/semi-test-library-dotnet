using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
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
    public static class ForceVoltageRampMeasureCurrent
    {
        /// <summary>
        /// Applies a voltage ramp to specified SMU pins and measures the resulting current.
        /// </summary>
        /// <param name="tsmContext">Context for the semiconductor module operations.</param>
        /// <param name="smuPinNames">Names of the SMU pins to apply the voltage ramp and measure current.</param>
        /// <param name="publishedDataID">ID for the published data.</param>
        public static void ForceVoltageRampMeasureCurrentExample(ISemiconductorModuleContext tsmContext, string[] smuPinNames, string publishedDataID = "ForceVoltageRampMeasureCurrent")
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            dcPowerPins.ForceVoltageSequence(voltageSequence, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 20);

            var measurement = dcPowerPins.FetchMeasurement(pointsToFetch: voltageSequence.Length).Select(x => x[0].VoltageMeasurement);
            tsmContext.PublishResults(measurement, publishedDataID);
        }
    }
}
