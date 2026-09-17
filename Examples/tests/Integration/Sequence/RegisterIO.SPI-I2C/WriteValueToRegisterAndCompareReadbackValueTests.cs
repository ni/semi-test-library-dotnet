using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class WriteValueToRegisterAndCompareReadbackValueTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI)]
        public void WriteValueToRegisterAndCompareReadbackValue_SharedValue_ReadbackMatches(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            // BME280 ctrl_meas (0xF4) is writable and reads back the written value in sleep mode.
            SiteData<bool> comparisonResults = TestStep.WriteValueToRegisterAndCompareReadbackValue(
                tsmContext, protocol, registerAddress: 0xF4, valueToWrite: 0x27);

            Assert.All(comparisonResults.SiteNumbers, site => Assert.True(comparisonResults.GetValue(site)));
        }
    }
}
