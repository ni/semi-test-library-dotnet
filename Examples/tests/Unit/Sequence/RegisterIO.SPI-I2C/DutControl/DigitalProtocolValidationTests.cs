using System;

using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.DutControl
{
    [Collection("NonParallelizable")]
    public class DigitalProtocolLongToU32SamplesTests_LsbFirst
    {
        [Fact]
        public void LongToU32Samples_8BitValueLsbFirst_ReturnsSingleCorrectSample()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0xAB, valueBitWidth: 8, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Single(result);
            Assert.Equal(0xABu, result[0]);
        }

        [Fact]
        public void LongToU32Samples_16BitValue8BitWidthLsbFirst_ReturnsLowByteFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x34u, result[0]); // Low byte first
            Assert.Equal(0x12u, result[1]); // High byte second
        }

        [Fact]
        public void LongToU32Samples_32BitValue8BitWidthLsbFirst_ReturnsBytesLsbFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0xDEADBEEF, valueBitWidth: 32, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(4, result.Length);
            Assert.Equal(0xEFu, result[0]);
            Assert.Equal(0xBEu, result[1]);
            Assert.Equal(0xADu, result[2]);
            Assert.Equal(0xDEu, result[3]);
        }
    }

    [Collection("NonParallelizable")]
    public class DigitalProtocolLongToU32SamplesTests_MsbFirst
    {
        [Fact]
        public void LongToU32Samples_16BitValue8BitWidthMsbFirst_ReturnsHighByteFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x12u, result[0]); // High byte first
            Assert.Equal(0x34u, result[1]); // Low byte second
        }

        [Fact]
        public void LongToU32Samples_NoBitOrderSpecified_DefaultsToMsbFirst()
        {
            uint[] result = DigitalProtocol.LongToU32Samples(0x1234, valueBitWidth: 16, sampleBitWidth: 8);

            Assert.Equal(2, result.Length);
            Assert.Equal(0x12u, result[0]);
            Assert.Equal(0x34u, result[1]);
        }
    }

    [Collection("NonParallelizable")]
    public class DigitalProtocolU32SamplesToLongTests
    {
        [Fact]
        public void U32SamplesToLong_Two8BitSamplesLsbFirst_ReturnsCorrectValue()
        {
            long result = DigitalProtocol.U32SamplesToLong(new uint[] { 0x34, 0x12 }, sampleBitWidth: 8, BitOrder.LsbFirst);

            Assert.Equal(0x1234, result);
        }

        [Fact]
        public void U32SamplesToLong_Two8BitSamplesMsbFirst_ReturnsCorrectValue()
        {
            long result = DigitalProtocol.U32SamplesToLong(new uint[] { 0x12, 0x34 }, sampleBitWidth: 8, BitOrder.MsbFirst);

            Assert.Equal(0x1234, result);
        }
    }

    [Collection("NonParallelizable")]
    public class DigitalProtocolRoundTripConversionTests
    {
        [Theory]
        [InlineData(0x00, 8, 8)]
        [InlineData(0xFF, 8, 8)]
        [InlineData(0x1234, 16, 8)]
        [InlineData(0xBEEF, 16, 16)]
        [InlineData(0xDEADBEEF, 32, 8)]
        [InlineData(0x48, 8, 4)]
        public void LongToU32SamplesAndBack_LsbFirst_ReproducesOriginalValue(long value, uint valueBitWidth, uint sampleBitWidth)
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
        public void LongToU32SamplesAndBack_MsbFirst_ReproducesOriginalValue(long value, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongToU32Samples(value, valueBitWidth, sampleBitWidth, BitOrder.MsbFirst);
            long roundTripped = DigitalProtocol.U32SamplesToLong(samples, sampleBitWidth, BitOrder.MsbFirst);

            Assert.Equal(value, roundTripped);
        }
    }

    [Collection("NonParallelizable")]
    public class DigitalProtocolArrayConversionTests
    {
        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void LongArrayToU32SamplesAndBack_LsbFirst_ReproducesOriginalArray(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth, BitOrder.LsbFirst);
            long[] roundTripped = DigitalProtocol.U32SamplesToLongArray(samples, sampleBitWidth, values.Length, BitOrder.LsbFirst);

            Assert.Equal(values, roundTripped);
        }

        [Theory]
        [InlineData(new long[] { 0xAB, 0xCD }, 8, 8)]
        [InlineData(new long[] { 0x1234, 0x5678 }, 16, 8)]
        [InlineData(new long[] { 0xAA, 0xBB, 0xCC }, 8, 4)]
        public void LongArrayToU32SamplesAndBack_MsbFirst_ReproducesOriginalArray(long[] values, uint valueBitWidth, uint sampleBitWidth)
        {
            uint[] samples = DigitalProtocol.LongArrayToU32Samples(values, valueBitWidth, sampleBitWidth, BitOrder.MsbFirst);
            long[] roundTripped = DigitalProtocol.U32SamplesToLongArray(samples, sampleBitWidth, values.Length, BitOrder.MsbFirst);

            Assert.Equal(values, roundTripped);
        }
    }

    [Collection("NonParallelizable")]
    public class DigitalProtocolValidationTests
    {
        private readonly SPI _spi = new SPI();

        [Theory]
        [InlineData(0u)]
        [InlineData(33u)]
        public void SampleWidth_OutOfRange_ThrowsArgumentOutOfRangeException(uint sampleWidth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _spi.SampleWidth = sampleWidth);
        }

        [Theory]
        [InlineData(1u)]
        [InlineData(32u)]
        public void SampleWidth_InRange_IsApplied(uint sampleWidth)
        {
            _spi.SampleWidth = sampleWidth;

            Assert.Equal(sampleWidth, _spi.SampleWidth);
        }

        [Theory]
        [InlineData(0u)]
        [InlineData(65u)]
        public void DefaultBitWidths_OutOfRange_ThrowsArgumentOutOfRangeException(uint bitWidth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _spi.DefaultAddressBitWidth = bitWidth);
            Assert.Throws<ArgumentOutOfRangeException>(() => _spi.DefaultValueBitWidth = bitWidth);
        }

        [Theory]
        [InlineData(0u)]
        [InlineData(65u)]
        public void ValidateBitWidth_OutOfRange_ThrowsArgumentOutOfRangeException(uint bitWidth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DigitalProtocol.ValidateBitWidth(bitWidth, "bitWidth"));
        }

        [Theory]
        [InlineData(1u)]
        [InlineData(64u)]
        public void ValidateBitWidth_InRange_DoesNotThrow(uint bitWidth)
        {
            DigitalProtocol.ValidateBitWidth(bitWidth, "bitWidth");
        }

        [Fact]
        public void ValidateAddress_ExceedsBitWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DigitalProtocol.ValidateAddress(0x100, addressBitWidth: 8));
        }

        [Fact]
        public void ValidateAddress_FitsBitWidth_DoesNotThrow()
        {
            DigitalProtocol.ValidateAddress(0xFF, addressBitWidth: 8);
            DigitalProtocol.ValidateAddress(uint.MaxValue, addressBitWidth: 32);
            DigitalProtocol.ValidateAddress(uint.MaxValue, addressBitWidth: 64);
        }

        [Theory]
        [InlineData(0x100L)]
        [InlineData(-1L)]
        public void ValidateValue_ExceedsBitWidth_ThrowsArgumentOutOfRangeException(long value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DigitalProtocol.ValidateValue(value, valueBitWidth: 8));
        }

        [Fact]
        public void ValidateValue_FitsBitWidth_DoesNotThrow()
        {
            DigitalProtocol.ValidateValue(0xFF, valueBitWidth: 8);
            DigitalProtocol.ValidateValue(long.MaxValue, valueBitWidth: 63);
            DigitalProtocol.ValidateValue(-1L, valueBitWidth: 64);
        }

        [Fact]
        public void ValidateAddresses_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DigitalProtocol.ValidateAddresses(null));
        }

        [Fact]
        public void ValidateAddresses_Empty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DigitalProtocol.ValidateAddresses(new uint[0]));
        }

        [Fact]
        public void ValidateAddresses_ExceedsMaxRegistersPerBurst_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DigitalProtocol.ValidateAddresses(new uint[DigitalProtocol.MaxRegistersPerBurst + 1]));
        }

        [Fact]
        public void ValidateAddresses_AtMaxRegistersPerBurst_DoesNotThrow()
        {
            DigitalProtocol.ValidateAddresses(new uint[DigitalProtocol.MaxRegistersPerBurst]);
        }

        [Fact]
        public void MultiRegisterOperations_AddressExceedsBitWidth_ThrowArgumentOutOfRangeExceptionBeforeAccessingBundle()
        {
            uint[] addresses = { 0x10, 0x100 };
            var siteValue = new SiteData<long>(new[] { 0 }, new[] { 1L });

            Assert.Throws<ArgumentOutOfRangeException>(() => _spi.ReadRegisters(addresses, addressBitWidth: 8, valueBitWidth: 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => _spi.WriteRegisters(addresses, new[] { 1L, 2L }, addressBitWidth: 8, valueBitWidth: 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => _spi.WriteRegisters(addresses, new[] { siteValue, siteValue }, addressBitWidth: 8, valueBitWidth: 8));
        }

        [Theory]
        [InlineData(1u)]
        [InlineData(8u)]
        [InlineData(32u)]
        public void GetSourceWaveformSize_FitsMaxBurstAtMaxBitWidths(uint sampleWidth)
        {
            int samplesPerMaxWidthField = DigitalProtocol.LongToU32Samples(-1L, valueBitWidth: 64, sampleBitWidth: sampleWidth).Length;

            Assert.Equal(DigitalProtocol.MaxRegistersPerBurst * 2 * samplesPerMaxWidthField, DigitalProtocol.GetSourceWaveformSize(sampleWidth));
        }

        [Fact]
        public void ValidateValuesMatchAddresses_NullValues_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DigitalProtocol.ValidateValuesMatchAddresses(new uint[] { 0x10 }, null));
        }

        [Fact]
        public void ValidateValuesMatchAddresses_MismatchedLengths_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DigitalProtocol.ValidateValuesMatchAddresses(new uint[] { 0x10, 0x11 }, new long[] { 1 }));
        }

        [Fact]
        public void ValidateValuesMatchAddresses_MatchingLengths_DoesNotThrow()
        {
            DigitalProtocol.ValidateValuesMatchAddresses(new uint[] { 0x10, 0x11 }, new long[] { 1, 2 });
        }

        [Fact]
        public void ValidateSameSites_NullEntry_ThrowsArgumentException()
        {
            var values = new[] { new SiteData<long>(new long[] { 1, 2 }), null };

            Assert.Throws<ArgumentException>(() => DigitalProtocol.ValidateSameSites(values));
        }

        [Fact]
        public void ValidateSameSites_DifferentSiteSets_ThrowsArgumentException()
        {
            var values = new[]
            {
                new SiteData<long>(new[] { 0, 1 }, new long[] { 1, 2 }),
                new SiteData<long>(new[] { 0 }, new long[] { 3 })
            };

            Assert.Throws<ArgumentException>(() => DigitalProtocol.ValidateSameSites(values));
        }

        [Fact]
        public void ValidateSameSites_SameSitesInDifferentOrder_DoesNotThrow()
        {
            var values = new[]
            {
                new SiteData<long>(new[] { 0, 1 }, new long[] { 1, 2 }),
                new SiteData<long>(new[] { 1, 0 }, new long[] { 3, 4 })
            };

            DigitalProtocol.ValidateSameSites(values);
        }

        [Fact]
        public void LongToU32Samples_ZeroSampleWidth_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DigitalProtocol.LongToU32Samples(1, valueBitWidth: 8, sampleBitWidth: 0));
        }

        [Fact]
        public void U32SamplesToLongArray_ZeroValueCount_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DigitalProtocol.U32SamplesToLongArray(new uint[] { 1, 2 }, sampleBitWidth: 8, valueCount: 0));
        }

        [Fact]
        public void U32SamplesToLongArray_SamplesNotDivisibleByValueCount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => DigitalProtocol.U32SamplesToLongArray(new uint[] { 1, 2, 3 }, sampleBitWidth: 8, valueCount: 2));
        }

        [Fact]
        public void ValueAtMaximumBitWidth_RoundTripsWithoutTruncation()
        {
            uint[] samples = DigitalProtocol.LongToU32Samples(long.MaxValue, valueBitWidth: 64, sampleBitWidth: 8);

            Assert.Equal(long.MaxValue, DigitalProtocol.U32SamplesToLong(samples, sampleBitWidth: 8));
        }
    }
}
