using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Helpers;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Test
{
    public class SPIWaveformHelperTests
    {
        [Theory]
        [InlineData(0x0F, 0xA5, 0x0FA5u)]
        [InlineData(0x00, 0x00, 0x0000u)]
        [InlineData(0x7F, 0xFF, 0x7FFFu)]
        [InlineData(0x20, 0x01, 0x2001u)]
        public void EncodeWriteCommand_ValidAddressAndData_ReturnsCorrectSample(byte address, byte data, uint expectedSample)
        {
            uint[] result = SPIWaveformHelper.EncodeWriteCommand(address, data);

            Assert.Single(result);
            Assert.Equal(expectedSample, result[0]);
        }

        [Fact]
        public void EncodeWriteCommand_WriteBitIsClear()
        {
            uint[] result = SPIWaveformHelper.EncodeWriteCommand(0x0F, 0xA5);

            Assert.False(SPIWaveformHelper.IsReadCommand(result[0]));
        }

        [Theory]
        [InlineData(0x0F, 0x8F00u)]
        [InlineData(0x00, 0x8000u)]
        [InlineData(0x7F, 0xFF00u)]
        public void EncodeReadCommand_ValidAddress_ReturnsCorrectSample(byte address, uint expectedSample)
        {
            uint[] result = SPIWaveformHelper.EncodeReadCommand(address);

            Assert.Single(result);
            Assert.Equal(expectedSample, result[0]);
        }

        [Fact]
        public void EncodeReadCommand_ReadBitIsSet()
        {
            uint[] result = SPIWaveformHelper.EncodeReadCommand(0x0F);

            Assert.True(SPIWaveformHelper.IsReadCommand(result[0]));
        }

        [Theory]
        [InlineData(0x80)]
        [InlineData(0xFF)]
        public void EncodeWriteCommand_AddressExceeds7Bits_ThrowsArgumentOutOfRangeException(byte invalidAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SPIWaveformHelper.EncodeWriteCommand(invalidAddress, 0x00));
        }

        [Theory]
        [InlineData(0x80)]
        [InlineData(0xFF)]
        public void EncodeReadCommand_AddressExceeds7Bits_ThrowsArgumentOutOfRangeException(byte invalidAddress)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SPIWaveformHelper.EncodeReadCommand(invalidAddress));
        }

        [Theory]
        [InlineData(new uint[] { 0x00A5u }, 0xA5)]
        [InlineData(new uint[] { 0x0060u }, 0x60)]
        [InlineData(new uint[] { 0x0000u }, 0x00)]
        [InlineData(new uint[] { 0x00FFu }, 0xFF)]
        [InlineData(new uint[] { 0xFF60u }, 0x60)]
        public void DecodeReadResponse_ValidWaveform_ReturnsLowByte(uint[] waveform, byte expectedData)
        {
            byte result = SPIWaveformHelper.DecodeReadResponse(waveform);

            Assert.Equal(expectedData, result);
        }

        [Fact]
        public void DecodeReadResponse_NullWaveform_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => SPIWaveformHelper.DecodeReadResponse(null));
        }

        [Fact]
        public void DecodeReadResponse_EmptyWaveform_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => SPIWaveformHelper.DecodeReadResponse(new uint[0]));
        }

        [Theory]
        [InlineData(0x0FA5u, 0x0F)]
        [InlineData(0x8F00u, 0x0F)]
        [InlineData(0xFF00u, 0x7F)]
        [InlineData(0x0000u, 0x00)]
        public void ExtractAddress_ReturnsCorrect7BitAddress(uint sample, byte expectedAddress)
        {
            byte result = SPIWaveformHelper.ExtractAddress(sample);

            Assert.Equal(expectedAddress, result);
        }

        [Fact]
        public void IsReadCommand_ReadCommandSample_ReturnsTrue()
        {
            Assert.True(SPIWaveformHelper.IsReadCommand(0x8F00u));
        }

        [Fact]
        public void IsReadCommand_WriteCommandSample_ReturnsFalse()
        {
            Assert.False(SPIWaveformHelper.IsReadCommand(0x0FA5u));
        }

        [Fact]
        public void EncodeWriteCommand_ThenDecode_RoundTripsCorrectly()
        {
            byte address = 0x20;
            byte data = 0xAB;

            uint[] encoded = SPIWaveformHelper.EncodeWriteCommand(address, data);
            byte decodedAddress = SPIWaveformHelper.ExtractAddress(encoded[0]);
            byte decodedData = SPIWaveformHelper.DecodeReadResponse(encoded);

            Assert.Equal(address, decodedAddress);
            Assert.Equal(data, decodedData);
        }

        [Fact]
        public void EncodeReadCommand_ThenExtractAddress_RoundTripsCorrectly()
        {
            byte address = 0x3C;

            uint[] encoded = SPIWaveformHelper.EncodeReadCommand(address);
            byte decodedAddress = SPIWaveformHelper.ExtractAddress(encoded[0]);

            Assert.Equal(address, decodedAddress);
            Assert.True(SPIWaveformHelper.IsReadCommand(encoded[0]));
        }
    }
}