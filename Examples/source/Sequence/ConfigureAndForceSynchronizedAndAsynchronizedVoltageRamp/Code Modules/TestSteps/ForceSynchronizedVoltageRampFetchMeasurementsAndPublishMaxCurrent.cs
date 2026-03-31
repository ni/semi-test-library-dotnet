using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Collections.Generic;
using System.Linq;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class provides example methods demonstrating how to perform Hardware Level Sequencing with SMUs
    /// using DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Forces a hardware-timed voltage ramp sequence that is synchronized across the specified SMU pins
        /// and fetches measurements taken during each step of the sequence.
        /// Publishes the max current value across the steps using the "MaxCurrent" published data id.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force the voltage ramp on.</param>
        /// <param name="startVoltage">The starting value of a voltage ramp.</param>
        /// <param name="stopVoltage">The ending value of a voltage ramp.</param>
        /// <param name="numberOfSteps">The number of steps in the voltage ramp sequence.</param>
        public static void ForceSynchronizedVoltageRampFetchMeasurementsAndPublishMaxCurrent(ISemiconductorModuleContext tsmContext, string[] smuPinNames, double startVoltage = 0, double stopVoltage = 3, int numberOfSteps = 10)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: startVoltage, outputStop: stopVoltage, numberOfPoints: numberOfSteps);
            dcPowerPins.ForceVoltageSequenceSynchronized(voltageSequence);

            // Measurements taken during the sequence execution can be fetched once the sequence finishes.
            // The fetched result contains the measured Current, Voltage, and In Compliance state values for each step of the sequence.
            PinSiteData<SingleDCPowerFetchResult[]> fetchResults = dcPowerPins.FetchMeasurement(pointsToFetch: numberOfSteps);

            // Use the Select method to extract the Current, Voltage, and In Compliance state values as needed.
            PinSiteData<double> maxCurrentMeasurement = fetchResults
                .Select(samples => samples.Max(sample => sample.CurrentMeasurement));

            tsmContext.PublishResults(maxCurrentMeasurement, publishedDataId: "MaxCurrent");

            // Disabling StartTrigger post using ForceVoltageSequenceSynchronized and fetching the measurements
            // to clean up and avoid any unintended consequences on later test steps that may use the same pins.
            dcPowerPins.DisableTriggers(new List<TriggerType> { TriggerType.StartTrigger });
        }
    }
}
