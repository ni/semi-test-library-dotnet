using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes an NI Digital Pattern instrument sessions associated with the pin map.
        /// It loads in all digital project files associated with the digital project configured for the test program,
        /// this includes the: pin map, specifications, patterns, source waveforms, capture waveforms, timing sheets, and levels sheets.
        /// No sheets will be applied during this setup step unless specified by the <paramref name="levelsSheetToApply"/> and/or <paramref name="timingSheetToApply"/> inputs.
        /// Otherwise, the program will not assume to know loaded sheet to apply and will expect that the users program will apply the appropriate sheet(s) within a proceeding step.
        /// If the <paramref name="resetDevice"/> input is set to True, then the instrument will be reset as the session is initialized (default = False).
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
    }
}
