using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannel.Common.CustomInstrumentB
{
    /// <summary>
    /// Defines <see cref="CustomInstrumentB"/> sessions initialize and close APIs.
    /// </summary>
    public static class CustomInstrumentBInitializeAndClose
    {
        /// <summary>
        /// Initializes <see cref="CustomInstrumentB"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        public static void Initialize(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomInstrumentBNames(
                out string[] instrumentNames,
                out string[] channelGroups,
                out string[] channelLists);
            for (int i = 0; i < instrumentNames.Length; i++)
            {
                var session = new CustomInstrumentB(instrumentNames[i], channelLists[i]);
                tsmContext.SetCustomInstrumentBSessions(instrumentNames[i], channelGroups[i], session);
            }

            CustomInstrumentBSessionInformation.GenerateResourceDescriptorToSitePinDictionary(tsmContext);
        }

        /// <summary>
        /// Initializes <see cref="CustomInstrumentB"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device before closing.</param>
        public static void Close(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomInstrumentBSessions(out var sessions);
            foreach (var session in sessions)
            {
                session.Close();
            }
        }
    }
}
