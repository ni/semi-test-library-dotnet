using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.TMU;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// Provides an example method demonstrating TMU skew measurement functionality using STL.
    /// </summary>
    public static class MeasureSkewTMU
    {
        /// <summary>
        /// Demonstrates how to measure the skew between a reference pin and a target pin using the TMU.
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
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The <see cref="TmuExtensions.ConfigureSkewMeasurement"/> method enables the TMU resource
        /// internally, so no separate <see cref="TmuExtensions.EnableTMU"/> call is required.
        /// </para>
        /// <para>
        /// Ensure that the pin map includes both "CLK_REF" and "CLK_OUT" and that the hardware
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
            string[] referencePinNames = new[] { "CLK_REF" };
            string[] targetPinNames = new[] { "CLK_OUT" };

            // Step 1: Query TSM session manager to get the digital sessions bundle.
            // The bundle must contain both reference and target pins.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital(new[] { "CLK_REF", "CLK_OUT" });

            // Step 2: (Mandatory) Assign TMU resources to the reference pin(s) only.
            // The TMU resource is managed from the reference pin for skew measurements.
            // Note that the TMU hardware resource is not reserved until step 3.
            digitalPins.AssignTMUResources(pinNames: referencePinNames);

            // Step 3: Configure the TMU to perform a skew measurement.
            // - referencePinNames: The pin(s) that act as the start (reference) source.
            // - targetPinNames: The pin(s) that act as the stop (target) source.
            // - edgeType: Trigger on rising edge transitions on both pins.
            // - samplesToAcquire: Number of skew measurements to collect.
            // - armType: Start measurement immediately without waiting for an arm event.
            // This method also enables (reserves) the TMU resource at the hardware level.
            digitalPins.ConfigureSkewMeasurement(
                referencePinNames: referencePinNames,
                targetPinNames: targetPinNames,
                edgeType: TmuPolarity.RisingEdge,
                samplesToAcquire: numberOfSamples,
                armType: TmuArmType.Immediate);

            // Step 4: Initiate the TMU measurement on the reference pin(s).
            digitalPins.TMUInitiate(pinNames: referencePinNames);

            // Step 5: Fetch the averaged skew measurement results, keyed by the reference pin(s).
            PinSiteData<double> skewMeasurements = digitalPins.FetchAveragedTMUMeasurement(
                timeoutInSeconds: timeoutInSeconds,
                pinNames: referencePinNames);

            // Step 6: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU(pinNames: referencePinNames);
            digitalPins.ClearTMUAssignment(pinNames: referencePinNames);
        }
    }
}