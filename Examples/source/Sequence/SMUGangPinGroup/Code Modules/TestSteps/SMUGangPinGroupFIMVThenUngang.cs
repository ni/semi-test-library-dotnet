using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SMUGangPinGroup
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the GangPinGroup and
    /// DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// These methods can be used to gang DUT pins together to output higher current.
    /// These methods are only supported under the following conditions:
    /// 1. The pin map must define a pin group to contain all the pins that are to be ganged together.
    /// 2. The SMU module must support the source trigger and measure trigger feature.
    /// For example: PXIe-4137, PXIe-4139, PXIe-4147, PXIe-4150, PXIe-4162, and PXIe-4163.
    /// 3. The pins are physically connected externally on the application load board, either in a fixed configuration or via relays.
    /// The example methods of this class demonstrate how relay configurations can be applied
    /// to ensures the SMUs channels are physically connected in parallel before the GangPinGroup operation,
    /// and subsequently disconnected after the UngangPinGroup operation.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Gangs the pins in specified pin group, allowing them to operate in unison to achieve a higher current output.
        /// Then, high current is forced on the ganged pin group and a voltage measurement is published.
        /// Finally, the ganged pin group is powered down and un-ganged.
        /// Use the relayConfigurationToConnect and relayConfigurationToDisconnect parameters to specify the appropriate relay configurations
        /// that will physically connect/disconnect the pins in the pin group via external relays on the application load board.
        /// If the application load board is designed with the target pins permanently connected together,
        /// do not specify a value for the relayConfigurationToConnect/relayConfigurationToDisconnect parameters.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinGroup">Name of the pin group to be ganged.</param>
        /// <param name="currentLevel">Current level to set output.</param>
        /// <param name="voltageLimit">Voltage limit for output.</param>
        /// <param name="settlingTime">Settling time used for measurements and for the relay configuration to be connected (if applicable).</param>
        /// <param name="apertureTime">Aperture time used for measurements.</param>
        /// <param name="relayConfigurationToConnect">Relay configuration that physically connects all the channels in parallel on the application load board, if required.</param>
        /// <param name="relayConfigurationToDisconnect">Relay configuration that physically disconnects the channels on the application load board, if required.</param>
        public static void SMUGangPinsFIMVThenUngang(
            ISemiconductorModuleContext tsmContext,
            string pinGroup,
            double currentLevel = 0.1,
            double voltageLimit = 0.24,
            double settlingTime = 0.001,
            double apertureTime = -1,
            string relayConfigurationToConnect = "",
            string relayConfigurationToDisconnect = "")
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(pinGroup);

            // Configure the appropriate relays required to physically connect the pins externally.
            if (!string.IsNullOrEmpty(relayConfigurationToConnect))
            {
                tsmContext.ApplyRelayConfiguration(relayConfigurationToConnect, waitSeconds: settlingTime);
            }

            // Perform gang operation on the pin group.
            smuBundle.GangPinGroup(pinGroup);
            smuBundle.ConfigureSourceDelay(settlingTime);
            if (apertureTime != -1)
            {
                smuBundle.ConfigureMeasureSettings(new DCPowerMeasureSettings { ApertureTime = apertureTime });
            }

            // Source and/or measure the signals.
            smuBundle.ForceCurrent(currentLevel, voltageLimit, waitForSourceCompletion: true);
            smuBundle.MeasureAndPublishVoltage(publishedDataId: "Voltage");

            // Clean up and restore the state of the instrumentation after finishing the test.
            smuBundle.ForceCurrent(currentLevel: 0.001, voltageLimit: 0.240);
            smuBundle.PowerDown();
            PreciseWait(timeInSeconds: settlingTime);

            // Configure the appropriate relays required to physically disconnect the pins externally.
            smuBundle.UngangPinGroup(pinGroup);
            if (!string.IsNullOrEmpty(relayConfigurationToDisconnect))
            {
                tsmContext.ApplyRelayConfiguration(relayConfigurationToDisconnect, waitSeconds: settlingTime);
            }
        }
    }
}
