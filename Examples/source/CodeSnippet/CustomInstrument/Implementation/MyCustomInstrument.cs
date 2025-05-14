using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.CustomInstrument.Examples
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface.
    /// </summary>
    /// <remarks>
    /// For each Custom Instrument Type, user has to create separate concrete implementations.
    /// </remarks>
    public class MyCustomInstrument : ICustomInstrument
    {
        /// <summary>
        /// Resource Name - "InstrumentName"
        /// </summary>
        /// <remarks>
        /// InstrumentName should be same as the one used in Pinmap file.
        /// </remarks>
        public string InstrumentName { get; }

        /// <summary>
        /// Channel information - "ChannelInfo"
        /// </summary>
        /// <remarks>
        /// Optionally, store channel information for later use.
        /// </remarks>
        public string ChannelInfo { get; set; }

        /// <summary>
        /// Create new driver session and store it in DriverSessionObject
        /// </summary>
        /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        public MyCustomInstrument(string instrumentName, string channelGroupId)
        {
            // Initialize your driver based on the instrument name and channel data
            // If driver reference is of string type or Integer type, call Initialize driver API and store it in data field.
            // If reference is of class type and if Initialization happens as part of constructor then create new object and store it in data field.
            // Driver session might also depend on the channelgroupID depending on the instrument type.

            ChannelInfo = channelGroupId;
            InstrumentName = instrumentName;

            // DriverSessionObject = new MyCustomInstrumentDriver( InstrumentName + "/" + ChannelInfo);

            // Initialize data members and other class data.
        }

        /// <summary>
        /// Declare data field for storing actual instrument reference (driver session).
        /// </summary>
        /// <remarks>
        /// If the instrument reference is of String type or Integer type, just define them with default data.
        /// If the instrument reference is a class object, declare the data field of the same class object type. Constructor of the class should take care of creating reference.
        /// </remarks>
        // <Driver datatype> DriverSessionObject {get; set}

        // Declare additional data members if required.

        /// <summary>
        /// Closes the custom instrument reference.
        /// </summary>
        public void Close()
        {
            // Close the custom instrument. Add logic to close reference of the actual instrument driver.
            // If refence is of String Type or Integer type then reset the data field to default values ( <empty string> and '0' respectively.)
            // If reference is a class object then call close method or equivalent on the class object.

            // DriverSessionObject.close();
            // DriverSessionObject = null;
        }

        /// <summary>
        /// Resets the custom instrument.
        /// </summary>
        /// <remarks>
        /// Reset operation is about resetting to a known state, stopping current operation, clear faults and errors, reinitialize properties, etc. Not all instruments have reset API.
        /// </remarks>
        public void Reset()
        {
            // Add code here to Reset the instrument. If instrument does not support reset operation, you can make it NOP.
        }
    }
}
