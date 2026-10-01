using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.DutControl
{
    public class I2CDefaultParametersTests
    {
        [Fact]
        public void I2CInstance_Default_HasI2CTemplatePatternNames()
        {
            Assert.Equal("I2C_write_template", I2C.Instance.WritePatternName);
            Assert.Equal("I2C_read_template", I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void I2CInstance_Default_HasSdiSdoAndSckPinNames()
        {
            Assert.Equal(new[] { "SDI", "SDO", "SCK" }, I2C.Instance.PinNames);
        }

        [Fact]
        public void I2CInstance_Default_UsesSdiAndSdoForSourceAndCapture()
        {
            Assert.Equal("SDI", I2C.Instance.SourcePinName);
            Assert.Equal("SDO", I2C.Instance.CapturePinName);
        }

        [Fact]
        public void I2CInstance_AccessedMultipleTimes_ReturnsSameInstance()
        {
            Assert.Same(I2C.Instance, I2C.Instance);
        }
    }
}