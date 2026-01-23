using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    /// <summary>
    /// Defines entry points for semiconductor setup and cleanup steps.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Closes any open instrument sessions associated with the pin map.
        /// If the <paramref name="resetDevice"/> input is set to True, then the instrument will be reset before closing the session (default = False).
        /// The sessions will always be closed in parallel.
        /// By default, the <paramref name="instrumentType"/> input is set to All, which closes sessions for all instrument types in parallel.
        /// This can be configured to target a specific instrument type, which can be useful for debugging purposes
        /// and/or if there is a need to ensure sessions are closed sequentially (requiring multiple instances of this step).
        /// Note that the following types are supported: niDCPower, niDigitalPattern, niRelayDriver, niDAQmx, niDMM, niScope, niFGen, and niSync.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="instrumentType">The type of instrument to close.</param>
        public static void CleanupInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            TestStandSteps.NIInstrumentType instrumentType = TestStandSteps.NIInstrumentType.All)
        {
            try
            {
                var instrumentTypes = instrumentType != TestStandSteps.NIInstrumentType.All
                    ? new TestStandSteps.NIInstrumentType[] { instrumentType }
                    : new TestStandSteps.NIInstrumentType[]
                    {
                        TestStandSteps.NIInstrumentType.NIDCPower,
                        TestStandSteps.NIInstrumentType.NIDigitalPattern,
                        TestStandSteps.NIInstrumentType.NIRelayDriver,
                        TestStandSteps.NIInstrumentType.NIDAQmx,
                        TestStandSteps.NIInstrumentType.NIDMM,
                        TestStandSteps.NIInstrumentType.NIFGen,
                        TestStandSteps.NIInstrumentType.NIScope,
                        TestStandSteps.NIInstrumentType.NISync
                    };

                Parallel.ForEach(instrumentTypes, type =>
                {
                    switch (type)
                    {
                        case TestStandSteps.NIInstrumentType.NIDCPower:
                            InstrumentAbstraction.DCPower.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIDAQmx:
                            InstrumentAbstraction.DAQmx.InitializeAndClose.ClearAllDAQmxTasks(tsmContext);
                            break;
                        case TestStandSteps.NIInstrumentType.NIDMM:
                            InstrumentAbstraction.DMM.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIFGen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NISync:
                            InstrumentAbstraction.Sync.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        default:
                            throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Cleanup_InvalidInstrumentType, type));
                    }
                });
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        /// <summary>
        /// This method is deprecated, Use <see cref="CleanupInstrumentation(ISemiconductorModuleContext, bool, TestStandSteps.NIInstrumentType)"/> instead.
        /// </summary>
        /// <remarks>
        /// This method makes a callback to correct overload  method.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="instrumentType">The type of instrument to close.</param>
        [Obsolete("Use other overload 'CleanupInstrumentation(ISemiconductorModuleContext, bool, TestStandSteps.NIInstrumentType)' instead.", error: false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void CleanupInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            NIInstrumentType instrumentType = NIInstrumentType.All)
        {
            CleanupInstrumentation(tsmContext, resetDevice, (TestStandSteps.NIInstrumentType)instrumentType);
        }

        /// <summary>
        /// Closes any open instrument sessions associated with the pin map. To close instruments of specific types, use <see cref="CleanupInstrumentation(ISemiconductorModuleContext, bool, TestStandSteps.NIInstrumentType)"/> instead.
        /// </summary>
        /// <remarks>
        /// This method makes a callback to new overload  method.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void CleanupInstrumentation(ISemiconductorModuleContext tsmContext)
        {
            CleanupInstrumentation(tsmContext, false, TestStandSteps.NIInstrumentType.All);
        }

        /// <summary>
        /// This enum is deprecated. Use <see cref="TestStandSteps.NIInstrumentType"/> instead.
        /// </summary>
        [Obsolete("This enum is deprecated. Use TestStandSteps.NIInstrumentType instead.", error: false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public enum NIInstrumentType
        {
            /// <summary>
            /// All NI instruments.
            /// </summary>
            All,

            /// <summary>
            /// An NI-DCPower instrument.
            /// </summary>
            NIDCPower,

            /// <summary>
            /// An NI-Digital Pattern instrument.
            /// </summary>
            NIDigitalPattern,

            /// <summary>
            /// A relay driver module (NI-SWITCH instrument).
            /// </summary>
            NIRelayDriver,

            /// <summary>
            /// An NI-DAQmx task.
            /// </summary>
            NIDAQmx,

            /// <summary>
            /// An NI-DMM instrument.
            /// </summary>
            NIDMM,

            /// <summary>
            /// An NI-FGEN instrument.
            /// </summary>
            NIFGen,

            /// <summary>
            /// An NI-SCOPE instrument.
            /// </summary>
            NIScope,

            /// <summary>
            /// An NI-Sync instrument.
            /// </summary>
            NISync
        }
    }
}
