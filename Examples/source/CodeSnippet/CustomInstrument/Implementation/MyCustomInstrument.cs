using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Store information if driver session is channel specific
        /// </remarks>
        public string ChannelInfo { get; set; }

        /// <summary>
        /// Declare data field for storing actual instrument reference.
        /// </summary>
        // <Driver datatype> DriverSessionObject {get; set}

        // Declare additional data members if required.

        /// <summary>
        /// Closes the custom instrument.
        /// </summary>
        public void Close()
        {
            // Close the custom instrument. Add logic to close reference of the actual instrument driver reference.
            // DriverSessionObject.close();
        }

        /// <summary>
        /// Resets the custom instrument.
        /// </summary>
        public void Reset()
        {
            // Reset the custom instrument.
        }

        /// <summary>
        /// Resets the custom instrument.
        /// </summary>
        /// /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        public MyCustomInstrument(string instrumentName, string channelGroupId)
        {
            // Create new driver session and store it in DriverSessionObject
            // DriverSessionObject = new MyCustomInstrumentDriver();
            // Initialize datamembers and other class data.
        }
    }
}
