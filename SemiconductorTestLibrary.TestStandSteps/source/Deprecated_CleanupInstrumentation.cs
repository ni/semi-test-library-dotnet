using System;
using System.ComponentModel;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandStepsDeprecated
{
    /// <summary>
    /// [Deprecated]SetupAndCleanupSteps.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// [Deprecated]NIInstrumentType.
        /// </summary>
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

        /// <summary>
        /// [Deprecated]CleanupInstrumentation.
        /// </summary>
        /// <param name="tsmContext">a</param>
        /// <param name="resetDevice">a</param>
        /// <param name="instrumentType">a</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This overload is deprecated. Use the overload without optional parameter.", error: true)]
        public static void CleanupInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            NIInstrumentType instrumentType = NIInstrumentType.All)
        {
            TestStandSteps.SetupAndCleanupSteps.CleanupInstrumentation(
            tsmContext,
            resetDevice,
            (TestStandSteps.NIInstrumentType)instrumentType);
        }
    }
}
