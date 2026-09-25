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
        /// Demonstrates how to measure the high duty cycle ratio of a digital signal using the PXIe-657x's TMU.
        /// The TMU measures the time duration the signal spends in the high state and the signal period,
        /// then divides the two to compute the duty cycle as a ratio (0.0 to 1.0).
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// <list type="number">
        ///   <item>Queries the TSM session manager to get the digital sessions bundle associated with the "C0" pin.</item>
        ///   <item>Assigns TMU resources to the specified pins.</item>
        ///   <item>Configures the TMU for high duty cycle time measurement and initiates it.</item>
        ///   <item>Fetches the averaged high duration result and publishes it using the "DutyCycleTime" Published Data ID.</item>
        ///   <item>Configures the TMU for period measurement and initiates it.</item>
        ///   <item>Fetches the averaged period result.</item>
        ///   <item>Divides the high duration by the period to obtain the duty cycle ratio and publishes the result using the "DutyCycle" Published Data ID.</item>
        ///   <item>Cleans up by disabling the TMU and clearing assignments.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The <see cref="TmuExtensions.ConfigureTMUDutyCycleMeasurement(DigitalSessionsBundle, TmuDutyCycle, long, TmuArmSetting, string[])"/> method returns a time duration in seconds,
        /// not a ratio or percentage. The duty cycle ratio is computed by dividing that duration by the signal period,
        /// which is measured separately using <see cref="TmuExtensions.ConfigurePeriodMeasurement(DigitalSessionsBundle, TmuPolarity, long, TmuArmSetting, string[])"/>.
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
            long numberOfSamples = 100;          // Number of samples to collect for each measurement.
            double timeoutInSeconds = 5.0;       // Maximum time to wait for measurement completion.

            // Step 1: Query TSM session manager to get the digital sessions bundle associated with the "C0" pin.
            var sessionManager = new TSMSessionManager(tsmContext);
            var digitalPins = sessionManager.Digital("C0");

            // Step 2: (Mandatory) Assign TMU resources to the digital pins.
            // This assigns a TMU resource to each of the pins in the digital sessions bundle object,
            // in this case just the "C0" pin.
            // Note that the TMU hardware resource is not reserved until step 3.
            digitalPins.AssignTMUResources();

            // Step 3: Configure the TMU to measure the high duration of the duty cycle.
            // - dutyCycleType: Measure the time from the rising edge to the subsequent falling edge at Voh.
            //   Alternatively, use TmuDutyCycle.Low to measure the time from the falling edge to the subsequent rising edge at Vol.
            // - samplesToAcquire: Number of duty cycle time measurements to collect.
            // This method also enables (reserves) the TMU resource at the hardware level.
            // Note: The returned measurement is a time duration in seconds, not a ratio or percentage.
            digitalPins.ConfigureTMUDutyCycleMeasurement(
                dutyCycleType: TmuDutyCycle.High,
                samplesToAcquire: numberOfSamples);

            // Step 4: Initiate the TMU measurement.
            digitalPins.TMUInitiate();

            // Step 5: Fetch the averaged measurement results.
            // The TMU collects multiple samples and returns the average high duration in seconds.
            PinSiteData<double> dutyCycleTimeMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);

            tsmContext.PublishResults(dutyCycleTimeMeasurements, publishedDataId: "DutyCycleTime");

            // Step 6: Measure the signal period, which is required to convert the high duration into a ratio.
            // DisableTMU must be called before reconfiguring.
            digitalPins.DisableTMU();

            digitalPins.ConfigurePeriodMeasurement(
                edgeType: TmuPolarity.RisingEdge,
                samplesToAcquire: numberOfSamples);

            digitalPins.TMUInitiate();
            PinSiteData<double> periodMeasurements = digitalPins.FetchAveragedTMUMeasurement(timeoutInSeconds);

            // Step 7: Divide the high duration by the period to calculate the duty cycle as a ratio.
            var dutyCycleMeasurements = dutyCycleTimeMeasurements.Divide(periodMeasurements);

            tsmContext.PublishResults(dutyCycleMeasurements, publishedDataId: "DutyCycleRatio");

            // Step 8: Clean up TMU resources.
            // Always disable the TMU and clear assignments when finished to free up resources.
            digitalPins.DisableTMU();
            digitalPins.ClearTMUAssignment();
        }
    }
}
