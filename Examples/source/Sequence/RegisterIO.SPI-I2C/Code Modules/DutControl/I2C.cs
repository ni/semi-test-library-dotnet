using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl
{
    /// <summary>
    /// Implements <see cref="IDigitalProtocol"/> for I2C register access using
    /// NI Digital Pattern instruments with source and capture waveforms.
    /// </summary>
    /// <remarks>
    /// The I2C data line (SDA) is connected to the SDI pin, which is used for both the source and the capture waveforms,
    /// and the I2C clock line (SCL) is connected to the SCK pin. The CS and SDO pins are held high by the I2C patterns,
    /// which keeps the DUT in I2C mode and selects its device address.
    /// The waveforms use their own names so they do not conflict with the SPI waveforms loaded on the same session.
    /// Obtain the singleton instance via <see cref="Instance"/>, then call
    /// <see cref="DigitalProtocol.SetBundle"/> once per code module before performing
    /// any register operations. Optionally call
    /// <see cref="TestStep.ConfigureDigitalProtocol"/> once per test program to override
    /// default protocol parameters.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// IDigitalProtocol protocol = tsmContext.DutControl(CommunicationProtocol.I2C);
    /// protocol.WriteRegister(address: 0x48, value: 4);
    /// SiteData&lt;long&gt; readBack = protocol.ReadRegister(address: 0x48);
    /// </code>
    /// </example>
    public sealed class I2C : DigitalProtocol
    {
        private static readonly Lazy<I2C> _instance = new Lazy<I2C>(() => new I2C());

        /// <summary>
        /// Gets the process-wide singleton I2C protocol instance, which holds shared configuration and the active bundle.
        /// It is not thread-safe, do not invoke in parallel.
        /// </summary>
        public static I2C Instance => _instance.Value;

        internal I2C()
        {
            WritePatternName = "I2C_write_template";
            ReadPatternName = "I2C_read_template";
            SourceWaveformName = "i2c_source_buffer";
            CaptureWaveformName = "i2c_capture_buffer";
            SourcePinName = "SDI";
            CapturePinName = "SDI";
            PinNames = new[] { "CS", "SDI", "SDO", "SCK" };
        }
    }
}
