using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPI
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
    /// Defines a digital protocol interface for reading and writing DUT registers.
    /// Implementations provide protocol-specific waveform encoding, pattern bursting,
    /// and capture decoding (e.g., I2C, SPI).
    /// </summary>
    public interface IDigitalProtocol
    {
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
