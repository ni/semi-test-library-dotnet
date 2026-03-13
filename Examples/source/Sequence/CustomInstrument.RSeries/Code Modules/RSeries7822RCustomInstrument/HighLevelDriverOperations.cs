using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// This class contains methods to perform driver operations using Custom Instrument support provided in STL.
    /// </summary>
    public static class HighLevelDriverOperations
    {
        /// <summary>
        /// Method to perform a driver write operation using PinSiteData, where the data value may be pin and/or site specific.
        /// </summary>
        /// <param name="rSeriesSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="data">The data to be written.</param>
        public static void WriteData(this CustomInstrumentSessionsBundle rSeriesSessionsBundle, PinSiteData<bool> data)
        {
            rSeriesSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as RSeries7822R;

                // Set the output state for each channel.
                foreach (var sitePinInfo in sessionInfo.AssociatedSitePinList)
                {
                    session.SetOutputState(sitePinInfo.IndividualChannelString, data.GetValue(sitePinInfo));
                }

                // Perform write data operation on the driver session.
                session.WritePortData();
            });
        }

        /// <summary>
        /// Method to perform a driver read operation with a pin and/or site specific return value.
        /// </summary>
        /// <param name="rSeriesSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <returns>Measured values as PinSiteData.</returns>
        public static PinSiteData<bool> ReadData(this CustomInstrumentSessionsBundle rSeriesSessionsBundle)
        {
            return rSeriesSessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo =>
            {
                var session = sessionInfo.Session as RSeries7822R;

                // Perform measure data operation on the driver session.
                var portData = session.ReadPortData();

                var channelCount = sessionInfo.AssociatedSitePinList.Count;
                var results = new bool[channelCount];
                for (int i = 0; i < channelCount; i++)
                {
                    var channelString = sessionInfo.AssociatedSitePinList[i].IndividualChannelString;
                    var channelInfo = session.ChannelInfoMap[channelString];
                    results[i] = GetBitFromByte(
                        portData[(channelInfo.ConnectorNumber, channelInfo.PortNumber)],
                        channelInfo.PortIndex);
                }
                return results;
            });
        }

        /// <summary>
        /// Enables loop back mode.
        /// </summary>
        /// <param name="rSeriesSessionsBundle">CustomInstrumentSessionsBundle.</param>
        public static void EnableLoopBack(this CustomInstrumentSessionsBundle rSeriesSessionsBundle)
        {
            rSeriesSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as RSeries7822R;

                // Set true for enabling LoopBack.
                session.EnableLoopBack(true);
            });
        }
    }
}
