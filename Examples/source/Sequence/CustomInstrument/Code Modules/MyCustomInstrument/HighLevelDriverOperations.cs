using MyCompany.MyCustomInstrumentDriverAPI;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.CustomerInstrument.MyCustomInstrument
{
    /// <summary>
    /// This class contains example methods to perform driver operations using Custom Instrument support provided in STL.
    /// </summary>
    public static partial class HighLevelDriverOperations
    {
        /// <summary>
        /// Sample method to perform driver operation without a return value.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="parameter1">1st parameter.</param>
        /// <param name="parameter2">2nd parameter.</param>
        public static void DriverOperationWithScalarInputs(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, double parameter1, double parameter2)
        {
            myCustomInstrumentSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                // Perform write data operation on the driver session.
                driverSession.WriteData(parameter1, parameter2);
            });
        }

        /// <summary>
        /// Sample method to perform pin/site specific driver write operation using PinSiteData.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="parameter">1st parameter.</param>
        public static void DriverOperationWithPinSiteDataInput(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, PinSiteData<double> parameter)
        {
            myCustomInstrumentSessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                double pinSiteSpecificData = parameter.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName);
                string channelString = sitePinInfo.IndividualChannelString;

                // Perform write data operation on the driver session.
                driverSession.WriteChannelData(channelString, pinSiteSpecificData);
            });
        }

        /// <summary>
        /// Sample method to perform driver operation with a return value.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <returns>Measured values as PinSiteData.</returns>
        public static PinSiteData<double> DriverOperationThatReturnsPinSiteData(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle)
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
        /// Sample method to apply device configurations.
        /// </summary>
        /// <param name="myCustomInstrumentSessionsBundle">CustomInstrumentSessionsBundle.</param>
        /// <param name="configurationData">Configuration Data.</param>
        public static void ApplyConfigurations(this CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle, double configurationData)
        {
            myCustomInstrumentSessionsBundle.Do(sessionInfo =>
            {
                var session = sessionInfo.Session as MyCustomInstrument;
                var driverSession = session.InstrumentDriverSession;
                // Call driver method to apply device configurations.
                driverSession.Configure(configurationData); 
            });
        }

        /// <summary>
        /// Sample method to clear configurations.
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
