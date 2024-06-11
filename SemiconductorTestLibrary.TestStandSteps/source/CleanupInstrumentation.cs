using System;
using System.Globalization;
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
        /// Note the string value matches what's defined by the TSM <see cref="InstrumentTypeIdConstants"/>, plus ones we define like Sync.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="instrumentTypes">The types of instruments to close.</param>
        public static void CleanupInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            string[] instrumentTypes = null)
        {
            try
            {
                if (instrumentTypes is null)
                {
                    instrumentTypes = new string[]
                    {
                        InstrumentTypeIdConstants.NIDCPower,
                        InstrumentTypeIdConstants.NIDigitalPattern,
                        InstrumentTypeIdConstants.NIRelayDriver,
                        InstrumentTypeIdConstants.NIDAQmx,
                        InstrumentTypeIdConstants.NIDmm,
                        InstrumentTypeIdConstants.NIFgen,
                        InstrumentTypeIdConstants.NIScope,
                        InstrumentAbstraction.Sync.InitializeAndClose.NISyncInstrumentTypeId
                    };
                }
                foreach (var instrumentType in instrumentTypes)
                {
                    switch (instrumentType)
                    {
                        case InstrumentTypeIdConstants.NIDCPower:
                            InstrumentAbstraction.DCPower.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIDAQmx:
                            InstrumentAbstraction.DAQmx.InitializeAndClose.ClearAllDAQmxTasks(tsmContext);
                            break;
                        case InstrumentTypeIdConstants.NIDmm:
                            InstrumentAbstraction.DMM.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIFgen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        case InstrumentAbstraction.Sync.InitializeAndClose.NISyncInstrumentTypeId:
                            InstrumentAbstraction.Sync.InitializeAndClose.Close(tsmContext, resetDevice);
                            break;
                        default:
                            throw new NIMixedSignalException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Cleanup_InvalidInstrumentType, instrumentType));
                    }
                }
            }
            catch (Exception e)
            {
                NIMixedSignalException.Throw(e);
            }
        }
    }
}
