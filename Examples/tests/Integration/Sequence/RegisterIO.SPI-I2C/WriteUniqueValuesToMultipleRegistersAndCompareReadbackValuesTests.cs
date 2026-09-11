using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class WriteUniqueValuesToMultipleRegistersAndCompareReadbackValuesTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI)]
        public void WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues_UniqueValues_ReadbackMatches(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            // BME280 writable config registers: ctrl_hum (0xF2), ctrl_meas (0xF4), config (0xF5).
            SiteData<bool[]> comparisonResults = TestStep.WriteUniqueValuesToMultipleRegistersAndCompareReadbackValues(
                tsmContext, protocol, registerAddresses: new uint[] { 0xF2, 0xF4, 0xF5 }, valuesToWrite: new long[] { 0x01, 0x27, 0xA0 });

            Assert.All(comparisonResults.SiteNumbers, site => Assert.All(comparisonResults.GetValue(site), Assert.True));
        }
    }
}
