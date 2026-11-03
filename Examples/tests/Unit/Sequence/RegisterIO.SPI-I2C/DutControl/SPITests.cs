using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.DutControl
{
    [Collection("NonParallelizable")]
    public class SpiDefaultParametersTests
    {
        // A fresh instance is used so the defaults are not affected by tests that configure SPI.Instance.
        private readonly SPI _spi = new SPI();

        [Fact]
        public void SpiInstance_Default_HasSpiTemplatePatternNames()
        {
            Assert.Equal("SPI_write_template", _spi.WritePatternName);
            Assert.Equal("SPI_read_template", _spi.ReadPatternName);
        }

        [Fact]
        public void SpiInstance_Default_Has1BitAddressAnd8BitValueWidths()
        {
            Assert.Equal(7u, _spi.DefaultAddressBitWidth);
            Assert.Equal(8u, _spi.DefaultValueBitWidth);
        }

        [Fact]
        public void SpiInstance_Default_HasMsbFirstBitOrder()
        {
            Assert.Equal(BitOrder.MsbFirst, _spi.BitOrder);
        }

        [Fact]
        public void SpiInstance_Default_HasSourceAndCaptureBufferWaveformNames()
        {
            Assert.Equal("SPI_source_buffer", _spi.SourceWaveformName);
            Assert.Equal("SPI_capture_buffer", _spi.CaptureWaveformName);
        }

        [Fact]
        public void SpiInstance_Default_HasReg0Reg1Reg2SequencerRegisters()
        {
            Assert.Equal("reg0", _spi.ReadWriteCountSequenceRegister);
            Assert.Equal("reg1", _spi.AddressBitWidthSequenceRegister);
            Assert.Equal("reg2", _spi.ValueBitWidthSequenceRegister);
        }

        [Fact]
        public void SpiInstance_Default_HasCsSckSdiAndSdoPinNames()
        {
            Assert.Equal(new[] { "CS", "SCK", "SDI", "SDO" }, _spi.PinNames);
            Assert.Equal("SDI", _spi.SourcePinName);
            Assert.Equal("SDO", _spi.CapturePinName);
        }

        [Fact]
        public void SpiInstance_AccessedMultipleTimes_ReturnsSameInstance()
        {
            Assert.Same(SPI.Instance, SPI.Instance);
        }
    }
}
