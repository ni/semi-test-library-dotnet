using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Implements <see cref="IDigitalProtocol"/> for SPI register access using
    /// NI Digital Pattern instruments with source and capture waveforms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Obtain the singleton instance via <see cref="Instance"/>, then call
    /// <see cref="DigitalProtocol.SetBundle"/> once per code module before performing
    /// any register operations. Optionally call
    /// <see cref="TestStep.ConfigureDigitalProtocol"/> once per test program to override
    /// default protocol parameters.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// IDigitalProtocol protocol = new TSMSessionManager(tsmContext).DutControl(CommunicationProtocol.SPI);
    /// protocol.WriteRegister(address: 0x48, value: 4);
    /// SiteData&lt;long&gt; readBack = protocol.ReadRegister(address: 0x48);
    /// </code>
    /// </para>
    /// </remarks>
    public sealed class SPI : DigitalProtocol
    {
        private static readonly Lazy<SPI> _instance = new Lazy<SPI>(() => new SPI());

        /// <summary>Gets the singleton SPI protocol instance.</summary>
        public static SPI Instance => _instance.Value;

        private SPI()
        {
            ReadPatternName = "SPI_read_template";
            WritePatternName = "SPI_write_template";
        }
    }
}
