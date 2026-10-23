using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class WriteSiteUniqueValueToRegisterAndCompareReadbackValuesTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";
        private const string SupportingMaterialsFolderPath = @"\Examples\source\Sequence\RegisterIO.SPI-I2C\Supporting Materials";
        private const string DigitalPatternProjectName = "STLExample.RegisterIO.SPIAndI2C.digiproj";

        [Theory(Skip = "Requires a physical digital pattern instrument connected to a DUT. Not supported in offline mode.")]
        [InlineData(CommunicationProtocol.SPI, 7, "SPI_write_template", "SPI_read_template", "spi_source_buffer", "spi_capture_buffer", 0x74)]
        [InlineData(CommunicationProtocol.I2C, 8, "I2C_write_template", "I2C_read_template", "i2c_source_buffer", "i2c_capture_buffer", 0xF4)]
        public void WriteSiteUniqueValueToRegisterAndCompareReadbackValues_SiteUniqueValues_ReadbackMatchesPerSite(CommunicationProtocol protocol, int addressBitWidth, string writePatternName, string readPatternName, string sourceWaveformName, string captureWaveformName, uint registerAddress)
        {
            var tsmContext = CreateTSMContext(SupportingMaterialsFolderPath, PinMapFileName, DigitalPatternProjectName, out var publishedDataReader);
            SetupNIDigitalPatternInstrumentation(tsmContext);
            DutPowerUp(
                tsmContext: tsmContext,
                dutSupplyPinsOrPinGroups: new string[] { "VIN" },
                perSupplyPinOrPinGroupVoltages: new double[] { 3.3 },
                perSupplyPinOrPinGroupCurrentLimits: new double[] { 0.002 },
                settlingTime: 0,
                powerUpSuppliesSerially: false);
            bool isI2C = protocol == CommunicationProtocol.I2C;
            TestStep.ConfigureDigitalProtocol(
                communicationProtocol: protocol,
                addressBitWidth: addressBitWidth,
                valueBitWidth: 8,
                writePatternName: writePatternName,
                readPatternName: readPatternName,
                sourceWaveformName: sourceWaveformName,
                captureWaveformName: captureWaveformName,
                sampleWidth: 1,
                readWriteCountSequenceRegister: "reg0",
                addressBitWidthSequenceRegister: "reg1",
                valueBitWidthSequenceRegister: "reg2",
                pinNames: new[] { "CS", "SCK", "SDI", "SDO" });

            // SPI sends the read/write bit followed by a 7-bit register address, so BME280 register 0xF4
            // is addressed as 0x74. I2C sends the full 8-bit register address.
            TestStep.WriteSiteUniqueValueToRegisterAndCompareReadbackValues(
                tsmContext,
                protocol,
                registerAddress: registerAddress,
                perSiteValuesToWrite: new long[] { 0x33 });

            var comparisonResults = publishedDataReader.GetAndClearPublishedData().Where(d => d.PublishedDataId == "ComparisonResult").ToArray();
            Assert.Equal(tsmContext.SiteNumbers.Count, comparisonResults.Length);
            Assert.All(comparisonResults, d => Assert.True(d.BooleanValue));

            DutPowerDown(
                tsmContext,
                dutSupplyPinsOrPinGroups: new string[] { "VIN" },
                settlingTime: 0,
                powerDownSuppliesSerially: false);
        }
    }
}
