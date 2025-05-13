using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.SemiconductorTestLibrary.CustomInstrument.Examples
{
    /// <summary>
    /// Factory for creating Custom Instrument object
    /// </summary>
    public class MyCustomInstrumentFactory : ICustomInstrumentFactory
    {
        /// <summary>
        /// Use unique Instrument type ID for each custom instrument.
        /// </summary>
        public string InstrumentTypeId => "MyUniqueCustomInstrumentTypeID";

        /// <summary>
        /// Deafult Constructor
        /// </summary>
        public MyCustomInstrumentFactory()
        {
            /// Intialize object
        }

        /// <summary>
        /// Creates a new instance of <see cref="ICustomInstrument"/> based on the instrument type ID.
        /// </summary>
        /// <returns>A new instance of <see cref="ICustomInstrument"/>.</returns>
        /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        public ICustomInstrument CreateInstrument(string instrumentName, string channelGroupId)
        {
            return new MyCustomInstrument(instrumentName, channelGroupId);
        }
    }
}