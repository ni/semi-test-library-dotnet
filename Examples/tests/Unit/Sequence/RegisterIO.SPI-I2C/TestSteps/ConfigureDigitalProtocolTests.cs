using System;
using System.Reflection;
using System.Runtime.Serialization;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.Test.TestSteps
{
    [Collection("NonParallelizable")]
    public class DigitalProtocolBundleLifetimeTests
    {
        [Theory]
        [InlineData(CommunicationProtocol.SPI)]
        [InlineData(CommunicationProtocol.I2C)]
        public void ClearBundle_RetainedBundle_ClearsReferenceAndCanBeRepeated(CommunicationProtocol protocol)
        {
            IDigitalProtocol digitalProtocol = SemiconductorModuleContextExtensions.GetProtocol(protocol);
            FieldInfo bundleField = typeof(DigitalProtocol).GetField("_digitalSessionsBundle", BindingFlags.Instance | BindingFlags.NonPublic);
            var bundle = (DigitalSessionsBundle)FormatterServices.GetUninitializedObject(typeof(DigitalSessionsBundle));

            try
            {
                bundleField.SetValue(digitalProtocol, bundle);
                Assert.Same(bundle, bundleField.GetValue(digitalProtocol));

                digitalProtocol.ClearBundle();

                Assert.Null(bundleField.GetValue(digitalProtocol));

                digitalProtocol.ClearBundle();

                Assert.Null(bundleField.GetValue(digitalProtocol));
            }
            finally
            {
                digitalProtocol.ClearBundle();
            }
        }
    }

    [Collection("NonParallelizable")]
    public sealed class ConfigureDigitalProtocolSPITests : IDisposable
    {
        public ConfigureDigitalProtocolSPITests()
        {
            ResetSpiInstance();
        }

        public void Dispose()
        {
            ResetSpiInstance();
        }

        [Fact]
        public void ConfigureDigitalProtocol_UnsupportedProtocol_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.ConfigureDigitalProtocol((CommunicationProtocol)99));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(33)]
        [InlineData(-1)]
        public void ConfigureDigitalProtocol_InvalidSampleWidth_ThrowsArgumentOutOfRangeException(int sampleWidth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, sampleWidth: sampleWidth));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(65)]
        public void ConfigureDigitalProtocol_InvalidAddressBitWidth_ThrowsArgumentOutOfRangeException(int addressBitWidth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, addressBitWidth: addressBitWidth));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(65)]
        public void ConfigureDigitalProtocol_InvalidValueBitWidth_ThrowsArgumentOutOfRangeException(int valueBitWidth)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI, valueBitWidth: valueBitWidth));
        }

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
        public void ConfigureDigitalProtocol_OmittedPatternNames_KeepsSpiDefaults()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI);

            Assert.Equal("SPI_write_template", SPI.Instance.WritePatternName);
            Assert.Equal("SPI_read_template", SPI.Instance.ReadPatternName);
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
        public void ConfigureDigitalProtocol_OmittedWaveformNames_KeepsSpiDefaults()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.SPI);

            Assert.Equal("SPI_source_buffer", SPI.Instance.SourceWaveformName);
            Assert.Equal("SPI_capture_buffer", SPI.Instance.CaptureWaveformName);
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

        // The singleton is shared with other test classes, so restore the defaults after every test.
        private static void ResetSpiInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.SPI,
                writePatternName: "SPI_write_template",
                readPatternName: "SPI_read_template",
                sourceWaveformName: "SPI_source_buffer",
                captureWaveformName: "SPI_capture_buffer");
            SPI.Instance.PinNames = new[] { "SDI", "SDO" };
        }
    }

    [Collection("NonParallelizable")]
    public sealed class ConfigureDigitalProtocolI2CTests : IDisposable
    {
        public ConfigureDigitalProtocolI2CTests()
        {
            ResetI2CInstance();
        }

        public void Dispose()
        {
            ResetI2CInstance();
        }

        [Fact]
        public void ConfigureDigitalProtocol_AddressBitWidth_UpdatesI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, addressBitWidth: 8);

            Assert.Equal(8u, I2C.Instance.DefaultAddressBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_ValueBitWidth_UpdatesI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, valueBitWidth: 32);

            Assert.Equal(32u, I2C.Instance.DefaultValueBitWidth);
        }

        [Fact]
        public void ConfigureDigitalProtocol_WritePatternName_UpdatesI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, writePatternName: "custom_i2c_write");

            Assert.Equal("custom_i2c_write", I2C.Instance.WritePatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_ReadPatternName_UpdatesI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, readPatternName: "custom_i2c_read");

            Assert.Equal("custom_i2c_read", I2C.Instance.ReadPatternName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_PinNames_UpdatesI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(CommunicationProtocol.I2C, pinNames: new[] { "SDA", "SCK" });

            Assert.Equal(new[] { "SDA", "SCK" }, I2C.Instance.PinNames);
        }

        [Fact]
        public void ConfigureDigitalProtocol_WaveformNames_UpdatesI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.I2C,
                sourceWaveformName: "src_wfm",
                captureWaveformName: "cap_wfm");

            Assert.Equal("src_wfm", I2C.Instance.SourceWaveformName);
            Assert.Equal("cap_wfm", I2C.Instance.CaptureWaveformName);
        }

        [Fact]
        public void ConfigureDigitalProtocol_SequencerRegisters_UpdatesI2CInstance()
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
        public void ConfigureDigitalProtocol_SampleWidth_UpdatesI2CInstance()
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

        // The singleton is shared with other test classes, so restore the defaults after every test.
        private static void ResetI2CInstance()
        {
            TestStep.ConfigureDigitalProtocol(
                CommunicationProtocol.I2C,
                writePatternName: "I2C_write_template",
                readPatternName: "I2C_read_template",
                sourceWaveformName: "I2C_source_buffer",
                captureWaveformName: "I2C_capture_buffer");
            I2C.Instance.PinNames = new[] { "CS", "SDI", "SDO", "SCK" };
        }
    }
}
