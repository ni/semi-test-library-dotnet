using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
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
        private const string SupportingMaterialsFolderPath = @"\Examples\source\Sequence\RegisterIO.SPI-I2C\Supporting Materials\DigitalProject";
        private const string DigitalPatternProjectName = "STLExample.RegisterIO.SPIAndI2C.digiproj";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI, 7, 8, "SPI_write_template", "SPI_read_template")]
        [InlineData(CommunicationProtocol.I2C, 8, 8, "I2C_write_template", "I2C_read_template")]
        public void WriteValueToRegisterAndCompareReadbackValue_SharedValue_ReadbackMatches(CommunicationProtocol protocol, int addressBitWidth, int valueBitWidth, string writeTemplate, string readTemplate)
        {
            var tsmContext = CreateTSMContext(SupportingMaterialsFolderPath, PinMapFileName, DigitalPatternProjectName, out _);
            SetupNIDigitalPatternInstrumentation(tsmContext);
            DutPowerUp(tsmContext, new string[] { "VIN" }, new double[] { 3.3 }, new double[] { 0.002 }, 0, false);
            TestStep.ConfigureDigitalProtocol(protocol, addressBitWidth, valueBitWidth, writeTemplate, readTemplate, "source_buffer", "capture_buffer", 1, "reg0", "reg1", "reg2", new[] { "CS", "SCK", "SDI", "SDO" });

            SiteData<bool> comparisonResults = TestStep.WriteValueToRegisterAndCompareReadbackValue(
                tsmContext, protocol, registerAddress: 0xF4, valueToWrite: 0x27);

            Assert.All(comparisonResults.SiteNumbers, site => Assert.True(comparisonResults.GetValue(site)));
            DutPowerDown(tsmContext, new string[] { "VIN" }, 0, false);
        }
    }
}
