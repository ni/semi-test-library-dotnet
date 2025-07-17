using System.Linq;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class AcquireAnalogInputWaveformsTests
    {
        [Fact]
        public void DAQmxTestAcquireAnalogInputWaveforms()
        {
            var tsmContext = CreateTSMContext("DAQmxTests.pinmap", out var publishedDataReader);
            SetupNIDAQmxAIVoltageTask(tsmContext);

            AcquireAnalogInputWaveforms(
                tsmContext,
                pinsOrPinGroups: new[] { "AllAIPins" });

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            // Validate Maximum Value
            var publishedDataMaximum = publishedData.Where(d => d.PublishedDataId == "Maximum");
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataMaximum.Where(d => d.Pin == "VCC1").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataMaximum.Where(d => d.Pin == "VCC2").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataMaximum.Where(d => d.Pin == "VDET").Count());
            foreach (var data in publishedDataMaximum)
            {
                Assert.InRange(data.DoubleValue, 9, 10);
            }
            // Validate Minimum Value
            var publishedDataMinimum = publishedData.Where(d => d.PublishedDataId == "Minimum");
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataMinimum.Where(d => d.Pin == "VCC1").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataMinimum.Where(d => d.Pin == "VCC2").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataMinimum.Where(d => d.Pin == "VDET").Count());
            foreach (var data in publishedDataMinimum)
            {
                Assert.InRange(data.DoubleValue, -10, -9);
            }
            CleanupInstrumentation(tsmContext);
        }
    }
}
