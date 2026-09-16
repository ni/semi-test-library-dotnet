using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.TMU
{
    /// <summary>
    /// This class provides example methods demonstrating how to perform Time Measurement Unit (TMU) measurements
    /// using Digital Instrument Abstraction methods from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Demonstrates how to measure the rise time of a digital signal using the PXIe-657x's TMU.
        /// Rise time is defined as the time for a signal to transition from the low voltage
        /// threshold (Vol) to the high voltage threshold (Voh).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle associated with the "C0" pin.</item>
        ///   <item>Assigns TMU resources to the specified pins.</item>
        ///   <item>Configures the TMU for rise time measurement.</item>
        ///   <item>Initiates the TMU measurement.</item>
        ///   <item>Fetches and averages the measurement results.</item>
        ///   <item>Publishes the averaged rise time using the "RiseTime" published data id.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The <see cref="TmuExtensions.ConfigureTMURiseTimeMeasurement"/> method enables the TMU resource
        /// internally, so no separate <see cref="TmuExtensions.EnableTMU"/> call is required.
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
            // Note that the TMU hardware resource is not reserved until step 3.
            digitalPins.AssignTMUResources();

            // Step 3: Configure the TMU to perform a rise time measurement.
            // Sets the start source to Vol on rising edge and the stop source to Voh on rising edge.
            // - samplesToAcquire: Number of rise time measurements to collect.
            // - armSetting: Arm each sample on the edge of a signal with the same properties as the start source.
            // This method also enables (reserves) the TMU resource at the hardware level.
            digitalPins.ConfigureTMURiseTimeMeasurement(
                samplesToAcquire: numberOfSamples,
                armSetting: TmuArmSetting.StartEdge);

            // Step 4: Initiate the TMU measurement.
            digitalPins.TMUInitiate();

            // Step 5: Fetch the averaged measurement results.
            // The TMU collects multiple samples and returns the average rise time.
            PinSiteData<double> riseTimeMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);
            tsmContext.PublishResults(riseTimeMeasurements, publishedDataId: "RiseTime");

            // Step 6: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU();
            digitalPins.ClearTMUAssignment();
        }
    }
}
