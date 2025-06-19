using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument.MyCustomInstrument
{
    /// <summary>
    /// Factory for creating Custom Instrument object.
    /// </summary>
    public class MyCustomInstrumentFactory : ICustomInstrumentFactory
    {
        /// <summary>
        /// Unique instrument type ID.
        /// </summary>
        public const string CustomInstrumentTypeID = "MyUniqueCustomInstrumentTypeID";

        /// <summary>
        /// Use unique instrument type ID for each custom instrument.
        /// </summary>
        public string InstrumentTypeId => CustomInstrumentTypeID;

        /// <summary>
        /// Creates a new instance of <see cref="ICustomInstrument"/> object based on the instrument definitions found in the pin map matching the InstrumentTypeId property.
        /// </summary>
        /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        /// <param name="channelList">Channel List</param>
        /// <returns>A new instance of <see cref="ICustomInstrument"/> object.</returns>
        public ICustomInstrument CreateInstrument(string instrumentName, string channelGroupId, string channelList)
        {
            return new MyCustomInstrument(instrumentName, channelGroupId, channelList);
        }

        /// <summary>
        /// Validates the custom instruments based on the provided instrument names, channel group IDs, and channel lists associated with the instrument type ID.
        /// </summary>
        /// <param name="instrumentNames">Instrument names</param>
        /// <param name="channelGroupIds">Channel groupIDs</param>
        /// <param name="channelLists">Channel lists</param>
        /// <Remarks>
        /// This method is called as part of initialization of custom instruments.
        /// </Remarks>
        public void ValidateCustomInstruments(string[] instrumentNames, string[] channelGroupIds, string[] channelLists)
        {
            // Validate Custom instruments and raise an exception if validation fails.
        }
    }
}