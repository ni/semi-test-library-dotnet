using System.Linq;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class BurstPatternTests
    {
        [Fact]
        public void InitializeNIDigital_RunBurstPatternTest_ValidatePublishedData()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            BurstPattern(
                tsmContext,
                pinsOrPinGroups: new[] { "PA_EN", "C0", "C1" },
                patternName: "TX_RF");

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            // Validate busrt operation
            var publishedDataForBurst = publishedData.Where(d => d.PublishedDataId == "Pattern Pass/Fail Result");
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataForBurst.Count());
            foreach (var data in publishedDataForBurst)
            {
                Assert.True(data.BooleanValue);
            }
            // Validate published data on each pins
            var publishedDataForPins = publishedData.Where(d => d.PublishedDataId == "Pattern Fail Count");
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "PA_EN").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "C0").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "C1").Count());
            foreach (var data in publishedDataForPins)
            {
                Assert.InRange(data.DoubleValue, 0, 1);
            }
            CleanupInstrumentation(tsmContext);
        }
    }
}
