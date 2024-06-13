using System;
using System.Globalization;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Resets the instrument sessions for the specified <paramref name="instrumentTypes"/> associated with the pin map
        /// by invoking the Reset() method of the supported instrument driver.
        /// Note that the following types are supported: niDCPower, niDigitalPattern, niDMM, niRelayDriver, niScope, niFGen, Sync.
        /// For instrumentation that also provide the ResetDevice() method (hard reset), this can optionally be invoked
        /// instead of the Reset() method (soft-reset) if the <paramref name="resetDevice"/> input is set True (default = False).
        /// Refer to the individual instrument driver documentation for more details.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to perform a hard reset on the device.</param>
        /// <param name="instrumentTypes">The types of instruments to reset.</param>
        public static void ResetInstrumentation(
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
                            InstrumentAbstraction.DCPower.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Reset(tsmContext);
                            break;
                        case NIInstrumentType.NIDmm:
                            InstrumentAbstraction.DMM.InitializeAndClose.Reset(tsmContext);
                            break;
                        case NIInstrumentType.NIFgen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NISync:
                            InstrumentAbstraction.Sync.InitializeAndClose.Reset(tsmContext);
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
    }
}
