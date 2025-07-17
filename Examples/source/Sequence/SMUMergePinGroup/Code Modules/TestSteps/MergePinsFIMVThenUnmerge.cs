using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.NIDCPower.MergePinGroup
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the MergePinGroup and UnmergePinGroup 
    /// DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// These methods can be used to merge DUT pins together to force higher current and measure the voltage.
    /// These methods are only supported under the following conditions:
    /// 1. The pin map must define a pin group to contain all the pins that are to be merged together.
    /// 2. Each pin in the pin group must be mapped to an SMU channels of same module for a given site.
    /// 3. The SMU module must support the niDCPower Merged Channels feature.
    /// For example: PXIe-4147, PXIe-4162, and PXIe-4163.
    /// 4. The pins are physically connected externally on the application load board, either in a fixed configuration or via relays.
    /// The example methods of this class demonstrate how relay configurations can be applied 
    /// to ensures the SMUs channels are physically connected in parallel before the MergePinGroup operation,
    /// and subsequently disconnected after the UnmergePinGroup operation.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Merges the pins in specified pin group, allowing them to operate in unison to achieve a higher current output.
        /// Use the connectedRelayConfiguration parameter to specify the appropriate relay configuration
        /// that will physically connect the pins in the pin group together via external relays on the application load board.
        /// If the application load board is designed with the target pins permanently connected together,
        /// do not specify a value for the connectedRelayConfiguration parameter. 
        /// The settlingTime parameter is only applicable when the connectedRelayConfiguration parameter is used.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinGroup">Name of the pin group to be merged.</param>
        /// <param name="currentLevel">Current level to set output.</param>
        /// <param name="voltageLimit">Voltage limit for output.</param>
        /// <param name="settlingTime">Settling time used for measurements.</param>
        /// <param name="apertureTime">Aperture time used for measurements.</param>
        /// <param name="connectedRelayConfiguration">Relay configuration that physically connects all the channels in parallel on the application load board, if required.</param>
        /// <param name="disconnectedRelayConfiguration">Relay configuration that disconnects all the channels.</param>
        public static void MergePinsFIMVThenUnmerge(
            ISemiconductorModuleContext tsmContext,
            string pinGroup,
            double currentLevel = 5,
            double voltageLimit = 0.4,
            double settlingTime = 0.001,
            double apertureTime = -1,
            string connectedRelayConfiguration = "",
            string disconnectedRelayConfiguration = "")
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(pinGroup);

            // Configure the instrumentation connected to the target pins.
            if (!connectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for merging.
                tsmContext.ApplyRelayConfiguration(connectedRelayConfiguration, waitSeconds: settlingTime);
            }

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
            smuBundle.ForceCurrent(currentLevel, voltageLimit, waitForSourceCompletion: true);
            smuBundle.MeasureAndPublishVoltage(publishedDataId: "Voltage");
            smuBundle.MeasureAndPublishCurrent(publishedDataId: "Current");

            // Clean up and restore the state of the instrumentation after finishing the test.
            smuBundle.ForceCurrent(currentLevel: 0.001, voltageLimit: 0.01);
            smuBundle.PowerDown();
            PreciseWait(timeInSeconds: settlingTime);

            // Use the SMU Bundle object to perform unmerge operation on the pin group and disconnect the relays.
            smuBundle.UnmergePinGroup(pinGroup);
            if (!disconnectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for unmerging.
                tsmContext.ApplyRelayConfiguration(disconnectedRelayConfiguration, waitSeconds: settlingTime);
            } 
        }
    }
}
