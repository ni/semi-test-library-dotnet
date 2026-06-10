using NationalInstruments.ModularInstruments.NIScope;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Represents the settings for configuring an oscilloscope channel.
    /// </summary>
    public class ChannelSettings
    {
        /// <summary>
        /// Gets or sets the vertical range of the channel.
        /// </summary>
        public double Range { get; set; }

        /// <summary>
        /// Gets or sets the vertical offset of the channel.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Gets or sets the coupling for the channel.
        /// </summary>
        public ScopeVerticalCoupling Coupling { get; set; }

        /// <summary>
        /// Gets or sets the probe attenuation factor.
        /// </summary>
        public double ProbeAttenuation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the channel is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSettings"/> class.
        /// </summary>
        public ChannelSettings()
        {
        }
    }
}
