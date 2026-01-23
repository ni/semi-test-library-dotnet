using System;
using System.ComponentModel;
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
                            InstrumentAbstraction.DCPower.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIDigitalPattern:
                            InstrumentAbstraction.Digital.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIRelayDriver:
                            InstrumentAbstraction.Relay.InitializeAndClose.Reset(tsmContext);
                            break;
                        case TestStandSteps.NIInstrumentType.NIDMM:
                            InstrumentAbstraction.DMM.InitializeAndClose.Reset(tsmContext);
                            break;
                        case TestStandSteps.NIInstrumentType.NIFGen:
                            InstrumentAbstraction.Fgen.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NIScope:
                            InstrumentAbstraction.Scope.InitializeAndClose.Reset(tsmContext, resetDevice);
                            break;
                        case TestStandSteps.NIInstrumentType.NISync:
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

        /// <summary>
        /// This method is deprecated, Use <see cref="ResetInstrumentation(ISemiconductorModuleContext, bool, TestStandSteps.NIInstrumentType)"/> instead.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to perform a hard reset on the device.</param>
        /// <param name="instrumentType">The type of instrument to reset.</param>
        [Obsolete("This method is deprecated, Use ResetInstrumentation(ISemiconductorModuleContext, bool, TestStandSteps.NIInstrumentType)", error: false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ResetInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            NIInstrumentType instrumentType = NIInstrumentType.All)
        {
            ResetInstrumentation(
            tsmContext,
            resetDevice,
            (TestStandSteps.NIInstrumentType)instrumentType);
        }

        /// <summary>
        /// Resets all the supported instrument sessions associated with the pin map. To reset specific type of instruments, use <see cref="ResetInstrumentation(ISemiconductorModuleContext, bool, TestStandSteps.NIInstrumentType)"/> instead.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void ResetInstrumentation(ISemiconductorModuleContext tsmContext)
        {
            ResetInstrumentation(tsmContext, false, TestStandSteps.NIInstrumentType.All);
        }
    }
}
