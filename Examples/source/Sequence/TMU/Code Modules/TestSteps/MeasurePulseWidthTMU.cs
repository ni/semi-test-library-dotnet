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
        /// Demonstrates how to measure the high pulse width of a digital signal using the PXIe-657x's TMU.
        /// Pulse width measures the duration of a single pulse — the time from the rising edge to
        /// the subsequent falling edge at Voh (for <see cref="TmuPulseWidth.High"/>).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle associated with the "C0" pin.</item>
        ///   <item>Assigns TMU resources to the specified pins.</item>
        ///   <item>Configures the TMU for pulse width measurement.</item>
        ///   <item>Initiates the TMU measurement.</item>
        ///   <item>Fetches and averages the measurement results.</item>
        ///   <item>Publishes the averaged pulse width using the "PulseWidth" published data id.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The <see cref="TmuExtensions.ConfigureTMUPulseWidthMeasurement"/> method enables the TMU resource
        /// internally, so no separate <see cref="TmuExtensions.EnableTMU"/> call is required.
        /// </para>
        /// <para>
        /// Ensure that the pin map includes "C0" and that the hardware
        /// is properly configured before calling this method.
        /// </para>
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void MeasurePulseWidthWithSTL(ISemiconductorModuleContext tsmContext)
        {
            // Configuration parameters for TMU pulse width measurement.
            int numberOfSamples = 100;           // Number of pulse width samples to collect.
            double timeoutInSeconds = 5.0;       // Maximum time to wait for measurement completion.

            // Step 1: Query TSM session manager to get the digital sessions bundle associated with the "C0" pin.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital("C0");

            // Step 2: (Mandatory) Assign TMU resources to the digital pins.
            // This assigns a TMU resource to each of the pins in the digital sessions bundle object,
            // in this case just the "C0" pin.
            // Note that the TMU hardware resource is not reserved until step 3.
            digitalPins.AssignTMUResources();

            // Step 3: Configure the TMU to perform a high pulse width measurement.
            // - pulseWidthType: Measure the duration from the rising edge to the subsequent falling edge at Voh.
            //   Alternatively, use TmuPulseWidth.Low to measure from the falling edge to the subsequent rising edge at Vol.
            // - samplesToAcquire: Number of pulse width measurements to collect.
            // This method also enables (reserves) the TMU resource at the hardware level.
            digitalPins.ConfigureTMUPulseWidthMeasurement(
                pulseWidthType: TmuPulseWidth.High,
                samplesToAcquire: numberOfSamples);

            // Step 4: Initiate the TMU measurement.
            digitalPins.TMUInitiate();

            // Step 5: Fetch the averaged measurement results.
            // The TMU collects multiple samples and returns the average high pulse width in seconds.
            PinSiteData<double> pulseWidthMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);
            tsmContext.PublishResults(pulseWidthMeasurements, publishedDataId: "PulseWidth");

            // Step 6: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU();
            digitalPins.ClearTMUAssignment();
        }
    }
}
