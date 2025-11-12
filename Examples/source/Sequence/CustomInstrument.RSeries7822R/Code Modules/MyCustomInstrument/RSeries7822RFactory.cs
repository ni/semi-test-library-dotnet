using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.MyCustomInstrument
{
    /// <summary>
    /// Factory for creating Custom Instrument object.
    /// </summary>
    public class RSeries7822RFactory : ICustomInstrumentFactory
    {
        /// <summary>
        /// The unique instrument type ID associated with a specific custom instrument implementation.
        /// </summary>
        /// <remarks>
        /// Use this field to access the instrument type ID string from the class itself.
        /// Note that this constant field is not part of the ICustomInstrument interface, but is being implemented here for convenience
        /// to allow accessing the instrument type ID string from the class itself, rather than having to create an instance of the MyCustomInstrument to access it via the InstrumentTypeId property.
        /// Also, note that this a work around for a known limitation of the C# language, which does not allow an interface to define static properties or constant fields.
        /// </remarks>
        public const string CustomInstrumentTypeId = "PXI_RSeries_7822R";

        /// <summary>
        /// The unique instrument type ID associated with the instrument.
        /// </summary>
        /// <remarks>
        /// Use this field to access the instrument type ID string from an object instance of this class.
        /// Note that this property is required as part of the ICustomInstrument interface as it is used in other places within the underlying custom instrument abstraction.
        /// </remarks>
        public string InstrumentTypeId => CustomInstrumentTypeId;

        /// <summary>
        /// Creates a new instance of <see cref="ICustomInstrument"/> object based on the instrument definitions found in the pin map matching the InstrumentTypeId property.
        /// </summary>
        /// <param name="instrumentName">Instrument Name</param>
        /// <param name="channelGroupId">Channel Group Id</param>
        /// <param name="channelList">Channel List</param>
        /// <returns>A new instance of <see cref="ICustomInstrument"/> object.</returns>
        public ICustomInstrument CreateInstrument(string instrumentName, string channelGroupId, string channelList)
        {
            return new RSeries7822R(instrumentName, channelGroupId, channelList);
        }

        /// <summary>
        /// Validates the custom instruments based on the provided instrument names, channel group IDs, and channel lists associated with the instrument type ID.
        /// </summary>
        /// <param name="instrumentNames">Instrument names</param>
        /// <param name="channelGroupIds">Channel groupIDs</param>
        /// <param name="channelLists">Channel lists</param>
        /// <Remarks>
        /// This method is called as part of initialization of custom instruments.
        /// Each instrument should have two ChannelgroupIDs, one group should be for digital channels from dio to dio7 and another group should be for analog channels ai0 to ai3.
        /// </Remarks>
        public void ValidateCustomInstruments(string[] instrumentNames, string[] channelGroupIds, string[] channelLists)
        {
            // Reconstruct the instrumentNames and channelLists inputs into the format of IEnumerable<KeyValuePair<instrumentName, sortedChannelList>>.
            // Each KeyValuePair element corresponds to one channel group.
            var instruments = instrumentNames.Zip(channelLists, (instrumentName, channelList)
                => new KeyValuePair<string, string>(instrumentName, channelList));

            // Group the instruments by name.
            var instrumentsByName = instruments.GroupBy(instrument => instrument.Key);

            // Check if all instruments have single channel group.
            if (instrumentsByName.Any(instrument => instrument.Count() != 1))
            {
                throw new InvalidCustomInstrumentPinMapDefinitionException("At least one instrument definition does not satify the single channel group constraint");
            }
        }
    }
}