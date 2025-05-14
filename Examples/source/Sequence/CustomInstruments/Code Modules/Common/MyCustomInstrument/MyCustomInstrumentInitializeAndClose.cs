using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.CustomInstrument.Common
{
    /// <summary>
    /// Defines <see cref="MyCustomInstrument"/> sessions initialize and close APIs.
    /// </summary>
    public static class MyCustomInstrumentInitializeAndClose
    {
        /// <summary>
        /// Initializes <see cref="MyCustomInstrument"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        public static void Initialize(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetCustomInstrumentNames(
                MyCustomInstrument.InstrumentTypeId,
                out string[] instrumentNames,
                out string[] channelGroups,
                out string[] channelLists);
            for (int i = 0; i < instrumentNames.Length; i++)
            {
                var session = new MyCustomInstrument(instrumentNames[i], channelGroups[i], channelLists[i]);
                tsmContext.SetCustomSession(MyCustomInstrument.InstrumentTypeId, instrumentNames[i], channelGroups[i], session);
            }

            MyCustomInstrumentSessionInformation.GenerateResourceDescriptorToSitePinDictionary(tsmContext);
        }

        /// <summary>
        /// Initializes <see cref="MyCustomInstrument"/> sessions in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device before closing.</param>
        public static void Close(ISemiconductorModuleContext tsmContext, bool resetDevice = false)
        {
            tsmContext.GetAllCustomSessions(MyCustomInstrument.InstrumentTypeId, out var sessions, out _, out _);
            foreach (var session in sessions)
            {
                (session as MyCustomInstrument).Close();
            }
        }
    }
}
