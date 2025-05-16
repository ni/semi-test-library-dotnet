using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrumentation.SessionPerChannelGroup.Common.CustomInstrumentC
{
    /// <summary>
    /// Defines <see cref="CustomInstrumentC"/> sessions initialize and close APIs.
    /// </summary>
    public static class CustomInstrumentCInitializeAndClose
    {
        /// <summary>
        /// Initializes <see cref="CustomInstrumentC"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        public static void Initialize(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomInstrumentCNames(
                out string[] instrumentNames,
                out string[] channelGroups,
                out string[] channelLists);
            for (int i = 0; i < channelGroups.Length; i++)
            {
                var session = new CustomInstrumentC(instrumentNames[i], channelGroups[i], channelLists[i]);
                tsmContext.SetCustomInstrumentCSessions(instrumentNames[i], channelGroups[i], session);
            }

            CustomInstrumentCSessionInformation.GenerateResourceDescriptorToSitePinDictionary(tsmContext);
        }

        /// <summary>
        /// Initializes <see cref="CustomInstrumentB"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device before closing.</param>
        public static void Close(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomInstrumentCSessions(out var sessions);
            foreach (var session in sessions)
            {
                session.Close();
            }
        }
    }
}
