using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.SPI;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SPI.Test
{
    public class LongToU32SamplesTests
    {
        [Fact]
        public void SingleSample_8BitValue_8BitWidth_ReturnsCorrectSample()
        {
            uint[] result = DUTRegisterIO.LongToU32Samples(0xAB, valueBitWidth: 8, sampleBitWidth: 8);

            Assert.Single(result);
            Assert.Equal(0xABu, result[0]);
        }

        [Fact]
        public void TwoSamples_16BitValue_8BitWidth_ReturnsTwoSamples()
        {
            uint[] result = DUTRegisterIO.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x34u, result[0]); // Low byte first
            Assert.Equal(0x12u, result[1]); // High byte second
        }

        [Fact]
        public void SingleSample_16BitValue_16BitWidth_ReturnsSingleSample()
        {
            uint[] result = DUTRegisterIO.LongToU32Samples(0xBEEF, valueBitWidth: 16, sampleBitWidth: 16);

            Assert.Single(result);
            Assert.Equal(0xBEEFu, result[0]);
        }

        [Fact]
        public void FourSamples_32BitValue_8BitWidth_ReturnsFourSamples()
        {
            uint[] result = DUTRegisterIO.LongToU32Samples(0xDEADBEEF, valueBitWidth: 32, sampleBitWidth: 8);

            Assert.Equal(4, result.Length);
            Assert.Equal(0xEFu, result[0]);
            Assert.Equal(0xBEu, result[1]);
            Assert.Equal(0xADu, result[2]);
            Assert.Equal(0xDEu, result[3]);
        }

        [Fact]
        public void ZeroValue_ReturnsAllZeroSamples()
        {
            uint[] result = DUTRegisterIO.LongToU32Samples(0, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(2, result.Length);
            Assert.Equal(0u, result[0]);
            Assert.Equal(0u, result[1]);
        }

        [Fact]
        public void MaxByteValue_8BitWidth_ReturnsMaxByte()
        {
            uint[] result = DUTRegisterIO.LongToU32Samples(0xFF, valueBitWidth: 8, sampleBitWidth: 8);

            Assert.Single(result);
            Assert.Equal(0xFFu, result[0]);
        }

        [Fact]
        public void PartialSample_7BitValue_4BitWidth_ReturnsTwoSamples()
        {
            // 7 bits / 4 bits per sample = 2 samples (ceil)
            uint[] result = DUTRegisterIO.LongToU32Samples(0x5A, valueBitWidth: 7, sampleBitWidth: 4);

            Assert.Equal(2, result.Length);
            Assert.Equal(0xAu, result[0]); // Low nibble
            Assert.Equal(0x5u, result[1]); // High nibble (masked to 3 bits of value, but sample holds 4)
        }
    }

    public class U32SamplesToLongTests
    {
        [Fact]
        public void SingleSample_8Bit_ReturnsCorrectValue()
        {
            long result = DUTRegisterIO.U32SamplesToLong(new uint[] { 0xAB }, sampleBitWidth: 8);

            Assert.Equal(0xAB, result);
        }

        [Fact]
        public void TwoSamples_8Bit_ReturnsCorrect16BitValue()
        {
            long result = DUTRegisterIO.U32SamplesToLong(new uint[] { 0x34, 0x12 }, sampleBitWidth: 8);

            Assert.Equal(0x1234, result);
        }

        [Fact]
        public void FourSamples_8Bit_ReturnsCorrect32BitValue()
        {
            long result = DUTRegisterIO.U32SamplesToLong(
                new uint[] { 0xEF, 0xBE, 0xAD, 0xDE }, sampleBitWidth: 8);

            Assert.Equal(0xDEADBEEF, result);
        }

        [Fact]
        public void SingleSample_16Bit_ReturnsCorrectValue()
        {
            long result = DUTRegisterIO.U32SamplesToLong(new uint[] { 0xBEEF }, sampleBitWidth: 16);

            Assert.Equal(0xBEEF, result);
        }

        [Fact]
        public void AllZeroSamples_ReturnsZero()
        {
            long result = DUTRegisterIO.U32SamplesToLong(new uint[] { 0, 0 }, sampleBitWidth: 8);

            Assert.Equal(0, result);
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
        public void LongToSamples_ThenSamplesToLong_RoundTrips(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DUTRegisterIO.LongToU32Samples(value, valueBitWidth, sampleBitWidth);
            long roundTripped = DUTRegisterIO.U32SamplesToLong(samples, sampleBitWidth);

            Assert.Equal(value, roundTripped);
        }
    }

    public class LongArrayToU32SamplesTests
    {
        [Fact]
        public void TwoValues_8BitWidth_ReturnsConcatenatedSamples()
        {
            long[] values = new long[] { 0xAB, 0xCD };
            uint[] result = DUTRegisterIO.LongArrayToU32Samples(values, valueBitWidth: 8, sampleBitWidth: 8);

            Assert.Equal(2, result.Length);
            Assert.Equal(0xABu, result[0]);
            Assert.Equal(0xCDu, result[1]);
        }

        [Fact]
        public void TwoValues_16BitWidth_8BitSample_ReturnsFourSamples()
        {
            long[] values = new long[] { 0x1234, 0x5678 };
            uint[] result = DUTRegisterIO.LongArrayToU32Samples(values, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(4, result.Length);
            Assert.Equal(0x34u, result[0]);
            Assert.Equal(0x12u, result[1]);
            Assert.Equal(0x78u, result[2]);
            Assert.Equal(0x56u, result[3]);
        }

        [Fact]
        public void SingleValue_MatchesLongToU32Samples()
        {
            long value = 0xDEAD;
            uint[] fromArray = DUTRegisterIO.LongArrayToU32Samples(new long[] { value }, valueBitWidth: 16, sampleBitWidth: 8);
            uint[] fromSingle = DUTRegisterIO.LongToU32Samples(value, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(fromSingle, fromArray);
        }
    }

    public class U32SamplesToLongArrayTests
    {
        [Fact]
        public void TwoValues_8BitSample_ReturnsCorrectValues()
        {
            uint[] samples = new uint[] { 0xAB, 0xCD };
            long[] result = DUTRegisterIO.U32SamplesToLongArray(samples, sampleBitWidth: 8, valueCount: 2);

            Assert.Equal(2, result.Length);
            Assert.Equal(0xAB, result[0]);
            Assert.Equal(0xCD, result[1]);
        }

        [Fact]
        public void TwoValues_16BitWidth_8BitSample_ReturnsCorrectValues()
        {
            uint[] samples = new uint[] { 0x34, 0x12, 0x78, 0x56 };
            long[] result = DUTRegisterIO.U32SamplesToLongArray(samples, sampleBitWidth: 8, valueCount: 2);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x1234, result[0]);
            Assert.Equal(0x5678, result[1]);
        }

        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void ArrayRoundTrip_LongArrayToSamples_ThenBack(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DUTRegisterIO.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth);
            long[] roundTripped = DUTRegisterIO.U32SamplesToLongArray(samples, sampleBitWidth, values.Length);

            Assert.Equal(values, roundTripped);
        }
    }
}
