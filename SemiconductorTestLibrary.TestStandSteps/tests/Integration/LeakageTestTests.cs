using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
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
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.STSNIBCauvery))]
    public class LeakageTestTests
    {
        [Fact]
        public void Initialize_RunLeakageTestWithPositiveLevel_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
               voltageLevel: 3.3,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunLeakageTestWithNegativeLevel_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
               voltageLevel: -1.1,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            string[] allPins = new string[] { "VCC1", "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunLeakageTestWithDigitalPinsOnly_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "DigitalPins" },
               voltageLevel: 1.1,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            string[] allPins = new string[] { "PA_EN", "C0", "C1" };
            AssertPublishedData(tsmContext, allPins, publishedDataReader);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunLeakageTestWithHighVoltageLevel_ThrowsNISemiconductorTestException()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            void LeakageTestMethod() => LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
               voltageLevel: 60,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            var exception = Assert.Throws<NISemiconductorTestException>(LeakageTestMethod);
            Assert.Contains("Requested value is not a supported value for this property.", exception.Message);
            Assert.Contains("An exception occurred while processing pins/sites:", exception.Message);
            Assert.Contains("site1/PA_EN", exception.Message);
            Assert.Contains("site1/C0", exception.Message);
            Assert.Contains("site1/C1", exception.Message);
            CleanupInstrumentation(tsmContext);
        }

        private void AssertPublishedData(ISemiconductorModuleContext tsmContext, string[] allPins, IPublishedDataReader publishedDataReader)
        {
            var publishedData = publishedDataReader.GetAndClearPublishedData();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, allPins, publishedData);
            // Limits are based on the expected value returned by the driver when in Offline Mode.
            AssertPublishedDataValueInRange(publishedData, -0.05, 0.05);
            AssertPublishedDataId("Leakage", publishedData);
        }
    }
}
