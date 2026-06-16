using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.Test.DutControl
{
    public class I2cDefaultParametersTests
    {
        [Fact]
        public void I2cInstance_HasCorrectDefaultPatternNames()
        {
            Assert.Equal("I2C_write_template", I2C.Instance.WritePatternName);
            Assert.Equal("I2C_read_template", I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void I2cInstance_HasDifferentPatternNamesThanSpi()
        {
            Assert.NotEqual(SPI.Instance.WritePatternName, I2C.Instance.WritePatternName);
            Assert.NotEqual(SPI.Instance.ReadPatternName, I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void I2cInstance_HasDefaultMsbFirstBitOrder()
        {
            Assert.Equal(BitOrder.MsbFirst, I2C.Instance.BitOrder);
        }

        [Fact]
        public void I2cInstance_HasDefaultBitWidths()
        {
            Assert.Equal(16u, I2C.Instance.DefaultAddressBitWidth);
            Assert.Equal(16u, I2C.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void I2cInstance_HasSameSharedDefaultsAsSpi()
        {
            Assert.Equal(SPI.Instance.SourceWaveformName, I2C.Instance.SourceWaveformName);
            Assert.Equal(SPI.Instance.CaptureWaveformName, I2C.Instance.CaptureWaveformName);
            Assert.Equal(SPI.Instance.SampleWidth, I2C.Instance.SampleWidth);
            Assert.Equal(SPI.Instance.ReadWriteCountSequenceRegister, I2C.Instance.ReadWriteCountSequenceRegister);
            Assert.Equal(SPI.Instance.AddressBitWidthSequenceRegister, I2C.Instance.AddressBitWidthSequenceRegister);
            Assert.Equal(SPI.Instance.ValueBitWidthSequenceRegister, I2C.Instance.ValueBitWidthSequenceRegister);
        }

        [Fact]
        public void I2cInstance_IsSingleton()
        {
            Assert.Same(I2C.Instance, I2C.Instance);
        }
    }
}
