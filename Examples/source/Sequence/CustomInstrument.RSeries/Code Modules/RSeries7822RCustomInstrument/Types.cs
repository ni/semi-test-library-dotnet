namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Defines custom types required by the example.
    /// </summary>
    public static class Types
    {
        internal struct ChannelInfo
        {
            public int ConnectorNumber;
            public int ChannelNumber;
            public int PortNumber;
            public int PortIndex;

            public ChannelInfo(int connectorNumber, int portNumber, int channelNumber, int portIndex)
            {
                ConnectorNumber = connectorNumber;
                PortNumber = portNumber;
                ChannelNumber = channelNumber;
                PortIndex = portIndex;
            }
        }

        /// <summary>
        /// Represents the possible configuration modes for the digital ports of an R Series device, either Input or Output.
        /// </summary>
        public enum PortConfiguration
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
}
