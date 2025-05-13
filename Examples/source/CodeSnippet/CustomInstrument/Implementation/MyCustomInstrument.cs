using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.CustomInstrument.Examples
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface.
    /// </summary>
    public class MyCustomInstrument : ICustomInstrument
    {
        /// <summary>
        /// Resource Name - "instrumentName"
        /// </summary>
        public string InstrumentName { get; private set; }

        /// <summary>
        /// Channel information - "channelInfo"
        /// </summary>
        /// <remarks>
        /// Optionally, store channel information if driver session is specific to Channel or Channel group.
        /// </remarks>
        public string ChannelInfo { get; set; }

        /// <summary>
        /// Declare data field for storing actual instrument reference ( driver session).
        /// </summary>
        /// <remarks>
        /// If reference is of String type or Integer type, just define them with default data. Constructor code should initialize it.
        /// If reference is a class object, declare the data field of the same class type.
        /// </remarks>
        // <Driver datatype> DriverSessionObject {get; set}

        // Declare additional data members if required.

        /// <summary>
        /// Closes the custom instrument.
        /// </summary>
        public void Close()
        {
            // Close the custom instrument. Add logic to close reference of the actual instrument driver reference.
            // If refence is of String Type or Integer type, reset the data field to default values ( <empty string> and '0')
            // If reference is a class object, call close method on it or destructor and assign null to the class object.

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

        /// <summary>
        /// Create new driver session and store it in DriverSessionObject
        /// </summary>
        /// /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        public MyCustomInstrument(string instrumentName, string channelGroupId)
        {
            // Initialize your driver based on the instrumentname and channel data
            // If driver reference is of string type or numeric type, call Initialize driver API and store it in data field.
            // If reference is of class type and if Initialization happens as part of constructor then create new object and store it in data field.
            // Driver session might also depend on the channel depending on the instrument type.

            ChannelInfo = channelGroupId;
            InstrumentName = instrumentName;

            // DriverSessionObject = new MyCustomInstrumentDriver( InstrumentName + "/" + ChannelInfo);

            // Initialize data members and other class data.
        }
    }
}
