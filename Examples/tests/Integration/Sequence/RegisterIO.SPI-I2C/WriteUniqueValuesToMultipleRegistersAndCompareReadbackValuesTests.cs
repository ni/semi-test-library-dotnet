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
    public class WriteUniqueValuesToMultipleRegistersAndCompareReadbackValuesTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";
        private const string SupportingMaterialsFolderPath = @"\Examples\source\Sequence\RegisterIO.SPI-I2C\Supporting Materials";
        private const string DigitalPatternProjectName = "STLExample.RegisterIO.SPIAndI2C.digiproj";

        [Theory(Skip = "Requires a physical digital pattern instrument connected to a DUT. Not supported in offline mode.")]
        [InlineData(CommunicationProtocol.SPI, 7, "SPI_write_template", "SPI_read_template", "SPI_source_buffer", "SPI_capture_buffer", new uint[] { 0x74, 0x75 })]
        [InlineData(CommunicationProtocol.I2C, 8, "I2C_write_template", "I2C_read_template", "I2C_source_buffer", "I2C_capture_buffer", new uint[] { 0xF4, 0xF5 })]
        public void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues_UniqueValues_ReadbackMatches(CommunicationProtocol protocol, int addressBitWidth, string writePatternName, string readPatternName, string sourceWaveformName, string captureWaveformName, uint[] registerAddresses)
        {
            // SPI sends the read/write bit followed by a 7-bit register address, so BME280 registers 0xF4 and 0xF5
            // are addressed as 0x74 and 0x75. I2C sends the full 8-bit register address.
            var registerValues = new long[] { 0x33, 0x12 };
            var tsmContext = CreateTSMContext(SupportingMaterialsFolderPath, PinMapFileName, DigitalPatternProjectName, out var publishedDataReader);
            SetupNIDigitalPatternInstrumentation(tsmContext);
            try
            {
            DutPowerUp(
                tsmContext: tsmContext,
                dutSupplyPinsOrPinGroups: new string[] { "VIN" },
                perSupplyPinOrPinGroupVoltages: new double[] { 3.3 },
                perSupplyPinOrPinGroupCurrentLimits: new double[] { 0.002 },
                settlingTime: 0,
                powerUpSuppliesSerially: false);
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

                TestStep.WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(
                    tsmContext,
                    protocol,
                    registerAddresses: registerAddresses,
                    valuesToWrite: registerValues);

                var publishedData = publishedDataReader.GetAndClearPublishedData();
                foreach (var registerAddress in registerAddresses)
                {
                    var comparisonResults = publishedData
                        .Where(d => d.PublishedDataId == $"Register_0x{registerAddress:X2}_Comparison")
                        .ToArray();
                    Assert.Equal(tsmContext.SiteNumbers.Count, comparisonResults.Length);
                    Assert.All(comparisonResults, d => Assert.True(d.BooleanValue));
                }
            }
            finally
            {
                TestStep.CleanupDigitalProtocol(protocol);
                DutPowerDown(
                    tsmContext,
                    dutSupplyPinsOrPinGroups: new string[] { "VIN" },
                    settlingTime: 0,
                    powerDownSuppliesSerially: false);
                CleanupInstrumentation(tsmContext);
            }
        }
    }
}
