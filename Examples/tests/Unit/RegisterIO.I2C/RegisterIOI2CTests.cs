using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.I2C;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.I2C.Test
{
    public class LongToU32SamplesTests_LsbFirst
    {
        [Fact]
        public void SingleSample_8BitValue_8BitWidth_ReturnsCorrectSample()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0xAB, valueBitWidth: 8, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Single(result);
            Assert.Equal(0xABu, result[0]);
        }

        [Fact]
        public void TwoSamples_16BitValue_8BitWidth_ReturnsTwoSamples()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x34u, result[0]); // Low byte first
            Assert.Equal(0x12u, result[1]); // High byte second
        }

        [Fact]
        public void SingleSample_16BitValue_16BitWidth_ReturnsSingleSample()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0xBEEF, valueBitWidth: 16, sampleBitWidth: 16, BitOrder.LsbFirst);

            Assert.Single(result);
            Assert.Equal(0xBEEFu, result[0]);
        }

        [Fact]
        public void FourSamples_32BitValue_8BitWidth_ReturnsFourSamples()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0xDEADBEEF, valueBitWidth: 32, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(4, result.Length);
            Assert.Equal(0xEFu, result[0]);
            Assert.Equal(0xBEu, result[1]);
            Assert.Equal(0xADu, result[2]);
            Assert.Equal(0xDEu, result[3]);
        }

        [Fact]
        public void ZeroValue_ReturnsAllZeroSamples()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0u, result[0]);
            Assert.Equal(0u, result[1]);
        }

        [Fact]
        public void MaxByteValue_8BitWidth_ReturnsMaxByte()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0xFF, valueBitWidth: 8, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Single(result);
            Assert.Equal(0xFFu, result[0]);
        }

        [Fact]
        public void PartialSample_7BitValue_4BitWidth_ReturnsTwoSamples()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0x5A, valueBitWidth: 7, sampleBitWidth: 4, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0xAu, result[0]); // Low nibble
            Assert.Equal(0x5u, result[1]); // High nibble
        }
    }

    public class LongToU32SamplesTests_MsbFirst
    {
        [Fact]
        public void SingleSample_8BitValue_8BitWidth_ReturnsCorrectSample()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0xAB, valueBitWidth: 8, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Single(result);
            Assert.Equal(0xABu, result[0]);
        }

        [Fact]
        public void TwoSamples_16BitValue_8BitWidth_ReturnsMsbFirst()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x12u, result[0]); // High byte first
            Assert.Equal(0x34u, result[1]); // Low byte second
        }

        [Fact]
        public void FourSamples_32BitValue_8BitWidth_ReturnsMsbFirst()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0xDEADBEEF, valueBitWidth: 32, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(4, result.Length);
            Assert.Equal(0xDEu, result[0]);
            Assert.Equal(0xADu, result[1]);
            Assert.Equal(0xBEu, result[2]);
            Assert.Equal(0xEFu, result[3]);
        }

        [Fact]
        public void DefaultBitOrder_IsMsbFirst()
        {
            uint[] result = RegisterIO_I2C.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x12u, result[0]); // High byte first
            Assert.Equal(0x34u, result[1]); // Low byte second
        }
    }

    public class U32SamplesToLongTests_LsbFirst
    {
        [Fact]
        public void SingleSample_8Bit_ReturnsCorrectValue()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(new uint[] { 0xAB }, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(0xAB, result);
        }

        [Fact]
        public void TwoSamples_8Bit_ReturnsCorrect16BitValue()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(new uint[] { 0x34, 0x12 }, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(0x1234, result);
        }

        [Fact]
        public void FourSamples_8Bit_ReturnsCorrect32BitValue()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(
                new uint[] { 0xEF, 0xBE, 0xAD, 0xDE }, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(0xDEADBEEF, result);
        }

        [Fact]
        public void SingleSample_16Bit_ReturnsCorrectValue()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(new uint[] { 0xBEEF }, sampleBitWidth: 16, BitOrder.LsbFirst);

            Assert.Equal(0xBEEF, result);
        }

        [Fact]
        public void AllZeroSamples_ReturnsZero()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(new uint[] { 0, 0 }, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(0, result);
        }
    }

    public class U32SamplesToLongTests_MsbFirst
    {
        [Fact]
        public void TwoSamples_8Bit_MsbFirst_ReturnsCorrectValue()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(new uint[] { 0x12, 0x34 }, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(0x1234, result);
        }

        [Fact]
        public void FourSamples_8Bit_MsbFirst_ReturnsCorrectValue()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(
                new uint[] { 0xDE, 0xAD, 0xBE, 0xEF }, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(0xDEADBEEF, result);
        }

        [Fact]
        public void DefaultBitOrder_IsMsbFirst()
        {
            long result = RegisterIO_I2C.U32SamplesToLong(new uint[] { 0x12, 0x34 }, sampleBitWidth: 8);

            Assert.Equal(0x1234, result);
        }
    }

    public class RoundTripConversionTests
    {
        [Theory]
        [InlineData(0x00, 8, 8)]
        [InlineData(0xFF, 8, 8)]
        [InlineData(0x1234, 16, 8)]
        [InlineData(0xBEEF, 16, 16)]
        [InlineData(0xDEADBEEF, 32, 8)]
        [InlineData(0x48, 8, 4)]
        public void LsbFirst_LongToSamples_ThenSamplesToLong_RoundTrips(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = RegisterIO_I2C.LongToU32Samples(value, valueBitWidth, sampleBitWidth, BitOrder.LsbFirst);
            long roundTripped = RegisterIO_I2C.U32SamplesToLong(samples, sampleBitWidth, BitOrder.LsbFirst);

            Assert.Equal(value, roundTripped);
        }

        [Theory]
        [InlineData(0x00, 8, 8)]
        [InlineData(0xFF, 8, 8)]
        [InlineData(0x1234, 16, 8)]
        [InlineData(0xBEEF, 16, 16)]
        [InlineData(0xDEADBEEF, 32, 8)]
        [InlineData(0x48, 8, 4)]
        public void MsbFirst_LongToSamples_ThenSamplesToLong_RoundTrips(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = RegisterIO_I2C.LongToU32Samples(value, valueBitWidth, sampleBitWidth, BitOrder.MsbFirst);
            long roundTripped = RegisterIO_I2C.U32SamplesToLong(samples, sampleBitWidth, BitOrder.MsbFirst);

            Assert.Equal(value, roundTripped);
        }
    }

    public class LongArrayToU32SamplesTests
    {
        [Fact]
        public void LsbFirst_TwoValues_8BitWidth_ReturnsConcatenatedSamples()
        {
            long[] values = new long[] { 0xAB, 0xCD };
            uint[] result = RegisterIO_I2C.LongArrayToU32Samples(values, valueBitWidth: 8, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0xABu, result[0]);
            Assert.Equal(0xCDu, result[1]);
        }

        [Fact]
        public void LsbFirst_TwoValues_16BitWidth_8BitSample_ReturnsFourSamples()
        {
            long[] values = new long[] { 0x1234, 0x5678 };
            uint[] result = RegisterIO_I2C.LongArrayToU32Samples(values, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(4, result.Length);
            Assert.Equal(0x34u, result[0]);
            Assert.Equal(0x12u, result[1]);
            Assert.Equal(0x78u, result[2]);
            Assert.Equal(0x56u, result[3]);
        }

        [Fact]
        public void MsbFirst_TwoValues_16BitWidth_8BitSample_ReturnsFourSamples()
        {
            long[] values = new long[] { 0x1234, 0x5678 };
            uint[] result = RegisterIO_I2C.LongArrayToU32Samples(values, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(4, result.Length);
            Assert.Equal(0x12u, result[0]);
            Assert.Equal(0x34u, result[1]);
            Assert.Equal(0x56u, result[2]);
            Assert.Equal(0x78u, result[3]);
        }

        [Fact]
        public void SingleValue_MatchesLongToU32Samples()
        {
            long value = 0xDEAD;
            uint[] fromArray = RegisterIO_I2C.LongArrayToU32Samples(new long[] { value }, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.MsbFirst);
            uint[] fromSingle = RegisterIO_I2C.LongToU32Samples(value, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(fromSingle, fromArray);
        }
    }

    public class U32SamplesToLongArrayTests
    {
        [Fact]
        public void LsbFirst_TwoValues_8BitSample_ReturnsCorrectValues()
        {
            uint[] samples = new uint[] { 0xAB, 0xCD };
            long[] result = RegisterIO_I2C.U32SamplesToLongArray(samples, sampleBitWidth: 8, valueCount: 2, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0xAB, result[0]);
            Assert.Equal(0xCD, result[1]);
        }

        [Fact]
        public void LsbFirst_TwoValues_16BitWidth_8BitSample_ReturnsCorrectValues()
        {
            uint[] samples = new uint[] { 0x34, 0x12, 0x78, 0x56 };
            long[] result = RegisterIO_I2C.U32SamplesToLongArray(samples, sampleBitWidth: 8, valueCount: 2, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x1234, result[0]);
            Assert.Equal(0x5678, result[1]);
        }

        [Fact]
        public void MsbFirst_TwoValues_16BitWidth_8BitSample_ReturnsCorrectValues()
        {
            uint[] samples = new uint[] { 0x12, 0x34, 0x56, 0x78 };
            long[] result = RegisterIO_I2C.U32SamplesToLongArray(samples, sampleBitWidth: 8, valueCount: 2, BitOrder.MsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x1234, result[0]);
            Assert.Equal(0x5678, result[1]);
        }

        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void LsbFirst_ArrayRoundTrip_LongArrayToSamples_ThenBack(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = RegisterIO_I2C.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth, BitOrder.LsbFirst);
            long[] roundTripped = RegisterIO_I2C.U32SamplesToLongArray(samples, sampleBitWidth, values.Length, BitOrder.LsbFirst);

            Assert.Equal(values, roundTripped);
        }

        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void MsbFirst_ArrayRoundTrip_LongArrayToSamples_ThenBack(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = RegisterIO_I2C.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth, BitOrder.MsbFirst);
            long[] roundTripped = RegisterIO_I2C.U32SamplesToLongArray(samples, sampleBitWidth, values.Length, BitOrder.MsbFirst);

            Assert.Equal(values, roundTripped);
        }
    }
}
