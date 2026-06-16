using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl
{
    /// <summary>
    /// Specifies the bit ordering used when packing and unpacking waveform samples.
    /// </summary>
    public enum BitOrder
    {
        /// <summary>
        /// Most-significant bit (or sample) first.
        /// </summary>
        MsbFirst,

        /// <summary>
        /// Least-significant bit (or sample) first.
        /// </summary>
        LsbFirst
    }

    /// <summary>
    /// Defines a protocol-agnostic contract for reading and writing DUT registers.
    /// Implementations provide protocol-specific waveform encoding, pattern bursting,
    /// and capture decoding (e.g., <see cref="SPI"/>, <see cref="I2C"/>).
    /// </summary>
    public interface IDigitalProtocol
    {
        /// <summary>Gets or sets the digital pattern name used to write a register value.</summary>
        string WritePatternName { get; set; }

        /// <summary>Gets or sets the digital pattern name used to read a register value.</summary>
        string ReadPatternName { get; set; }

        /// <summary>Gets or sets the digital source waveform name used by the read and write patterns.</summary>
        string SourceWaveformName { get; set; }

        /// <summary>Gets or sets the digital capture waveform name used by the read pattern.</summary>
        string CaptureWaveformName { get; set; }

        /// <summary>Gets or sets the number of bits each digital waveform sample is.</summary>
        uint SampleWidth { get; set; }

        /// <summary>Gets or sets the sequencer register that controls the transaction count.</summary>
        string ReadWriteCountSequenceRegister { get; set; }

        /// <summary>Gets or sets the sequencer register that controls the address bit width.</summary>
        string AddressBitWidthSequenceRegister { get; set; }

        /// <summary>Gets or sets the sequencer register that controls the value bit width.</summary>
        string ValueBitWidthSequenceRegister { get; set; }

        /// <summary>Gets or sets the protocol pin names expected by the template patterns.</summary>
        string[] PinNames { get; set; }

        /// <summary>Gets or sets the default number of bits the register address is.</summary>
        uint DefaultAddressBitWidth { get; set; }

        /// <summary>Gets or sets the default number of bits the register value holds.</summary>
        uint DefaultValueBitWidth { get; set; }

        /// <summary>Gets or sets the bit ordering used when packing and unpacking waveform samples.</summary>
        BitOrder BitOrder { get; set; }

        /// <summary>
        /// Sets the <see cref="DigitalSessionsBundle"/> used for digital pattern instrument control.
        /// Must be called at least once per test module before performing any register operations.
        /// </summary>
        /// <param name="digitalSessionsBundle">The <see cref="DigitalSessionsBundle"/> object scoped to the active sites.</param>
        void SetBundle(DigitalSessionsBundle digitalSessionsBundle);

        /// <summary>
        /// Reads the value at the specified DUT register address.
        /// </summary>
        /// <param name="address">The address of the register to read.</param>
        /// <returns>The per-site value read from the register.</returns>
        SiteData<long> ReadRegister(uint address);

        /// <summary>
        /// Writes the specified broadcast value to the specified DUT register address.
        /// </summary>
        /// <param name="address">The address of the register to write the value to.</param>
        /// <param name="value">The value to write to the register.</param>
        void WriteRegister(uint address, long value);

        /// <summary>
        /// Writes the specified site-unique value to the specified DUT register address.
        /// </summary>
        /// <param name="address">The address of the register to write the value to.</param>
        /// <param name="value">The per-site value to write to the register.</param>
        void WriteRegister(uint address, SiteData<long> value);

        /// <summary>
        /// Reads the values from the specified DUT register addresses.
        /// </summary>
        /// <param name="addresses">The addresses of the registers to read.</param>
        /// <returns>The per-site values read from the registers.</returns>
        SiteData<long[]> ReadRegisters(uint[] addresses);

        /// <summary>
        /// Writes the specified register-unique values to the specified register addresses.
        /// </summary>
        /// <param name="addresses">The addresses of each register to write the values to.</param>
        /// <param name="values">The register-unique values to write to each register.</param>
        void WriteRegisters(uint[] addresses, long[] values);

        /// <summary>
        /// Writes the specified site-unique values to the specified register addresses.
        /// </summary>
        /// <param name="addresses">The addresses of each register to write the values to.</param>
        /// <param name="values">The per-site values to write to each register.</param>
        void WriteRegisters(uint[] addresses, SiteData<long>[] values);
    }
}
