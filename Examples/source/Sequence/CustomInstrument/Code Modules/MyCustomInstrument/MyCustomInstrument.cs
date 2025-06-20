using MyCompany.MyCustomInstrumentDriverAPI;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument.MyCustomInstrument
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface.
    /// </summary>
    /// <remarks>
    /// Users must create separate concrete implementations for different Custom Instrument types.
    /// </remarks>
    public class MyCustomInstrument : ICustomInstrument
    {
        /// <summary>
        /// The custom instrument driver object.
        /// </summary>
        /// <remarks>
        /// If the Driver Session is a class object, then create property with datatype same as class object type.
        /// If the Driver Session is not a class object, for example, String type or Integer type, then define them as such with a* default data value or make the type nullable.
        /// </remarks>
        public CustomInstrumentDriver InstrumentDriverSession { get; private set; }

        /// <summary>
        /// Instrument name that matches the one defined in the Pinmap file.
        /// </summary>
        public string InstrumentName { get; }

        /// <summary>
        /// Channel information.
        /// </summary>
        /// <remarks>
        /// Optionally, store channel information for later use.
        /// </remarks>
        public string ChannelGroupId { get; }

        /// <summary>
        /// Channel list.
        /// </summary>
        /// <remarks>
        /// Store channel list for later use. This information is needed to perform driver operation on specific channel.
        /// </remarks>
        public string ChannelList { get; }

        /// <summary>
        /// Resource name which includes instrument name and channel information.
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Creates new driver session and stores it in InstrumentDriverSession.
        /// </summary>
        /// <param name="instrumentName">Instrument Name as defined in the Pin Map</param>
        /// <param name="channelGroupId">Channel Group Id as defined in the Pin Map</param>
        /// <param name="channelList">Channel List as defined in the Pin Map</param>
        public MyCustomInstrument(string instrumentName, string channelGroupId, string channelList)
        {
            // Initialize your driver based on the instrument name and channel data and update 'InstrumentDriverSession'.

            ChannelGroupId = channelGroupId;
            InstrumentName = instrumentName;
            ChannelList = channelList;
            ResourceName = $"{InstrumentName}/{ChannelGroupId}"; // Append 'InstrumentName' with channel info depending on your instrument type.

            InstrumentDriverSession = new CustomInstrumentDriver(ResourceName);

            // Initialize other data members.
        }

        /// <summary>
        /// Closes the custom instrument session.
        /// </summary>
        public void Close()
        {
            InstrumentDriverSession.Close();
            InstrumentDriverSession = default; // Explicitly set InstrumentDriverSession to default value if required.
        }

        /// <summary>
        /// Resets the custom instrument.
        /// </summary>
        /// <remarks>
        /// Reset operation is about resetting to a known state, stopping current operation, clear faults and errors, reinitialize properties, etc. Not all instruments have reset API.
        /// </remarks>
        public void Reset()
        {
            // If instrument does not support reset operation, you can make it NOP.
            InstrumentDriverSession.Reset();
        }
    }
}
