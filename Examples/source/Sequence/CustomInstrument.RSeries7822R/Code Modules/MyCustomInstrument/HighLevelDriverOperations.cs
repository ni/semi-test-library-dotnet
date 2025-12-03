using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries7822R
{
    /// <summary>
    /// This class contains sample methods to perform driver operations using Custom Instrument support provided in STL.
    /// </summary>
    public static class HighLevelDriverOperations
    {
        /// <summary>
        /// Sample method to perform a driver write operation using PinSiteData, where the data value may be pin and/or site specific.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="data">The data to be written.</param>
        public static void WriteData(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, PinSiteData<double> data)
        {
            myCustomInstrumentSessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var session = sessionInfo.Session as RSeries7822R;
                double pinSiteSpecificData = data.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName);
                string channelString = sitePinInfo.IndividualChannelString;
                // Perform write data operation on the driver session.
                session.WriteChannelData(channelString, pinSiteSpecificData);
            });
        }

        /// <summary>
        /// Sample method to perform a driver read operation with a pin and/or site specific return value.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <returns>Measured values as PinSiteData.</returns>
        public static PinSiteData<double> MeasureData(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle)
        {
            return myCustomInstrumentSessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var session = sessionInfo.Session as RSeries7822R;
                string channelString = sitePinInfo.IndividualChannelString;
                // Perform measure data operation on the driver session.
                return session.MeasureChannelData(channelString);
            });
        }

        /// <summary>
        /// Enables loop back mode.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        public static void EnableLoopBack(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle)
        {
            myCustomInstrumentSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as RSeries7822R;
                // Set true for enabling LoopBack.
                session.EnableLoopBack(true);
            });
        }
    }
}
