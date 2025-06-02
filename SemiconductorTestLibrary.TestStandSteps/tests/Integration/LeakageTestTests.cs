using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class LeakageTestTests
    {
        [Fact]
        public void Initialize_RunLeakageTestWithPositiveLevel_Succeeds()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
               voltageLevel: 3.3,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunLeakageTestWithNegativeLevel_Succeeds()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
               voltageLevel: -1.1,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_RunLeakageTestWithHighVoltageLevel_ThrowsNISemiconductorTestException()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            SetupNIDigitalPatternInstrumentation(tsmContext);

            void ForceDcVoltageMethod() => LeakageTest(
               tsmContext,
               pinsOrPinGroups: new[] { "VCC1", "DigitalPins" },
               voltageLevel: 60,
               currentLimit: 0.005,
               apertureTime: 5e-5,
               settlingTime: 5e-5);

            var exception = Assert.Throws<NISemiconductorTestException>(ForceDcVoltageMethod);
            Assert.Contains("An error occurred while processing site1/PA_EN,site1/C0,site1/C1.", exception.Message);

            CleanupInstrumentation(tsmContext);
        }
    }
}
