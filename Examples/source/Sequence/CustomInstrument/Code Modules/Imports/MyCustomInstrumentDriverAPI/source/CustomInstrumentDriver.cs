using System;

namespace MyCompany.MyCustomInstrumentDriverAPI
{
    /// <summary>
    /// This is a fake driver class containing dummy driver methods. This class would typically be specific to an actual hardware driver for controlling an instrument.
    /// </summary>
    public class CustomInstrumentDriver
    {
        /// <summary>
        /// Initializes dummy driver session.
        /// </summary>
        /// <param name="resourceName">Resource name.</param>
        public CustomInstrumentDriver(string resourceName)
        {
            // Initialize dummy driver session.
        }

        /// <summary>
        /// Dummy driver method for close session.
        /// </summary>
        public void Close()
        {
            // Driver code.
        }

        /// <summary>
        /// Dummy driver method for reset session.
        /// </summary>
        public void Reset()
        {
            // Driver code.
        }

        /// <summary>
        /// Dummy driver method for write data operation.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        public void WriteData(double data)
        {
            // Driver code.
        }

        /// <summary>
        /// Dummy driver method for write channel data operation.
        /// </summary>
        /// <param name="channelString">Channel name.</param>
        /// <param name="pinSiteSpecificData">Data sepcific to pin/site.</param>
        public void WriteChannelData(string channelString, double pinSiteSpecificData)
        {
            // Driver code.
        }

        /// <summary>
        /// Dummy driver method for measure data operation.
        /// </summary>
        /// <param name="channelString">Channel string.</param>
        /// <returns>Measured data.</returns>
        public double MeasureData(string channelString)
        {
            // Driver code.
            Random rand = new Random();
            double data = rand.Next(0, 5);
            return data;
        }

        /// <summary>
        /// Dummy driver method for configure.
        /// </summary>
        /// <param name="configurationPreset">Configuration preset</param>
        public void Configure(string configurationPreset)
        {
            // Driver code.
        }

        /// <summary>
        /// Dummy driver method for reset configuration.
        /// </summary>
        public void ResetConfiguration()
        {
            // Driver code.
        }
    }
}
