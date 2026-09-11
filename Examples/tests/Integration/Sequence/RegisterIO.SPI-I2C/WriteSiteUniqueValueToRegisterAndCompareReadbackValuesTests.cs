using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C;
using NationalInstruments.Examples.SemiconductorTestLibrary.RegisterIO.SPIAndI2C.DutControl;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class WriteSiteUniqueValueToRegisterAndCompareReadbackValuesTests
    {
        private const string PinMapFileName = "STLExample.RegisterIO.SPIAndI2C.pinmap";

        [Theory(Skip = "Requires the shared digital project and a digital pattern instrument.")]
        [InlineData(CommunicationProtocol.SPI)]
        public void WriteSiteUniqueValueToRegisterAndCompareReadbackValues_SiteUniqueValues_ReadbackMatchesPerSite(CommunicationProtocol protocol)
        {
            var tsmContext = CreateTSMContext(PinMapFileName, out _);

            SiteData<bool> comparisonResults = TestStep.WriteSiteUniqueValueToRegisterAndCompareReadbackValues(
                tsmContext, protocol, registerAddress: 0xF4, perSiteValuesToWrite: new long[] { 0x21, 0x22, 0x23, 0x24 });

            Assert.All(comparisonResults.SiteNumbers, site => Assert.True(comparisonResults.GetValue(site)));
        }
    }
}
