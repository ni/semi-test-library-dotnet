using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
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
        /// Configure and initiate hardware-timed voltage ramp sequence on the specified SMU pins
        /// and fetches current measurements taken during each step of the sequence.
        /// Publishes the max current value across the steps using the "MaxCurrent" published data id.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force the voltage ramp on.</param>
        /// <param name="startVoltage">The starting value of a voltage ramp.</param>
        /// <param name="stopVoltage">The ending value of a voltage ramp.</param>
        /// <param name="numberOfSteps">The number of steps in the voltage ramp sequence.</param>
        public static void ConfigureVoltageRampSequenceInitiateAndFetchCurrentMeasurements(ISemiconductorModuleContext tsmContext, string[] smuPinNames, double startVoltage = 0, double stopVoltage = 3, int numberOfSteps = 10)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Measurements can be taken during sequence execution, with exactly one sample for each step,
            // but to enable this, the MeasureWhen property must be set to AutomaticallyAfterSourceComplete.
            dcPowerPins.ConfigureMeasureWhen(DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete);

            // Create a voltage ramp sequence from 0 to 3 volts with 10 points, which will create a sequence like [0V, 0.33V, 0.66V, ..., 3V]
            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: startVoltage, outputStop: stopVoltage, numberOfPoints: numberOfSteps);

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
            PinSiteData<SingleDCPowerFetchResult[]> fetchResults = dcPowerPins.FetchMeasurement(pointsToFetch: numberOfSteps);

            // Use the Select method to extract the Current, Voltage, and In Compliance state values as needed.
            PinSiteData<double> maxCurrentMeasurement = fetchResults
                .Select(samples => samples.Max(sample => sample.CurrentMeasurement));

            tsmContext.PublishResults(maxCurrentMeasurement, publishedDataId: "MaxCurrent");

            // Clearing the active advanced sequence after use.
            dcPowerPins.ClearActiveAdvancedSequence();

            // Deleting the advanced sequence after use to free up available sequences(limited to 100 per session).
            dcPowerPins.DeleteAdvancedSequence(sequenceName);
        }
    }
}
