using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// Provides an example method demonstrating TMU duty cycle measurement functionality using STL.
    /// </summary>
    public static class MeasureDutyCycleTMU
    {
        /// <summary>
        /// Demonstrates how to measure the high duty cycle of a digital signal using the TMU.
        /// The duty cycle measurement returns the time duration the signal spends in the high state
        /// (for <see cref="TmuDutyCycle.High"/>) or the low state (for <see cref="TmuDutyCycle.Low"/>).
        /// To convert the result to a percentage, divide the returned duration by the signal period.
        /// This measurement requires 1 comparator per pin.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle associated with the "C0" pin.</item>
        ///   <item>Assigns TMU resources to the specified pins.</item>
        ///   <item>Configures the TMU for duty cycle measurement.</item>
        ///   <item>Enables the TMU resource at the hardware level.</item>
        ///   <item>Initiates the TMU measurement.</item>
        ///   <item>Fetches and averages the measurement results.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Unlike <see cref="TmuExtensions.ConfigurePeriodMeasurement"/>, the
        /// <see cref="TmuExtensions.ConfigureTMUDutyCycleMeasurement"/> method does not enable the TMU.
        /// An explicit call to <see cref="TmuExtensions.EnableTMU"/> is required after configuration
        /// and before initiating the measurement.
        /// </para>
        /// <para>
        /// Ensure that the pin map includes "C0" and that the hardware
        /// is properly configured before calling this method.
        /// </para>
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void MeasureDutyCycleWithSTL(ISemiconductorModuleContext tsmContext)
        {
            // Configuration parameters for TMU duty cycle measurement.
            int numberOfSamples = 100;           // Number of duty cycle samples to collect.
            double timeoutInSeconds = 5.0;       // Maximum time to wait for measurement completion.

            // Step 1: Query TSM session manager to get the digital sessions bundle associated with the "C0" pin.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital("C0");

            // Step 2: (Mandatory) Assign TMU resources to the digital pins.
            // This assigns a TMU resource to each of the pins in the digital sessions bundle object,
            // in this case just the "C0" pin.
            // Note that the TMU hardware resource is not reserved until step 4.
            digitalPins.AssignTMUResources();

            // Step 3: Configure the TMU to perform a high duty cycle measurement.
            // - dutyCycleType: Measure the duration of the high portion of the cycle (rising to falling edge at Voh).
            //   Use TmuDutyCycle.Low to instead measure the duration of the low portion of the cycle.
            // - samplesToAcquire: Number of duty cycle measurements to collect.
            // - armType: Start measurement immediately without waiting for an arm event.
            // Note: This method does NOT enable (reserve) the TMU resource at the hardware level.
            // Note: The returned measurement value is a time duration, not a percentage.
            //       To convert to percentage duty cycle, divide by the signal period.
            digitalPins.ConfigureTMUDutyCycleMeasurement(
                dutyCycleType: TmuDutyCycle.High,
                samplesToAcquire: numberOfSamples,
                armType: TmuArmType.Immediate);

            // Step 4: Enable (reserve) the TMU resource at the hardware level.
            // This step is required when using ConfigureTMUDutyCycleMeasurement.
            digitalPins.EnableTMU();

            // Step 5: Initiate the TMU measurement.
            digitalPins.TMUInitiate();

            // Step 6: Fetch the averaged measurement results.
            // The TMU collects multiple samples and returns the average high duration in seconds.
            PinSiteData<double> dutyCycleMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);

            // Step 7: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU();
            digitalPins.ClearTMUAssignment();
        }
    }
}
