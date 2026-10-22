using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl
{
    /// <summary>
    /// Implements <see cref="IDigitalProtocol"/> for SPI register access using
    /// NI Digital Pattern instruments with source and capture waveforms.
    /// </summary>
    /// <remarks>
    /// Obtain the singleton instance via <see cref="Instance"/>, then call
    /// <see cref="DigitalProtocol.SetBundle"/> once per code module before performing
    /// any register operations. Optionally call
    /// <see cref="TestStep.ConfigureDigitalProtocol"/> once per test program to override
    /// default protocol parameters.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.SPI);
    /// protocol.WriteRegister(address: 0x48, value: 4);
    /// SiteData&lt;long&gt; readBack = protocol.ReadRegister(address: 0x48);
    /// </code>
    /// </example>
    public sealed class SPI : DigitalProtocol
    {
        private static readonly Lazy<SPI> _instance = new Lazy<SPI>(() => new SPI());

        /// <summary>
        /// Gets the process-wide singleton SPI protocol instance, which holds shared configuration and the active bundle.
        /// It is not thread-safe, do not invoke in parallel.
        /// </summary>
        public static SPI Instance => _instance.Value;

        internal SPI()
        {
            WritePatternName = "SPI_write_pattern";
            ReadPatternName = "SPI_read_pattern";
        }
    }
}
