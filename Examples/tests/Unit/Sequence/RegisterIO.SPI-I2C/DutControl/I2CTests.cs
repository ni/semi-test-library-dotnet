using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.DutControl
{
    public class I2cDefaultParametersTests
    {
        [Fact]
        public void I2cInstance_Default_HasI2cTemplatePatternNames()
        {
            Assert.Equal("I2C_write_template", I2C.Instance.WritePatternName);
            Assert.Equal("I2C_read_template", I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void I2cInstance_Default_Has16BitWidths()
        {
            Assert.Equal(16u, I2C.Instance.DefaultAddressBitWidth);
            Assert.Equal(16u, I2C.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void I2cInstance_Default_HasMsbFirstBitOrder()
        {
            Assert.Equal(BitOrder.MsbFirst, I2C.Instance.BitOrder);
        }

        [Fact]
        public void I2cInstance_Default_HasSourceAndCaptureBufferWaveformNames()
        {
            Assert.Equal("source_buffer", I2C.Instance.SourceWaveformName);
            Assert.Equal("capture_buffer", I2C.Instance.CaptureWaveformName);
        }

        [Fact]
        public void I2cInstance_Default_HasReg0Reg1Reg2SequencerRegisters()
        {
            Assert.Equal("reg0", I2C.Instance.ReadWriteCountSequenceRegister);
            Assert.Equal("reg1", I2C.Instance.AddressBitWidthSequenceRegister);
            Assert.Equal("reg2", I2C.Instance.ValueBitWidthSequenceRegister);
        }

        [Fact]
        public void I2cInstance_Default_HasSdiAndSdoPinNames()
        {
            Assert.Equal(new[] { "SDI", "SDO" }, I2C.Instance.PinNames);
        }

        [Fact]
        public void I2cInstance_AccessedMultipleTimes_ReturnsSameInstance()
        {
            Assert.Same(I2C.Instance, I2C.Instance);
        }
    }
}
