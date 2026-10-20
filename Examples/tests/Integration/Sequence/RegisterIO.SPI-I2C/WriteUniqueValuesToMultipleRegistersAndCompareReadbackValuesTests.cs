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
        private const string SupportingMaterialsFolderPath = @"\Examples\source\Sequence\RegisterIO.SPI-I2C\Supporting Materials\DigitalProject";
        private const string DigitalPatternProjectName = "STLExample.RegisterIO.SPIAndI2C.digiproj";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI)]
        public void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues_UniqueValues_ReadbackMatches(CommunicationProtocol protocol)
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
                sourceWaveformName: "source_buffer",
                captureWaveformName: "capture_buffer",
                sampleWidth: 1,
                readWriteCountSequenceRegister: "reg0",
                addressBitWidthSequenceRegister: "reg1",
                valueBitWidthSequenceRegister: "reg2",
                pinNames: new[] { "CS", "SCK", "SDI", "SDO" });

            const uint registerAddress = 0x74;
            TestStep.WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(
                tsmContext,
                protocol,
                registerAddresses: new uint[] { registerAddress },
                valuesToWrite: new long[] { 0x33 });

            var comparisonResults = publishedDataReader.GetAndClearPublishedData()
                .Where(d => d.PublishedDataId == $"Register_0x{registerAddress:X2}_Comparison")
                .ToArray();
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
