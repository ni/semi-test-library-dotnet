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
            // Use the SMU Bundle object to perform merge operation on the pin group.
            smuBundle.MergePinGroup(pinGroup);
        }
    }
}
