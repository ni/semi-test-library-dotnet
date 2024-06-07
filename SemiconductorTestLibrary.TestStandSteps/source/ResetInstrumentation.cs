using System;
using System.Globalization;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary
{
    public static partial class Steps
    {
        /// <summary>
        /// Resets the instrument sessions for the specified <paramref name="instrumentTypes"/> associated with the pin map
        /// by invoking the Reset() method of the supported instrument driver.
        /// Note that the following types are supported: niDCPower, niDigitalPattern, niDMM, niRelayDriver, niScope, niFGen, Sync.
        /// Note the string value matches what's defined by the TSM <see cref="InstrumentTypeIdConstants"/>, plus ones we define like "Sync".
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
                            InstrumentAbstraction.DCPower.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Reset(tsmContext);
                            break;
                        case InstrumentTypeIdConstants.NIDmm:
                            InstrumentAbstraction.DMM.InitializeAndClose.Reset(tsmContext);
                            break;
                        case InstrumentTypeIdConstants.NIFgen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case InstrumentTypeIdConstants.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case InstrumentAbstraction.Sync.InitializeAndClose.NISyncInstrumentTypeId:
                            InstrumentAbstraction.Sync.InitializeAndClose.Reset(tsmContext);
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
