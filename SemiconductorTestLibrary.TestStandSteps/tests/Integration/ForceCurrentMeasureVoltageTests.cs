using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
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
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
    public class ForceCurrentMeasureVoltageTests
    {
        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithPositiveInRangeVoltageLimit_VoltageLimitsCorrectlySetAndCorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(3.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow);
                Assert.Equal(3.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithPositiveOutRangeVoltageLimit_VoltageLimitsCorrectlySetAndCorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 1.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(1.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1);
                Assert.Equal(1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithNegativeInRangeVoltageLimit_VoltageLimitsCorrectlySetAndAndCorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: -1.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(1.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1);
                Assert.Equal(1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithNegativeOutRangeVoltageLimit_VoltageLimitsCorrectlySetAndCorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: -3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var dcPower = new TSMSessionManager(tsmContext).DCPower("VCC1");
            dcPower.Do(sessionInfo =>
            {
                Assert.Equal(3.3, sessionInfo.AllChannelsOutput.Source.Current.VoltageLimit, 1);
            });
            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow);
                Assert.Equal(3.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunForceCurrentMeasureVoltageWithOutHighRangeVoltageLimit_ThrowsNISemiconductorTestException()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            void ForceCurrentMeasureVoltageMethod() => ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 8,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceCurrentMeasureVoltageMethod);
            Assert.Contains("An exception occurred while processing pins/sites:", exception.Message);
            Assert.Contains("site1/PA_EN", exception.Message);
            Assert.Contains("site1/C0", exception.Message);
            Assert.Contains("site1/C1", exception.Message);
            Assert.Contains("Maximum Value: 6", exception.Message);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_MergePinGroupRunForceCurrentMeasureVoltageAndUnmergePinGroup_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
            dcPower.MergePinGroup("MergedPowerPins");
            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "MergedPowerPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);
            dcPower.UnmergePinGroup("MergedPowerPins");

            string[] allPins = { "VCC1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        private void AssertPublishedData(ISemiconductorModuleContext tsmContext, string[] allPins, IPublishedDataReader publishedDataReader)
        {
            var publishedData = publishedDataReader.GetAndClearPublishedData();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, allPins, publishedData);
            if (tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                // Limits are based on the expected value returned by the driver when in Offline Mode.
                AssertPublishedDataValueInRange(publishedData, -0.05, 0.05);
            }
            else
            {
                // When run on tester, limits are set based on the maximum voltage limits provided.
                AssertPublishedDataValueInRange(publishedData, -3.31, 3.31);
            }
            AssertPublishedDataId("Voltage", publishedData);
        }
    }
}
