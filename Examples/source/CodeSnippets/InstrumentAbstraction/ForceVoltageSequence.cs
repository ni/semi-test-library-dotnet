using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions.
    /// Specifically, how to force a voltage sequence on pins mapped to Source Measurement Unit (SMU) devices.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class ForceVoltageSequence
    {
        /// <summary>
        /// This example demonstrates how to force the same hardware-timed voltage ramp sequence to all SMU pins and sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        internal static void ForceVoltageRamp(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }

        /// <summary>
        /// This example demonstrates how to force the same hardware-timed voltage ramp sequence to all SMU pins and sites.
        /// It also demonstrates how to configure and fetch the resulting current measurements taken during each step of the sequence.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">Names of the SMU pins to apply the voltage ramp and measure current.</param>
        internal static void ForceVoltageRampFetchCurrentMeasurements(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequence(voltageSequence, waitForSequenceCompletion: true, sequenceTimeoutInSeconds: 20);

            // Measurements taken during the sequence execution can be fetched once the sequence finishes.
            // The fetched result contains the measured Current, Voltage, and In Compliance state values for each step of the sequence.
            // The Select method is used to extract just the Current values.
            PinSiteData<double[]> currentMeasurements = dcPowerPins.FetchMeasurement(pointsToFetch: 10)
                .Select(samples => samples.Select(sample => sample.CurrentMeasurement).ToArray());
        }

        /// <summary>
        /// This example demonstrates how to force a hardware-timed voltage ramp sequence that is synchronized across pins.
        /// It also demonstrates how to configure and fetch the resulting measurements to be taken during each step of the sequence.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        internal static void ForceSynchronizedVoltageRampAndFetchMeasurements(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureSettings(new DCPowerMeasureSettings() { MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete });

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            // Measurements taken during the sequence execution can be fetched once the sequence finishes.
            // The fetched result contains the measured Current, Voltage, and In Compliance state values for each step of the sequence.
            PinSiteData<SingleDCPowerFetchResult[]> fetchResults = dcPowerPins.FetchMeasurement(pointsToFetch: 10);
            // Use the Select method to extract the Current, Voltage, and In Compliance state values as needed.
            PinSiteData<double[]> currentMeasurements = fetchResults
                .Select(samples => samples.Select(sample => sample.CurrentMeasurement).ToArray());
            PinSiteData<double[]> voltageMeasurements = fetchResults
                .Select(samples => samples.Select(sample => sample.VoltageMeasurement).ToArray());
            PinSiteData<bool[]> inComplianceStates = fetchResults
                .Select(samples => samples.Select(sample => sample.InCompliance).ToArray());
        }
    }
}
