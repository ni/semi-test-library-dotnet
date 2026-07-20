using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// Provides an example method demonstrating TMU rise time measurement functionality using STL.
    /// </summary>
    public static class MeasureRiseTimeTMU
    {
        /// <summary>
        /// Demonstrates how to measure the rise time of a digital signal using the TMU.
        /// Rise time is defined as the time for a signal to transition from the low voltage
        /// threshold (Vol) to the high voltage threshold (Voh). This measurement requires
        /// 2 comparators per pin.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle associated with the "C0" pin.</item>
        ///   <item>Assigns TMU resources to the specified pins.</item>
        ///   <item>Configures the TMU for rise time measurement.</item>
        ///   <item>Enables the TMU resource at the hardware level.</item>
        ///   <item>Initiates the TMU measurement.</item>
        ///   <item>Fetches and averages the measurement results.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Unlike <see cref="TmuExtensions.ConfigurePeriodMeasurement"/>, the
        /// <see cref="TmuExtensions.ConfigureTMURiseTimeMeasurement"/> method does not enable the TMU.
        /// An explicit call to <see cref="TmuExtensions.EnableTMU"/> is required after configuration
        /// and before initiating the measurement.
        /// </para>
        /// <para>
        /// Ensure that the pin map includes "C0" and that the hardware
        /// is properly configured before calling this method.
        /// </para>
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void MeasureRiseTimeWithSTL(ISemiconductorModuleContext tsmContext)
        {
            // Configuration parameters for TMU rise time measurement.
            int numberOfSamples = 100;           // Number of rise time samples to collect.
            double timeoutInSeconds = 5.0;       // Maximum time to wait for measurement completion.

            // Step 1: Query TSM session manager to get the digital sessions bundle associated with the "C0" pin.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital("C0");

            // Step 2: (Mandatory) Assign TMU resources to the digital pins.
            // This assigns a TMU resource to each of the pins in the digital sessions bundle object,
            // in this case just the "C0" pin.
            // Note that the TMU hardware resource is not reserved until step 4.
            digitalPins.AssignTMUResources();

            // Step 3: Configure the TMU to perform a rise time measurement.
            // Sets the start source to Vol on rising edge and the stop source to Voh on rising edge.
            // - samplesToAcquire: Number of rise time measurements to collect.
            // - armType: Start measurement immediately without waiting for an arm event.
            // Note: This method does NOT enable (reserve) the TMU resource at the hardware level.
            digitalPins.ConfigureTMURiseTimeMeasurement(
                samplesToAcquire: numberOfSamples,
                armType: TmuArmType.Immediate);

            // Step 4: Enable (reserve) the TMU resource at the hardware level.
            // This step is required when using ConfigureTMURiseTimeMeasurement.
            digitalPins.EnableTMU();

            // Step 5: Initiate the TMU measurement.
            digitalPins.TMUInitiate();

            // Step 6: Fetch the averaged measurement results.
            // The TMU collects multiple samples and returns the average rise time.
            PinSiteData<double> riseTimeMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);

            // Step 7: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU();
            digitalPins.ClearTMUAssignment();
        }
    }
}