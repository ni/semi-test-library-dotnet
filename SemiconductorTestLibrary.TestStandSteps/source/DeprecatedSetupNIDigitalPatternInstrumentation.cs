using System;
using System.ComponentModel;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandStepsDeprecated
{
    /// <summary>
    /// [Deprecated] SetupAndCleanupSteps class.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// [Deprecated] This method is deprecated and use the other overload instead.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="levelsSheetToApply">The name of the levels sheet to apply.</param>
        /// <param name="timingSheetToApply">The name of the timing sheet to apply.</param>
        [Obsolete("Use TestStandSteps.NIInstrumentType instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetupNIDigitalPatternInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            string levelsSheetToApply = "",
            string timingSheetToApply = "")
        {
            TestStandSteps.SetupAndCleanupSteps.SetupNIDigitalPatternInstrumentation(
            tsmContext,
            resetDevice,
            levelsSheetToApply,
            timingSheetToApply,
            false);
        }
    }
}
