using System;
using System.Globalization;
using NationalInstruments.Restricted;
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
        /// If the <paramref name="resetDevice"/> input is set True, then the instrument will be reset before closing the session (default = False).
        /// The sessions will always be closed in parallel.
        /// Optionally, if the <paramref name="instrumentTypes"/> array input is populated, and not left empty (default),
        /// one or more instrument types can specifically be targeted to be closed, which can be useful for debugging purposes
        /// and/or if there is a need to ensure sessions close sequentially.
        /// Note that the following types are supported: niDCPower, niDigitalPattern, niRelayDriver, niDAQmx, niDMM, niFGen, niScope, Sync.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="instrumentTypes">The types of instruments to close.</param>
        public static void CleanupInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            NIInstrumentType[] instrumentTypes = null)
        {
            try
            {
                if (instrumentTypes is null || instrumentTypes.IsEmpty())
                {
                    instrumentTypes = new NIInstrumentType[]
                    {
                        NIInstrumentType.NIDCPower,
                        NIInstrumentType.NIDigitalPattern,
                        NIInstrumentType.NIRelayDriver,
                        NIInstrumentType.NIDAQmx,
                        NIInstrumentType.NIDmm,
                        NIInstrumentType.NIFgen,
                        NIInstrumentType.NIScope,
                        NIInstrumentType.NISync
                    };
                }
                foreach (var instrumentType in instrumentTypes)
                {
                    switch (instrumentType)
                    {
                        case NIInstrumentType.NIDCPower:
                            InstrumentAbstraction.DCPower.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIDAQmx:
                            InstrumentAbstraction.DAQmx.InitializeAndClose.ClearAllDAQmxTasks(tsmContext);
                            break;
                        case NIInstrumentType.NIDmm:
                            InstrumentAbstraction.DMM.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIFgen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NISync:
                            InstrumentAbstraction.Sync.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        default:
                            throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Cleanup_InvalidInstrumentType, instrumentType));
                    }
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        /// <summary>
        /// Defines NI instrument types the NI Semiconductor Test Library supports.
        /// </summary>
        public enum NIInstrumentType
        {
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
            NIDmm,

            /// <summary>
            /// An NI-FGEN instrument.
            /// </summary>
            NIFgen,

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
