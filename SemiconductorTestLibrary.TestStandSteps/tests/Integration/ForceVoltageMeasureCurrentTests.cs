using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class ForceVolategMeasureCurrentTests
    {
        [Fact]
        public void Initialize_RunForceVolatgeMeasureCurrentWithPositiveTest()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceVoltageMeasureCurrent(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                voltageLevel: 3.8,
                currentLimit: 3.2e-2,
                apertureTime: 5e-5);

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "VCC1").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "PA_EN").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "C0").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "C1").Count());
            foreach (var data in publishedData)
            {
                Assert.InRange(data.DoubleValue, 0, 0.001);
                Assert.Equal("Current", data.PublishedDataId);
            }
            CleanupInstrumentation(tsmContext);
        }
    }
}
