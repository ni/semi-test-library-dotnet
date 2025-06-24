﻿using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument.MyCustomInstrument
{
    /// <summary>
    /// This class contains example methods to perform driver operations using Custom Instrument support provided in STL.
    /// </summary>
    public static partial class HighLevelDriverOperations
    {
        /// <summary>
        /// Sample method to perform a driver write operation without a return value using a scalar input.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="data">The data to be written.</param>
        public static void WriteData(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, double data)
        {
            myCustomInstrumentSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                // Perform write data operation on the driver session.
                driverSession.WriteData(data);
            });
        }

        /// <summary>
        /// Sample method to perform a driver write operation without a return value using PinSiteData. Where the data value may be pin and/or site specific.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="data">The data to be written.</param>
        public static void WriteData(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, PinSiteData<double> data)
        {
            myCustomInstrumentSessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                double pinSiteSpecificData = data.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName);
                string channelString = sitePinInfo.IndividualChannelString;
                // Perform write data operation on the driver session.
                driverSession.WriteChannelData(channelString, pinSiteSpecificData);
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
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                string channelString = sitePinInfo.IndividualChannelString;
                // Perform measure data operation on the driver session.
                return driverSession.MeasureData(channelString);
            });
        }

        /// <summary>
        /// Sample method to apply device configuration.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="configurationPreset">Configuration preset.</param>
        public static void ApplyConfiguration(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, string configurationPreset)
        {
            myCustomInstrumentSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                // Call driver method to apply device configurations.
                driverSession.Configure(configurationPreset);
            });
        }

        /// <summary>
        /// Sample method to clear previously applied device configuration.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        public static void ClearConfiguration(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle)
        {
            myCustomInstrumentSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                // Call driver method to clear device configurations.
                driverSession.ResetConfiguration();
            });
        }
    }
}
