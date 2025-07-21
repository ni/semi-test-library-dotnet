using System.Linq;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class BurstPatternTests
    {
        [Fact]
        public void InitializeNIDigital_RunBurstPatternTest_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);
            string[] digitalPins = { "PA_EN", "C0", "C1" };

            BurstPattern(
                tsmContext,
                pinsOrPinGroups: digitalPins,
                patternName: "TX_RF");

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            // Validate burst operation
            var publishedDataForBurst = publishedData.Where(d => d.PublishedDataId == "Pattern Pass/Fail Result");
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataForBurst.Count());
            foreach (var data in publishedDataForBurst)
            {
                Assert.True(data.BooleanValue);
            }
            // Validate published data on each pins
            var publishedDataForPins = publishedData.Where(d => d.PublishedDataId == "Pattern Fail Count").ToArray();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, digitalPins, publishedDataForPins);
            AssertPublishedDataValue(0, publishedDataForPins);
            CleanupInstrumentation(tsmContext);
        }
    }
}
