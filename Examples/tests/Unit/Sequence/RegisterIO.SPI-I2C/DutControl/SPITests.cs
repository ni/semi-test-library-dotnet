using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.Test.DutControl
{
    public class DigitalProtocolLongToU32SamplesTests_LsbFirst
    {
        [Fact]
        public void SingleSample_8BitValue_8BitWidth_ReturnsCorrectSample()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0xAB, valueBitWidth: 8, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Single(result);
            Assert.Equal(0xABu, result[0]);
        }

        [Fact]
        public void TwoSamples_16BitValue_8BitWidth_ReturnsLsbFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x34u, result[0]); // Low byte first
            Assert.Equal(0x12u, result[1]); // High byte second
        }

        [Fact]
        public void FourSamples_32BitValue_8BitWidth_ReturnsLsbFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0xDEADBEEF, valueBitWidth: 32, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(4, result.Length);
            Assert.Equal(0xEFu, result[0]);
            Assert.Equal(0xBEu, result[1]);
            Assert.Equal(0xADu, result[2]);
            Assert.Equal(0xDEu, result[3]);
        }
    }

    public class DigitalProtocolLongToU32SamplesTests_MsbFirst
    {
        [Fact]
        public void TwoSamples_16BitValue_8BitWidth_ReturnsMsbFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x12u, result[0]); // High byte first
            Assert.Equal(0x34u, result[1]); // Low byte second
        }

        [Fact]
        public void DefaultBitOrder_IsMsbFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x12u, result[0]);
            Assert.Equal(0x34u, result[1]);
        }
    }

    public class DigitalProtocolU32SamplesToLongTests
    {
        [Fact]
        public void TwoSamples_8Bit_LsbFirst_ReturnsCorrectValue()
        {
            long result = DigitalProtocol.U32SamplesToLong(new uint[] { 0x34, 0x12 }, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(0x1234, result);
        }

        [Fact]
        public void TwoSamples_8Bit_MsbFirst_ReturnsCorrectValue()
        {
            long result = DigitalProtocol.U32SamplesToLong(new uint[] { 0x12, 0x34 }, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(0x1234, result);
        }
    }

    public class DigitalProtocolRoundTripConversionTests
    {
        [Theory]
        [InlineData(0x00, 8, 8)]
        [InlineData(0xFF, 8, 8)]
        [InlineData(0x1234, 16, 8)]
        [InlineData(0xBEEF, 16, 16)]
        [InlineData(0xDEADBEEF, 32, 8)]
        [InlineData(0x48, 8, 4)]
        public void LsbFirst_RoundTrips(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongToU32Samples(value, valueBitWidth, sampleBitWidth, BitOrder.LsbFirst);
            long roundTripped = DigitalProtocol.U32SamplesToLong(samples, sampleBitWidth, BitOrder.LsbFirst);

            Assert.Equal(value, roundTripped);
        }

        [Theory]
        [InlineData(0x00, 8, 8)]
        [InlineData(0xFF, 8, 8)]
        [InlineData(0x1234, 16, 8)]
        [InlineData(0xBEEF, 16, 16)]
        [InlineData(0xDEADBEEF, 32, 8)]
        [InlineData(0x48, 8, 4)]
        public void MsbFirst_RoundTrips(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongToU32Samples(value, valueBitWidth, sampleBitWidth, BitOrder.MsbFirst);
            long roundTripped = DigitalProtocol.U32SamplesToLong(samples, sampleBitWidth, BitOrder.MsbFirst);

            Assert.Equal(value, roundTripped);
        }
    }

    public class DigitalProtocolArrayConversionTests
    {
        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void LsbFirst_ArrayRoundTrips(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth, BitOrder.LsbFirst);
            long[] roundTripped = DigitalProtocol.U32SamplesToLongArray(samples, sampleBitWidth, values.Length, BitOrder.LsbFirst);

            Assert.Equal(values, roundTripped);
        }

        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void MsbFirst_ArrayRoundTrips(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth, BitOrder.MsbFirst);
            long[] roundTripped = DigitalProtocol.U32SamplesToLongArray(samples, sampleBitWidth, values.Length, BitOrder.MsbFirst);

            Assert.Equal(values, roundTripped);
        }
    }

    public class SpiDefaultParametersTests
    {
        [Fact]
        public void SpiInstance_HasCorrectDefaultPatternNames()
        {
            Assert.Equal("SPI_write_template", SPI.Instance.WritePatternName);
            Assert.Equal("SPI_read_template", SPI.Instance.ReadPatternName);
        }

        [Fact]
        public void SpiInstance_HasCorrectDefaultBitWidths()
        {
            Assert.Equal(16u, SPI.Instance.DefaultAddressBitWidth);
            Assert.Equal(16u, SPI.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void SpiInstance_HasDefaultMsbFirstBitOrder()
        {
            Assert.Equal(BitOrder.MsbFirst, SPI.Instance.BitOrder);
        }

        [Fact]
        public void SpiInstance_HasDefaultWaveformNames()
        {
            Assert.Equal("source_buffer", SPI.Instance.SourceWaveformName);
            Assert.Equal("capture_buffer", SPI.Instance.CaptureWaveformName);
        }

        [Fact]
        public void SpiInstance_HasDefaultSequencerRegisters()
        {
            Assert.Equal("reg0", SPI.Instance.ReadWriteCountSequenceRegister);
            Assert.Equal("reg1", SPI.Instance.AddressBitWidthSequenceRegister);
            Assert.Equal("reg2", SPI.Instance.ValueBitWidthSequenceRegister);
        }

        [Fact]
        public void SpiInstance_HasDefaultPinNames()
        {
            Assert.Equal(new[] { "SDI", "SDO" }, SPI.Instance.PinNames);
        }

        [Fact]
        public void SpiInstance_IsSingleton()
        {
            Assert.Same(SPI.Instance, SPI.Instance);
        }
    }
}
