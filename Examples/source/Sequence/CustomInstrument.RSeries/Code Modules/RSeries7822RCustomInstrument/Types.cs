namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Defines information related to each individual channel of an R Series device.
    /// </summary>
    internal struct ChannelInfo
    {
        public int ConnectorNumber;
        public int ChannelNumber;
        public int PortNumber;
        public int PortIndex;
        public PortMode Mode;

        public ChannelInfo(int connectorNumber, int portNumber, int channelNumber, int portIndex, PortMode mode)
        {
            ConnectorNumber = connectorNumber;
            PortNumber = portNumber;
            ChannelNumber = channelNumber;
            PortIndex = portIndex;
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