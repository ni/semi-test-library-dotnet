using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
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
        /// Merges the specified pin group.
        /// </summary>
        /// <param name="tsmContext">Teststand Semiconductor module context</param>
        /// <param name="pinGroup">Name of the pin group to be merged</param>
        /// <param name="settlingTime">Settling time used for measurements</param>
        /// <param name="connectedRelayConfiguration">Relay configuration that connects all the channels in parallel</param>
        public static void MergeSmuPinGroup(
            ISemiconductorModuleContext tsmContext,
            string pinGroup,
            double settlingTime = 0.001,
            string connectedRelayConfiguration = "")
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle smuBundle = sessionManager.DCPower(pinGroup);
            // Configure the instrumentation connected to the target pins
            if (!connectedRelayConfiguration.IsEmpty())
            {
                // Configure the relays required for merging.
                tsmContext.ApplyRelayConfiguration(connectedRelayConfiguration, waitSeconds: settlingTime);
            }

            // Use the MergePinGroup method on the sessions bundle to perform the merge operation. 
            // After which, the pins in the pin group will able to operate in unison when performing subsequent operations on the pin group.
            smuBundle.MergePinGroup(pinGroup);
        }
    }
}
