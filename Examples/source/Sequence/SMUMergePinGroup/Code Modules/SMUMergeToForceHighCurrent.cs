using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.SMUMergePinGroup
{
    /// <summary>
    /// This class contains example of how to use the MergePinGroup and UnmergePinGroup functions in the
    /// Instrument Abstractions from the Semiconductor Test Library. Specifically, how to merge Pin Groups
    /// for forcing high current and measure voltage for pins mapped to DCPower Instruments from same module.
    /// Later unmerge the pin group to disconnect the pins and instrumentation after the test is complete.
    /// </summary>
    /// <remarks>
    /// Note that only few DCPower Instruments support merging feature which includes PXIe-4147, PXIe-4162 and PXIe-4163.
    /// This also requires the pinmap to define the pingroup to contain all the pins to be merged and each
    /// pin is mapped to SMU channels of same module.
    /// This example assumes:
    ///  - Relay configurations to be applied before the merging operation ensures the SMUs channels are connected
    ///  in parallel configuration and disconnect after unmerging.
    /// </remarks>
    public static partial class ExampleTestSteps
    {
        /// <summary>
        /// Merges the specified pin group to force high current and measure voltage for pins mapped to DCPower Instruments from same module.
        /// Specifically, this method merges the pin group, forces a voltage level, measures the current, and then unmerges the pin group.
        /// </summary>
        /// <param name="tsmContext">Teststand Semiconductor module context</param>
        /// <param name="pinGroup">Name of the pin group to be merged</param>
        /// <param name="voltageLevel">Voltage level to set output</param>
        /// <param name="currentLimit">Current limit for output</param>
        /// <param name="settlingTime">Settling time used for measurements</param>
        /// <param name="apertureTime">Aperture time used for measurements</param>
        /// <param name="connectedRelayConfiguration">Relay configuration that connects all the channels in parallel</param>
        /// <param name="disconnectedRelayConfiguration">Relay configuration that disconnects all the channels</param>
        /// <returns>PinSiteData measurement  in double precision </returns>
        public static void SMUMergeToForceHighCurrent(
            ISemiconductorModuleContext tsmContext,
            string pinGroup,
            double voltageLevel = 3.3,
            double currentLimit = 0.01,
            double settlingTime = 0.001,
            double apertureTime = -1,
            string connectedRelayConfiguration = "",
            string disconnectedRelayConfiguration = "")
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(pinGroup);

            // Configure the instrumentation connected to the target pins
            if (!connectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for merging.
                tsmContext.ApplyRelayConfiguration(connectedRelayConfiguration, waitSeconds: settlingTime);
            }
            // Store the current source delay settings as a backup to restore later.
            PinSiteData<double> originalSourceDelays = smuBundle.GetSourceDelayInSeconds();

            // Perform merge operation on the pin group.
            smuBundle.MergePinGroup(pinGroup);
            smuBundle.ConfigureSourceDelay(settlingTime);
            if (apertureTime != -1)
            {
                DCPowerMeasureSettings measureSettings = new DCPowerMeasureSettings()
                {
                    ApertureTime = apertureTime,
                };
                smuBundle.ConfigureMeasureSettings(measureSettings);
            }
            
            // Source and/or measure the signals.
            smuBundle.ForceVoltage(voltageLevel, currentLimit, waitForSourceCompletion: true);
            PreciseWait(timeInSeconds: settlingTime);
            smuBundle.MeasureAndPublishCurrent(publishedDataId: "Current");
            
            // Clean up and restore the state of the instrumentation after finishing the test.
            smuBundle.ForceVoltage(voltageLevel: 0, currentLimit: 0.001);
            smuBundle.PowerDown();
            PreciseWait(timeInSeconds: settlingTime);

            // Use the SMU Bundle object to perform unmerge operation on the pin group and disconnect the relays.
            smuBundle.UnmergePinGroup(pinGroup);
            if (!disconnectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for unmerging.
                tsmContext.ApplyRelayConfiguration(disconnectedRelayConfiguration, waitSeconds: settlingTime);
            }
            // Restore the source delay to original value.
            smuBundle.ConfigureSourceDelay(originalSourceDelays);
        }

    }
}
