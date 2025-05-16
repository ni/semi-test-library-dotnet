using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerInstrument.Common.CustomInstrumentA
{
    /// <summary>
    /// Defines <see cref="CustomInstrumentA"/> sessions initialize and close APIs.
    /// </summary>
    public static class CustomInstrumentAInitializeAndClose
    {
        /// <summary>
        /// Initializes <see cref="CustomInstrumentA"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        public static void Initialize(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomInstrumentANames(
                out string[] instrumentNames,
                out string[] channelLists);
            for (int i = 0; i < instrumentNames.Length; i++)
            {
                var session = new CustomInstrumentA(instrumentNames[i], channelLists[i]);
                tsmContext.SetCustomInstrumentASessions(instrumentNames[i], session);
            }

            CustomInstrumentASessionInformation.GenerateResourceDescriptorToSitePinDictionary(tsmContext);
        }

        /// <summary>
        /// Initializes <see cref="CustomInstrumentA"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device before closing.</param>
        public static void Close(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomInstrumentASessions(out var sessions);
            foreach (var session in sessions)
            {
                session.Close();
            }
        }
    }
}
