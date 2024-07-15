using System;
using System.Globalization;
using System.Threading.Tasks;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Resets the instrument sessions for the specified <paramref name="instrumentType"/> associated with the pin map
        /// by invoking the Reset() method of the supported instrument driver.
        /// By default, the <paramref name="instrumentType"/> input is set to All, which targets all supported instrument types in parallel.
        /// This can be configured to target a specific instrument type, which can be useful for debugging purposes
        /// and/or if there is a need to ensure instruments are reset individually or sequentially (requiring multiple instances of this step).
        /// Note that the following types are supported: niDCPower, niDigitalPattern, niRelayDriver, niDMM, niScope, niFGen, and niSync.
        /// For instrumentation that also have the ResetDevice() method (hard reset), this can optionally be invoked instead of the Reset() method (soft-reset)
        /// if the <paramref name="resetDevice"/> input is set True (default = False): niDCPower, niDigitalPattern, niScope, and niFGen.
        /// Refer to the individual instrument driver documentation for more details.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to perform a hard reset on the device.</param>
        /// <param name="instrumentType">The type of instrument to reset.</param>
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
                            InstrumentAbstraction.DCPower.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case NIInstrumentType.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Reset(tsmContext);
                            break;
                        case NIInstrumentType.NIDMM:
                            InstrumentAbstraction.DMM.InitializeAndClose.Reset(tsmContext);
                            break;
                        case NIInstrumentType.NIFGen:
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
                });
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
