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
    public class WriteValueToRegisterAndCompareReadbackValueTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";
        private const string SupportingMaterialsFolderPath = @"\Examples\source\Sequence\RegisterIO.SPI-I2C\Supporting Materials";
        private const string DigitalPatternProjectName = "STLExample.RegisterIO.SPIAndI2C.digiproj";

        [Theory(Skip = "Requires a physical digital pattern instrument connected to a DUT. Not supported in offline mode.")]
        [InlineData(CommunicationProtocol.SPI)]
        [InlineData(CommunicationProtocol.I2C)]
        public void WriteValueToRegisterAndCompareReadbackValue_SharedValue_ReadbackMatches(CommunicationProtocol protocol)
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
                addressBitWidth: isI2C ? 8 : 7,
                valueBitWidth: 8,
                writePatternName: isI2C ? "I2C_write_template" : "SPI_write_template",
                readPatternName: isI2C ? "I2C_read_template" : "SPI_read_template",
                sourceWaveformName: isI2C ? "i2c_source_buffer" : "source_buffer",
                captureWaveformName: isI2C ? "i2c_capture_buffer" : "capture_buffer",
                sampleWidth: 1,
                readWriteCountSequenceRegister: "reg0",
                addressBitWidthSequenceRegister: "reg1",
                valueBitWidthSequenceRegister: "reg2",
                pinNames: new[] { "CS", "SCK", "SDI", "SDO" });

            // SPI sends the read/write bit followed by a 7-bit register address, so BME280 register 0xF4
            // is addressed as 0x74. I2C sends the full 8-bit register address.
            TestStep.WriteValueToRegisterAndCompareReadbackValue(
                tsmContext,
                protocol,
                registerAddress: isI2C ? 0xF4u : 0x74u,
                valueToWrite: 0x27);

            var comparisonResults = publishedDataReader.GetAndClearPublishedData().Where(d => d.PublishedDataId == "ComparisonResult").ToArray();
            Assert.Equal(tsmContext.SiteNumbers.Count, comparisonResults.Length);
            Assert.All(comparisonResults, d => Assert.True(d.BooleanValue));

            DutPowerDown(
                tsmContext,
                dutSupplyPinsOrPinGroups: new string[] { "VIN" },
                settlingTime: 0,
                powerDownSuppliesSerially: false);
            TestStep.CleanupDigitalProtocol(protocol);
        }
    }
}
