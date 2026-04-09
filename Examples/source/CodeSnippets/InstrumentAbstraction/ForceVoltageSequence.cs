using System.Collections.Generic;
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
        /// This example demonstrates how to configure and initiate a hardware-timed voltage ramp sequence on SMU pins.
        /// It also demonstrates how to initiate and fetch the resulting current measurements taken during each step of the sequence.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">Names of the SMU pins to apply the voltage ramp and measure current.</param>
        internal static void ConfigureVoltageRampSequenceInitateAndFetchCurrentMeasurements(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);

            // Using both simple sequencing and advanced sequencing for the same channel within the same session is not supported.
            // It is for this reason that STL uses DCPower Advance Sequencing instead of creating simple sequences.
            // You must declare and manage each sequence you configure by name.
            // Note that there is a limit to how many sequences can be configured per instrument session (100 per session).
            // Therefore, it is best practice to properly keep track of all configured sequences by name,
            // and properly dispose of them once they no longer required.
            var sequenceName = "VoltageRampSequence";
            dcPowerPins.ConfigureVoltageSequence(sequenceName, voltageSequence, setAsActiveSequence: true);

            dcPowerPins.Initiate();

            // Measurements taken during the sequence execution can be fetched once the sequence finishes.
            // The fetched result contains the measured Current, Voltage, and In Compliance state values for each step of the sequence.
            // The Select method is used to extract just the Current values.
            PinSiteData<double[]> currentMeasurements = dcPowerPins.FetchMeasurement(pointsToFetch: 10)
                .Select(samples => samples.Select(sample => sample.CurrentMeasurement).ToArray());

            // Clearing the active advanced sequence after use.
            dcPowerPins.ClearActiveAdvancedSequence();
            // Deleting the advanced sequence after use to free up available sequences (limited to 100 per session).
            dcPowerPins.DeleteAdvancedSequence(sequenceName);
        }

        /// <summary>
        /// This example demonstrates how to force a hardware-timed voltage ramp sequence that is synchronized across pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage sequence on.</param>
        internal static void ForceSynchronizedVoltageRamp(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: 0, outputStop: 3, numberOfPoints: 10);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);
        }
    }
}
