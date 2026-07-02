using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// Provides an example method demonstrating TMU functionality using STL.
    /// </summary>
    public static class MeasurePeriodTMU
    {
        /// <summary>
        /// Demonstrates how to measure the period of a digital signal using the TMU.
        /// This example configures the TMU to measure period by detecting rising edges,
        /// collects the specified number of samples, and returns the averaged measurement.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle associated with the "C0" pin.</item>
        ///   <item>Assigns TMU resources to the specified pins.</item>
        ///   <item>Configures the TMU for period measurement with rising edge detection.</item>
        ///   <item>Initiates the TMU measurement.</item>
        ///   <item>Fetches and averages the measurement results.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Ensure that the pin map includes "C0" and that the hardware
        /// is properly configured before calling this method.
        /// </para>
        /// </remarks>
        /// <param name = "tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void MeasurePeriodWithSTL(ISemiconductorModuleContext tsmContext)
        {
            // Configuration parameters for TMU period measurement.
            int numberOfSamples = 100;           // Number of period samples to collect.
            double timeoutInSeconds = 5.0;       // Maximum time to wait for measurement completion.

            // Step 1: Query TSM session manager to get the digital sessions bundle associated with the "C0" pin.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital("C0");

            // Step 2: (Mandatory) Assign TMU resources to the digital pins.
            // This assign's a TMU resource to each of the pins in the digital sessions bundle object,
            // in this case just the "C0" pin.
            // Note that the TMU hardware resource is not reserved until step 3.
            digitalPins.AssignTMUResources();

            // Step 3: Configure the TMU to perform a period rise measurement.
            // - edgeType: Trigger on rising edge transitions.
            // - samplesToAcquire: Number of period measurements to collect.
            // - armType: Start measurement immediately without waiting for an arm event.
            digitalPins.ConfigurePeriodMeasurement(
                edgeType: TmuSourcePolarity.RisingEdge,
                samplesToAcquire: numberOfSamples,
                armType: TmuArmType.Immediate);

            // Step 4: Initiate the TMU measurement.
            digitalPins.TMUInitiate();

            // Step 5: Fetch the averaged measurement results.
            // The TMU collects multiple samples and returns the average period.
            PinSiteData<double> periodMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);

            // Step 6: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU();
            digitalPins.ClearTMUAssignment();
        }
    }
}
