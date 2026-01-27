using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        // TODO: Testcase:17 Renaming optional parameter.
        /// <summary>
        /// Initializes NI FGEN instrument sessions associated with the pin map.
        /// If the <paramref name="resetInstrument"/> input is set to True, then the instrument will be reset as the session is initialized (default = False).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetInstrument">Whether to reset device during initialization.</param>
        public static void SetupNIFGenInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetInstrument = false)
        {
            try
            {
                InitializeAndClose.Initialize(tsmContext, resetInstrument);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
