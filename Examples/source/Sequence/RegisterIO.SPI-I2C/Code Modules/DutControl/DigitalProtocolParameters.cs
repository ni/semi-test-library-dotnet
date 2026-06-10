namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Parameter model for protocol-specific register I/O. Set only the properties you want
    /// to override; protocol-specific defaults are applied by the derived parameter classes.
    /// </summary>
    public abstract class DigitalProtocolParameters
    {
        /// <summary>Gets or sets the number of bits the register address is.</summary>
        public uint AddressBitWidth { get; set; }

        /// <summary>Gets or sets the number of bits the register value holds.</summary>
        public uint ValueBitWidth { get; set; }

        /// <summary>Gets or sets the number of bits each digital waveform sample is.</summary>
        public uint SampleWidth { get; set; } = 8;

        /// <summary>Gets or sets the protocol pin names expected by the template patterns.</summary>
        public string[] PinNames { get; set; }

        /// <summary>Gets or sets the digital pattern name used to read a register value.</summary>
        public string ReadPatternName { get; set; }

        /// <summary>Gets or sets the digital pattern name used to write a register value.</summary>
        public string WritePatternName { get; set; }

        /// <summary>Gets or sets the digital capture waveform name used by the read pattern.</summary>
        public string CaptureWaveformName { get; set; } = "capture_buffer";

        /// <summary>Gets or sets the digital source waveform name used by the read and write patterns.</summary>
        public string SourceWaveformName { get; set; } = "source_buffer";

        /// <summary>Gets or sets the sequencer register that controls the transaction count.</summary>
        public string ReadWriteCountSequenceRegister { get; set; } = "reg0";

        /// <summary>Gets or sets the sequencer register that controls the address bit width.</summary>
        public string AddressBitWidthSequenceRegister { get; set; } = "reg1";

        /// <summary>Gets or sets the sequencer register that controls the value bit width.</summary>
        public string ValueBitWidthSequenceRegister { get; set; } = "reg2";

        /// <summary>Gets or sets the bit ordering used when packing and unpacking waveform samples.</summary>
        public BitOrder BitOrder { get; set; } = BitOrder.MsbFirst;
    }

    /// <summary>
    /// SPI protocol parameters. Defaults match the SPI template patterns; override only what you need.
    /// </summary>
    public sealed class SPIProtocolParameters : DigitalProtocolParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SPIProtocolParameters"/> class with SPI defaults.
        /// </summary>
        public SPIProtocolParameters()
        {
            AddressBitWidth = 16;
            ValueBitWidth = 16;
            PinNames = new[] { "SDI", "SDO" };
            ReadPatternName = "SPI_read_template";
            WritePatternName = "SPI_write_template";
        }
    }

    /// <summary>
    /// I2C protocol parameters. Defaults match the I2C template patterns; override only what you need.
    /// </summary>
    public sealed class I2CProtocolParameters : DigitalProtocolParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2CProtocolParameters"/> class with I2C defaults.
        /// </summary>
        public I2CProtocolParameters()
        {
            AddressBitWidth = 8;
            ValueBitWidth = 8;
            PinNames = new[] { "SDA", "SCL" };
            ReadPatternName = "I2C_read_template";
            WritePatternName = "I2C_write_template";
        }
    }
}
