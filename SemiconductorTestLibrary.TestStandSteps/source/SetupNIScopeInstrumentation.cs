using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        // TODO: Testcase:19 Changing access scope to private.
        /// <summary>
        /// Initializes NI Scope instrument sessions associated with the pin map.
        /// If the <paramref name="resetDevice"/> input is set to True, then the instrument
        /// will be reset as the session is initialized (default = False).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        private static void SetupNIScopeInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false)
        {
            try
            {
                InstrumentAbstraction.Scope.InitializeAndClose.Initialize(tsmContext, resetDevice);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
