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
            var tsmContext = CreateTSMContext("SharedPinTests_MultiSite.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            var sessionmanager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerSessionsBundle = sessionmanager.DCPower("VCC1");

            dcPowerSessionsBundle.ConfigureSourceDelay(250e-6);
            dcPowerSessionsBundle.ForceVoltage(3.8, 3.2e-2, waitForSourceCompletion: true);
            var results = dcPowerSessionsBundle.MeasureCurrent();

            Assert.Equal(results.GetValue(0, "VCC1"), results.GetValue(1, "VCC1"));
            Assert.Equal(results.GetValue(0, "VCC1"), results.GetValue(2, "VCC1"));
            Assert.Equal(results.GetValue(0, "VCC1"), results.GetValue(3, "VCC1"));
            CleanupInstrumentation(tsmContext);
        }

        [Fact(Skip = "Manual Test")]
        public void InitializeMultiSiteSharedPin_MeasureCurrent_CorrectDataPresentInEachSites()
        {
            var tsmContext = CreateTSMContext("SharedPinTests_MultiSite.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            var sessionmanager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerSessionsBundle1 = sessionmanager.DCPower("VCC1").FilterBySite(0);
            DCPowerSessionsBundle dcPowerSessionsBundle2 = sessionmanager.DCPower("VCC1").FilterBySite(2);
            DCPowerSessionsBundle dcPowerSessionsBundle3 = sessionmanager.DCPower("VCC1");

            dcPowerSessionsBundle1.ConfigureSourceDelay(350e-6);
            dcPowerSessionsBundle1.ForceVoltage(1.8, 3.2e-2, waitForSourceCompletion: true);
            dcPowerSessionsBundle2.ConfigureSourceDelay(250e-6);
            dcPowerSessionsBundle2.ForceVoltage(4.8, 2.2e-2, waitForSourceCompletion: true);
            var results = dcPowerSessionsBundle3.MeasureCurrent();

            Assert.Equal(results.GetValue(0, "VCC1"), results.GetValue(1, "VCC1"));
            Assert.Equal(results.GetValue(2, "VCC1"), results.GetValue(3, "VCC1"));
            CleanupInstrumentation(tsmContext);
        }
    }
}
