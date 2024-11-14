using System;
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
            NIInstrumentType instrumentType = NIInstrumentType.All)
        {
            try
            {
                var instrumentTypes = instrumentType != NIInstrumentType.All
                    ? new NIInstrumentType[] { instrumentType }
                    : new NIInstrumentType[]
                    {
                        NIInstrumentType.NIDCPower,
                        NIInstrumentType.NIDigitalPattern,
                        NIInstrumentType.NIRelayDriver,
                        NIInstrumentType.NIDAQmx,
                        NIInstrumentType.NIDMM,
                        NIInstrumentType.NIFGen,
                        NIInstrumentType.NIScope,
                        NIInstrumentType.NISync
                    };

                Parallel.ForEach(instrumentTypes, type =>
                {
                    switch (type)
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
                        case NIInstrumentType.NIDMM:
                            InstrumentAbstraction.DMM.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIFGen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NISync:
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
    }
}
