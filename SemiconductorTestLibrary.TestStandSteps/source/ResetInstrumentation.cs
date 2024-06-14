using System;
using System.Globalization;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Resets the instrument sessions for the specified <paramref name="instrumentType"/> associated with the pin map
        /// by invoking the Reset() method of the supported instrument driver.
        /// Note that the following types are supported: niDCPower, niDigitalPattern, niDMM, niRelayDriver, niScope, niFGen, Sync.
        /// For instrumentation that also provide the ResetDevice() method (hard reset), this can optionally be invoked
        /// instead of the Reset() method (soft-reset) if the <paramref name="resetDevice"/> input is set True (default = False).
        /// Refer to the individual instrument driver documentation for more details.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to perform a hard reset on the device.</param>
        /// <param name="instrumentType">The types of instrument(s) to reset.</param>
        public static void ResetInstrumentation(
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
                        NIInstrumentType.NIDmm,
                        NIInstrumentType.NIFgen,
                        NIInstrumentType.NIScope,
                        NIInstrumentType.NISync
                    };

                foreach (var type in instrumentTypes)
                {
                    switch (type)
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
                            throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.Cleanup_InvalidInstrumentType, type));
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
