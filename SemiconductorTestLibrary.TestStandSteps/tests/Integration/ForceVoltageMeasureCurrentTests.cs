using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.ChiXiao))]
    public class ForceVoltageMeasureCurrentTests
    {
        [Fact]
        public void Initialize_RunForceVoltageMeasureCurrentWithPositiveTest_CorrectDataPublished()
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
            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, allPins, publishedData);
            // Limits are set based on the expected value returned by the driver when in Offline Mode.
            AssertPublishedDataValueInRange(publishedData, -0.001, 0.001);
            AssertPublishedDataId("Current", publishedData);
            CleanupInstrumentation(tsmContext);
        }

        [Fact(Skip = "Manual Test")]
        public void InitializeMultiSiteSharedPin_MeasureCurrent_SameDataPresentInAllSites()
        {
            var tsmContext = CreateTSMContext("SharedPinTests_MultiSite.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            ForceVoltageMeasureCurrent(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1" },
                voltageLevel: 3.8,
                currentLimit: 3.2e-2,
                apertureTime: 5e-5);

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            string[] pinList = new string[] { "VCC1" };
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, pinList, publishedData);
            // Limits are set based on the expected value returned by the driver when in Offline Mode.
            AssertPublishedDataValueInRange(publishedData, -0.001, 0.001);
            AssertPublishedDataId("Current", publishedData);
            CleanupInstrumentation(tsmContext);
        }
    }
}
