using System;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Helpers
{
    /// <summary>
    /// Provides helper functions to encode SPI write commands into source waveform data
    /// and decode captured waveform data into register values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class assumes a standard SPI register protocol where:
    /// <list type="bullet">
    /// <item>Bit 7 of the first byte is the R/W bit (0 = write, 1 = read).</item>
    /// <item>Bits 6:0 of the first byte are the 7-bit register address.</item>
    /// <item>The second byte is the data to write or the response data read back.</item>
    /// </list>
    /// Adjust the encoding/decoding logic if your DUT uses a different SPI register protocol.
    /// </para>
    /// </remarks>
    public static class SPIWaveformHelper
    {
        /// <summary>
        /// The R/W bit value for a write command (bit 7 = 0).
        /// </summary>
        public const byte WriteBitMask = 0x00;

        /// <summary>
        /// The R/W bit value for a read command (bit 7 = 1).
        /// </summary>
        public const byte ReadBitMask = 0x80;

        /// <summary>
        /// Encodes a write command (address + data) into a source waveform sample.
        /// The result is a 16-bit value packed into a uint: [R/W(0) + 7-bit address][8-bit data].
        /// </summary>
        /// <param name="address">The 7-bit register address (0x00–0x7F).</param>
        /// <param name="data">The 8-bit data value to write.</param>
        /// <returns>A single-element uint array suitable for <c>WriteSourceWaveformBroadcast</c>.</returns>
        public static uint[] EncodeWriteCommand(byte address, byte data)
        {
            ValidateAddress(address);
            uint commandByte = (uint)((address & 0x7F) | WriteBitMask);
            uint sample = (commandByte << 8) | data;
            return new uint[] { sample };
        }

        /// <summary>
        /// Encodes a read command (address only) into a source waveform sample.
        /// The result is a 16-bit value packed into a uint: [R/W(1) + 7-bit address][0x00].
        /// </summary>
        /// <param name="address">The 7-bit register address (0x00–0x7F).</param>
        /// <returns>A single-element uint array suitable for <c>WriteSourceWaveformBroadcast</c>.</returns>
        public static uint[] EncodeReadCommand(byte address)
        {
            ValidateAddress(address);
            uint commandByte = (uint)((address & 0x7F) | ReadBitMask);
            uint sample = commandByte << 8;
            return new uint[] { sample };
        }

        /// <summary>
        /// Encodes per-site write commands into a <see cref="SiteData{T}"/> of source waveform data.
        /// </summary>
        /// <param name="address">The 7-bit register address (0x00–0x7F).</param>
        /// <param name="perSiteData">Per-site data values to write.</param>
        /// <returns>A <see cref="SiteData{T}"/> of uint arrays suitable for <c>WriteSourceWaveformSiteUnique</c>.</returns>
        public static SiteData<uint[]> EncodeWriteCommandPerSite(byte address, SiteData<byte> perSiteData)
        {
            return perSiteData.Select(data => EncodeWriteCommand(address, data));
        }

        /// <summary>
        /// Decodes a captured waveform sample into the 8-bit register data value.
        /// Extracts the low byte from the first captured sample.
        /// </summary>
        /// <param name="captureWaveform">The raw captured waveform data (at least one sample).</param>
        /// <returns>The 8-bit data value read from the register.</returns>
        public static byte DecodeReadResponse(uint[] captureWaveform)
        {
            if (captureWaveform == null || captureWaveform.Length == 0)
            {
                throw new ArgumentException("Capture waveform must contain at least one sample.");
            }
            return (byte)(captureWaveform[0] & 0xFF);
        }

        /// <summary>
        /// Decodes per-site captured waveform data into per-site register values.
        /// </summary>
        /// <param name="perSiteCaptureData">The per-site captured waveform data.</param>
        /// <returns>A <see cref="SiteData{T}"/> of decoded byte values.</returns>
        public static SiteData<byte> DecodeReadResponsePerSite(SiteData<uint[]> perSiteCaptureData)
        {
            return perSiteCaptureData.Select(waveform => DecodeReadResponse(waveform));
        }

        /// <summary>
        /// Extracts the 7-bit address from a raw 16-bit SPI command sample.
        /// </summary>
        /// <param name="commandSample">The raw 16-bit command sample.</param>
        /// <returns>The 7-bit register address.</returns>
        public static byte ExtractAddress(uint commandSample)
        {
            return (byte)((commandSample >> 8) & 0x7F);
        }

        /// <summary>
        /// Determines whether the raw 16-bit SPI command sample is a read command.
        /// </summary>
        /// <param name="commandSample">The raw 16-bit command sample.</param>
        /// <returns><c>true</c> if the R/W bit indicates a read command.</returns>
        public static bool IsReadCommand(uint commandSample)
        {
            return ((commandSample >> 8) & 0x80) != 0;
        }

        private static void ValidateAddress(byte address)
        {
            if (address > 0x7F)
            {
                throw new ArgumentOutOfRangeException(nameof(address), "SPI register address must be 7 bits (0x00–0x7F).");
            }
        }
    }
}
