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
            TestStep.ConfigureDigitalProtocol(
                communicationProtocol: protocol,
                addressBitWidth: 7,
                valueBitWidth: 8,
                writePatternName: "SPI_write_template",
                readPatternName: "SPI_read_template",
                sourceWaveformName: "spi_source_buffer",
                captureWaveformName: "spi_capture_buffer",
                sampleWidth: 1,
                readWriteCountSequenceRegister: "reg0",
                addressBitWidthSequenceRegister: "reg1",
                valueBitWidthSequenceRegister: "reg2",
                pinNames: new[] { "CS", "SCK", "SDI", "SDO" });

            TestStep.WriteValueToRegisterAndCompareReadbackValue(
                tsmContext,
                protocol,
                registerAddress: 0x74,
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
