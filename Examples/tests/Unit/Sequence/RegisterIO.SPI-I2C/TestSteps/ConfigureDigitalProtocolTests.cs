using System;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.DutControl;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SpiAndI2c.Test.TestSteps
{
    public class ConfigureDigitalProtocolSPITests
    {
        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsAddressBitWidth()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, addressBitWidth: 8);

            Assert.Equal(8u, SPI.Instance.DefaultAddressBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsValueBitWidth()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, valueBitWidth: 32);

            Assert.Equal(32u, SPI.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsWritePatternName()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, writePatternName: "custom_spi_write");

            Assert.Equal("custom_spi_write", SPI.Instance.WritePatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsReadPatternName()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, readPatternName: "custom_spi_read");

            Assert.Equal("custom_spi_read", SPI.Instance.ReadPatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsPinNames()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, pinNames: new[] { "MOSI", "MISO" });

            Assert.Equal(new[] { "MOSI", "MISO" }, SPI.Instance.PinNames);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsWaveformNames()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.SPI,
                sourceWaveformName: "src_wfm",
                captureWaveformName: "cap_wfm");

            Assert.Equal("src_wfm", SPI.Instance.SourceWaveformName);
            Assert.Equal("cap_wfm", SPI.Instance.CaptureWaveformName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_SetsSequencerRegisters()
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
        public void ConfigureDigitalProtocol_SPI_SetsSampleWidth()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, sampleWidth: 16);

            Assert.Equal(16u, SPI.Instance.SampleWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SPI_NullPinNames_DoesNotOverridePinNames()
        {
            SPI.Instance.PinNames = new[] { "SDI", "SDO" };

            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, pinNames: null);

            Assert.Equal(new[] { "SDI", "SDO" }, SPI.Instance.PinNames);
        }
    }

    public class ConfigureDigitalProtocolI2CTests
    {
        [Fact]
        public void ConfigureDigitalProtocol_I2C_WithExplicitPatternNames_SetsPatternNames()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.I2C,
                writePatternName: "I2C_write_template",
                readPatternName: "I2C_read_template");

            Assert.Equal("I2C_write_template", I2C.Instance.WritePatternName);
            Assert.Equal("I2C_read_template", I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_I2C_WithSpiDefaultWritePattern_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                TestStep.ConfigureDigitalProtocol(
                    CommunicationProtocol.I2C,
                    writePatternName: "SPI_write_template",
                    readPatternName: "I2C_read_template"));
        }

        [Fact]
        public void ConfigureDigitalProtocol_I2C_WithSpiDefaultReadPattern_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                TestStep.ConfigureDigitalProtocol(
                    CommunicationProtocol.I2C,
                    writePatternName: "I2C_write_template",
                    readPatternName: "SPI_read_template"));
        }

        [Fact]
        public void ConfigureDigitalProtocol_I2C_WithBothSpiDefaultPatterns_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C));
        }

        [Fact]
        public void ConfigureDigitalProtocol_I2C_SetsAddressBitWidth()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.I2C,
                addressBitWidth: 8,
                writePatternName: "I2C_write_template",
                readPatternName: "I2C_read_template");

            Assert.Equal(8u, I2C.Instance.DefaultAddressBitWidth);
        }
    }
}
