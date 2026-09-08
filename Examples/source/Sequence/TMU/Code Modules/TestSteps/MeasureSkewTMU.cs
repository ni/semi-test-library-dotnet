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
        /// Demonstrates how to measure the skew between a reference pin and a target pin using the PXIe-657x's TMU.
        /// Skew is defined as the time difference between the same edge type occurring on the reference
        /// channel and the target channel. A positive result means the target edge occurs after the
        /// reference edge; a negative result means it occurs before.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle containing both reference and target pins.</item>
        ///   <item>Assigns TMU resources to the reference pin only.</item>
        ///   <item>Configures the TMU for skew measurement using the reference and target pins.</item>
        ///   <item>Initiates the TMU measurement on the reference pin.</item>
        ///   <item>Fetches and averages the skew measurement results.</item>
        ///   <item>Publishes the averaged skew using the "Skew" published data id.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The <see cref="TmuExtensions.ConfigureTMUSkewMeasurement"/> method enables the TMU resource
        /// internally, so no separate <see cref="TmuExtensions.EnableTMU"/> call is required.
        /// </para>
        /// <para>
        /// Ensure that the pin map includes both "C0" and "C1" and that the hardware
        /// is properly configured before calling this method.
        /// </para>
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void MeasureSkewWithSTL(ISemiconductorModuleContext tsmContext)
        {
            // Configuration parameters for TMU skew measurement.
            int numberOfSamples = 100;              // Number of skew samples to collect.
            double timeoutInSeconds = 5.0;          // Maximum time to wait for measurement completion.

            // Reference and target pin names. The number of reference pins must equal the number of target pins.
            string[] referencePinNames = new[] { "C0" };
            string[] targetPinNames = new[] { "C1" };

            // Step 1: Query TSM session manager to get the digital sessions bundle.
            // The bundle must contain both reference and target pins.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital(new[] { "C0", "C1" });

            // Step 2: (Mandatory) Assign TMU resources to the reference pin(s).
            // Assigning TMU resources to the target pin(s) is not required, since only the
            // reference pin's TMU resource is used to perform the skew measurement.
            // Note that the TMU hardware resource is not reserved until step 3.
            digitalPins.AssignTMUResources(pinNames: referencePinNames);

            // Step 3: Configure the TMU to perform a skew measurement.
            // - referencePinNames: The pin(s) that act as the start (reference) source.
            // - targetPinNames: The pin(s) that act as the stop (target) source.
            // - edgeType: Trigger on rising edge transitions on both pins.
            // - samplesToAcquire: Number of skew measurements to collect.
            // - armSetting: Arm each sample on the edge of a signal with the same properties as the start source.
            // This method also enables (reserves) the TMU resource at the hardware level.
            digitalPins.ConfigureTMUSkewMeasurement(
                referencePinNames: referencePinNames,
                targetPinNames: targetPinNames,
                edgeType: TmuPolarity.RisingEdge,
                samplesToAcquire: numberOfSamples,
                armSetting: TmuArmSetting.StartEdge);

            // Step 4: Initiate the TMU measurement on the reference pin(s).
            digitalPins.TMUInitiate(pinNames: referencePinNames);

            // Step 5: Fetch the averaged skew measurement results, keyed by the reference pin(s).
            PinSiteData<double> skewMeasurements = digitalPins.FetchAveragedTMUMeasurement(
                timeoutInSeconds: timeoutInSeconds,
                pinNames: referencePinNames);
            tsmContext.PublishResults(skewMeasurements, publishedDataId: "Skew");

            // Step 6: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU(pinNames: referencePinNames);
            digitalPins.ClearTMUAssignment(pinNames: referencePinNames);
        }
    }
}
