using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.NIDCPower.MergePinGroup
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the MergePinGroup and UnmergePinGroup 
    /// DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// These methods can be used to merge DUT pins together to output higher current.
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
        /// Power down the bundle and then unmerges the pins in specified pin group, allowing them to operate independently afterwards.
        /// Use the disconnectedRelayConfiguration parameter to specify the appropriate relay configuration
        /// that will physically disconnect the pins in the pin group via external relays on the application load board.
        /// If the application load board is designed with the target pins permanently connected together,
        /// do not specify a value for the disconnectedRelayConfiguration parameter. 
        /// The settlingTime parameter is only applicable when the disconnectedRelayConfiguration parameter is used.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinGroup">Name of the pin group to be merged.</param>
        /// <param name="disconnectedRelayConfiguration">Relay configuration that disconnects all the channels.</param>
        /// <param name="settlingTime">Settling time used for measurements.</param>
        public static void PowerDownAndUnmergeSmuPinGroup(
            ISemiconductorModuleContext tsmContext,
            string pinGroup,
            string disconnectedRelayConfiguration = "",
            double settlingTime = 0.001)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(pinGroup);
            // Power down the pins before disconnecting
            smuBundle.ForceCurrent(currentLevel: 0, voltageLimit: 0.01);
            smuBundle.PowerDown();
            //Configure the appropriate relays required to physically disconnect the pins externally.
            smuBundle.UnmergePinGroup(pinGroup);
            if (!String.IsNullOrEmpty(disconnectedRelayConfiguration))
            {
                tsmContext.ApplyRelayConfiguration(disconnectedRelayConfiguration, waitSeconds: settlingTime);
            }
        }
    }
}
