using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes NI Switch instrument sessions and NI DAQmx tasks associated with relay modules within the pin map.
        /// If the <paramref name="resetDevice"/> input is set to True, then the instrument will be reset as the session is initialized (default = False).
        /// If the <paramref name="initialRelayConfigurationToApply"/> input is provided, the step will apply the specified relay configuration.
        /// Note that the relay configuration must be defined within the pin map, otherwise the step will throw an exception.
        /// Supported devices: PXI-2567 and PXIe-6368.
        /// This method expects each NI DAQmx task associated with relays in the pin map to have the Task Type set to "DAQmxRelay".
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="initialRelayConfigurationToApply">The initial relay configuration to apply.</param>
        public static void SetupNIRelayModules(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            string initialRelayConfigurationToApply = "")
        {
            // Note that the SetupNIDAQmxDOTask method has its own try catch block and should be called before ApplyRelayConfiguration,
            // since the relay configuration being applied could have both 2567 and 6368 relays defined in it.
            SetupNIDAQmxDOTask(tsmContext, taskType: "DAQmxRelay");
            try
            {
                InstrumentAbstraction.Relay.InitializeAndClose.Initialize(tsmContext, resetDevice);
                if (!string.IsNullOrEmpty(initialRelayConfigurationToApply))
                {
                    tsmContext.ApplyRelayConfiguration(initialRelayConfigurationToApply);
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
