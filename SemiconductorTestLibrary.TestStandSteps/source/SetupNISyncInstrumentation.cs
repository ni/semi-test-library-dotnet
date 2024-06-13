﻿using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Sync;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes the NI Sync instrument sessions associated with the pin map.
        /// If the <paramref name="resetDevice"/> input is set True, then the instrument will be reset as the session is initialized (default = False).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        public static void SetupNISyncInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false)
        {
            try
            {
                InitializeAndClose.Initialize(tsmContext, resetDevice);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}