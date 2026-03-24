using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions. Specifically, how to force a synchronized voltage ramp on pins mapped to Source Measurement Unit (SMU) devices.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    public static class ForceVoltageSequence
    {
        /// <summary>
        /// This example demonstrates how to force the same hardware-timed voltage ramp sequence to all SMU pins and sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void ForceVoltageRamp(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);

            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);
            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }

        /// <summary>
        /// This example demonstrates how to force a hardware-timed voltage ramp sequence that is synchronized across pins.
        /// It also demonstrates how to configure and fetch the resulting measurements to be taken during each step of the sequence.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        public static void ForceSynchronizedVoltageRampAndFetch(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);

            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            PinSiteData<SingleDCPowerFetchResult[]> result = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
        }

        /// <summary>
        /// Applies a voltage ramp to specified SMU pins using <see cref="Source.ForceVoltageSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/> and measures the resulting current.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">Names of the SMU pins to apply the voltage ramp and measure current.</param>
        /// <param name="publishedDataID">ID for the published data.</param>
        public static void ForceVoltageRampMeasureCurrent(ISemiconductorModuleContext tsmContext, string[] smuPinNames, string publishedDataID = "ForceVoltageRampMeasureCurrent")
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);

            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);

            dcPowerPins.ForceVoltageSequence(voltageSequence, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 20);

            PinSiteData<double> measurement = dcPowerPins.FetchMeasurement(pointsToFetch: voltageSequence.Length).Select(x => x[0].CurrentMeasurement);

            tsmContext.PublishResults(measurement, publishedDataID);
        }
    }
}
