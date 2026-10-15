using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.DutControl
{
    // The validation helpers are exercised directly because the register operations that call them
    // cannot run without instrument sessions.
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
