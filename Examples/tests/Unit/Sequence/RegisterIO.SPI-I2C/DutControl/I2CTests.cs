using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.DutControl
{
    [Collection("NonParallelizable")]
    public class I2CDefaultParametersTests
    {
        // A fresh instance is used so the defaults are not affected by tests that configure I2C.Instance.
        private readonly I2C _i2c = new I2C();

        [Fact]
        public void I2CInstance_Default_HasI2CPatternNames()
        {
            Assert.Equal("I2C_write_pattern", _i2c.WritePatternName);
            Assert.Equal("I2C_read_pattern", _i2c.ReadPatternName);
        }

        [Fact]
        public void I2CInstance_Default_HasSdiSdoAndSckPinNames()
        {
            Assert.Equal(new[] { "SDI", "SDO", "SCK" }, _i2c.PinNames);
        }

        [Fact]
        public void I2CInstance_Default_UsesSdiAndSdoForSourceAndCapture()
        {
            Assert.Equal("SDI", _i2c.SourcePinName);
            Assert.Equal("SDO", _i2c.CapturePinName);
        }

        [Fact]
        public void I2CInstance_AccessedMultipleTimes_ReturnsSameInstance()
        {
            Assert.Same(I2C.Instance, I2C.Instance);
        }
    }
}