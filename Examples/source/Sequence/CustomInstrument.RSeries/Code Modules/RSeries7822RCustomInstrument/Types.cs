namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Defines information related to each individual channel of an R Series device.
    /// </summary>
    internal struct ChannelInfo
    {
        /// <summary>
        /// The zero-based number for the connector associated with the channel.
        /// </summary>
        public int ConnectorNumber;

        /// <summary>
        /// The zero-based number that identifies the channel of the device within its associated connector.
        /// </summary>
        public int ChannelNumber;

        /// <summary>
        /// The zero-based number for the port associated with the channel.
        /// </summary>
        public int PortNumber;

        /// <summary>
        /// The zero-based index of the channel's position within the associated port.
        /// </summary>
        public int IndexInPort;

        /// <summary>
        /// The configured mode of the digital port associated with the channel, either Input or Output.
        /// </summary>
        public PortMode Mode;

        /// <summary>
        /// Structure containing the information associated with a channel.
        /// </summary>
        /// <param name="connectorNumber">The zero-based number that identifies the channel of the device within its associated connector.</param>
        /// <param name="portNumber">The zero-based number for the port associated with the channel.</param>
        /// <param name="channelNumber">The zero-based number that identifies the channel of the device within its associated connector.</param>
        /// <param name="indexInPort">The zero-based index of the channel's position within the associated port.</param>
        /// <param name="mode">The configured mode of the digital port associated with the channel, either Input or Output.</param>
        internal ChannelInfo(int connectorNumber, int portNumber, int channelNumber, int indexInPort, PortMode mode)
        {
            ConnectorNumber = connectorNumber;
            PortNumber = portNumber;
            ChannelNumber = channelNumber;
            IndexInPort = indexInPort;
            Mode = mode;
        }
    }

    /// <summary>
    /// Represents the possible configuration modes for the digital ports of an R Series device, either Input or Output.
    /// </summary>
    public enum PortMode
    {
        /// <summary>
        /// Output mode.
        /// </summary>
        Output,
        /// <summary>
        /// Input mode.
        /// </summary>
        Input
    }
}