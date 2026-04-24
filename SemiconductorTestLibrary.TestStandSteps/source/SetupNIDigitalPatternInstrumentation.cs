using System;
using System.ComponentModel;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes NI Digital Pattern instrument sessions associated with the pin map.
        /// Loads all digital project files associated with the digital project configured for the test program.
        /// This includes the following files: pin map, specifications, patterns, source waveforms, capture waveforms, timing sheets, and levels sheets.
        /// No sheets are applied during this setup step unless specified by the <paramref name="levelsSheetToApply"/> and/or <paramref name="timingSheetToApply"/> inputs.
        /// Otherwise, the program does not assume to know which loaded sheet to apply. The program expects that the users program applies the appropriate sheets in a following step.
        /// If the <paramref name="resetDevice"/> input is set to True, the instrument is reset when the session is initialized (default = False).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="levelsSheetToApply">The name of the levels sheet to apply.</param>
        /// <param name="timingSheetToApply">The name of the timing sheet to apply.</param>
        /// <param name="applySourceWaveformData">Whether to apply the data in waveform files to source waveforms.</param>
        public static void SetupNIDigitalPatternInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            string levelsSheetToApply = "",
            string timingSheetToApply = "",
            bool applySourceWaveformData = false)
        {
            try
            {
                InstrumentAbstraction.Digital.InitializeAndClose.Initialize(
                    tsmContext, levelsSheetToApply, timingSheetToApply, resetDevice, applySourceWaveformData);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        /// <summary>
        /// This method is deprecated. Use <see cref="SetupNIDigitalPatternInstrumentation(ISemiconductorModuleContext, bool, string, string, bool)"/> instead.
        /// Initializes an NI Digital Pattern instrument sessions associated with the pin map.
        /// Loads all digital project files associated with the digital project configured for the test program.
        /// This includes the following files: pin map, specifications, patterns, source waveforms, capture waveforms, timing sheets, and levels sheets.
        /// No sheets are applied during this setup step unless specified by the <paramref name="levelsSheetToApply"/> and/or <paramref name="timingSheetToApply"/> inputs.
        /// Otherwise, the program does not assume to know which loaded sheet to apply. The program expects that the users program applies the appropriate sheets in a following step.
        /// If the <paramref name="resetDevice"/> input is set to True, the instrument is reset when the session is initialized (default = False).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="levelsSheetToApply">The name of the levels sheet to apply.</param>
        /// <param name="timingSheetToApply">The name of the timing sheet to apply.</param>
        [Obsolete("This overload is deprecated. Use SetupNIDigitalPatternInstrumentation(ISemiconductorModuleContext, bool, string, string, bool) instead.", error: false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetupNIDigitalPatternInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            string levelsSheetToApply = "",
            string timingSheetToApply = "")
        {
            SetupNIDigitalPatternInstrumentation(
                tsmContext,
                resetDevice,
                levelsSheetToApply,
                timingSheetToApply,
                applySourceWaveformData: false);
        }

        /// <summary>
        /// Initializes an NI Digital Pattern instrument sessions associated with the pin map.
        /// Loads in all digital project files associated with the digital project configured for the test program.
        /// This includes the following files: pin map, specifications, patterns, source waveforms, capture waveforms, timing sheets, and levels sheets.
        /// No sheets are applied during this setup step.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void SetupNIDigitalPatternInstrumentation(ISemiconductorModuleContext tsmContext)
        {
            SetupNIDigitalPatternInstrumentation(
                tsmContext,
                resetDevice: false,
                levelsSheetToApply: string.Empty,
                timingSheetToApply: string.Empty,
                applySourceWaveformData: false);
        }
    }
}
