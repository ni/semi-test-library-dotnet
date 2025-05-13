using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument
{
    /// <summary>
    /// Defines the initialization and closing of a custom instrument.
    /// </summary>
    public static class InitializeAndClose
    {
        /// <summary>
        /// Initializes the custom instrument.
        /// </summary>
        /// <param name="tsmContext">TSM Context</param>
        /// <param name="customInstrumentFactory">Custom Instrument Factory</param>
        public static void Initialize(ISemiconductorModuleContext tsmContext, ICustomInstrumentFactory customInstrumentFactory)
        {
            tsmContext.GetCustomInstrumentNames(customInstrumentFactory.InstrumentTypeId, out string[] instrumentNames, out string[] channelGroupIds, out string[] channelLists);

            for (int i = 0; i < channelGroupIds.Length; i++)
            {
                string instrumentName = instrumentNames[i];
                string channelGroupId = channelGroupIds[i];
                ICustomInstrument customInstrument = customInstrumentFactory.CreateInstrument();

                // Assuming we have to initialize one custom instrument session for each channel group. (In case of independent channel (session per channel), the channel group will be the channel itself)
                customInstrument.Initialize(instrumentName, channelGroupId);

                tsmContext.SetCustomSession(customInstrumentFactory.InstrumentTypeId, instrumentName, channelGroupId, customInstrument);
            }

            CustomInstrumentSessionInformation.GenerateInstrumentChannelToSitePinDictionary(tsmContext, customInstrumentFactory.InstrumentTypeId);
        }
    }
}