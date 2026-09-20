using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.TestSteps
{
    public class ConfigureDigitalProtocolSPITests
    {
        [Fact]
        public void ConfigureDigitalProtocol_AddressBitWidth_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, addressBitWidth: 8);

            Assert.Equal(8u, SPI.Instance.DefaultAddressBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_ValueBitWidth_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, valueBitWidth: 32);

            Assert.Equal(32u, SPI.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_WritePatternName_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, writePatternName: "custom_spi_write");

            Assert.Equal("custom_spi_write", SPI.Instance.WritePatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_ReadPatternName_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, readPatternName: "custom_spi_read");

            Assert.Equal("custom_spi_read", SPI.Instance.ReadPatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_PinNames_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, pinNames: new[] { "MOSI", "MISO" });

            Assert.Equal(new[] { "MOSI", "MISO" }, SPI.Instance.PinNames);
        }

        [Fact]
        public void ConfigureDigitalProtocol_WaveformNames_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.SPI,
                sourceWaveformName: "src_wfm",
                captureWaveformName: "cap_wfm");

            Assert.Equal("src_wfm", SPI.Instance.SourceWaveformName);
            Assert.Equal("cap_wfm", SPI.Instance.CaptureWaveformName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SequencerRegisters_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.SPI,
                readWriteCountSequenceRegister: "reg3",
                addressBitWidthSequenceRegister: "reg4",
                valueBitWidthSequenceRegister: "reg5");

            Assert.Equal("reg3", SPI.Instance.ReadWriteCountSequenceRegister);
            Assert.Equal("reg4", SPI.Instance.AddressBitWidthSequenceRegister);
            Assert.Equal("reg5", SPI.Instance.ValueBitWidthSequenceRegister);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SampleWidth_UpdatesSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, sampleWidth: 16);

            Assert.Equal(16u, SPI.Instance.SampleWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_NullPinNames_DoesNotOverridePinNames()
        {
            SPI.Instance.PinNames = new[] { "SDI", "SDO" };

            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, pinNames: null);

            Assert.Equal(new[] { "SDI", "SDO" }, SPI.Instance.PinNames);
        }
    }

    public class ConfigureDigitalProtocolI2CTests
    {
        [Fact]
        public void ConfigureDigitalProtocol_AddressBitWidth_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, addressBitWidth: 8);

            Assert.Equal(8u, I2C.Instance.DefaultAddressBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_ValueBitWidth_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, valueBitWidth: 32);

            Assert.Equal(32u, I2C.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_WritePatternName_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, writePatternName: "custom_i2c_write");

            Assert.Equal("custom_i2c_write", I2C.Instance.WritePatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_ReadPatternName_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, readPatternName: "custom_i2c_read");

            Assert.Equal("custom_i2c_read", I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_PinNames_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, pinNames: new[] { "SDA", "SCL" });

            Assert.Equal(new[] { "SDA", "SCL" }, I2C.Instance.PinNames);
        }

        [Fact]
        public void ConfigureDigitalProtocol_WaveformNames_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.I2C,
                sourceWaveformName: "src_wfm",
                captureWaveformName: "cap_wfm");

            Assert.Equal("src_wfm", I2C.Instance.SourceWaveformName);
            Assert.Equal("cap_wfm", I2C.Instance.CaptureWaveformName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SequencerRegisters_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.I2C,
                readWriteCountSequenceRegister: "reg3",
                addressBitWidthSequenceRegister: "reg4",
                valueBitWidthSequenceRegister: "reg5");

            Assert.Equal("reg3", I2C.Instance.ReadWriteCountSequenceRegister);
            Assert.Equal("reg4", I2C.Instance.AddressBitWidthSequenceRegister);
            Assert.Equal("reg5", I2C.Instance.ValueBitWidthSequenceRegister);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SampleWidth_UpdatesI2cInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, sampleWidth: 16);

            Assert.Equal(16u, I2C.Instance.SampleWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_NullPinNames_DoesNotOverridePinNames()
        {
            I2C.Instance.PinNames = new[] { "SDI", "SDO" };

            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, pinNames: null);

            Assert.Equal(new[] { "SDI", "SDO" }, I2C.Instance.PinNames);
        }

        [Fact]
        public void ConfigureDigitalProtocol_DefaultPatternNames_DoesNotOverrideI2cPatternNames()
        {
            I2C.Instance.WritePatternName = "I2C_write_template";
            I2C.Instance.ReadPatternName = "I2C_read_template";

            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, addressBitWidth: 8);

            Assert.Equal("I2C_write_template", I2C.Instance.WritePatternName);
            Assert.Equal("I2C_read_template", I2C.Instance.ReadPatternName);
        }
    }
}
