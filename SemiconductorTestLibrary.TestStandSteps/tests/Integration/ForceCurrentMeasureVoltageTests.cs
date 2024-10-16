using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class ForceCurrentMeasureVoltageTests
    {
        [Fact]
        public void InitializeDigital_RunForceCurrentMeasureVoltageWithPositiveInRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow);
                Assert.Equal(3.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void InitializeDigital_RunForceCurrentMeasureVoltageWithPositiveOutRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 1.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1);
                Assert.Equal(1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void InitializeDigital_RunForceCurrentMeasureVoltageWithNegativeInRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: -1.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1);
                Assert.Equal(1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void InitializeDigital_RunForceCurrentMeasureVoltageWithNegativeOutRangeVoltageLimit_VoltageLimitsCorrectlySet()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: -3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo =>
            {
                Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow);
                Assert.Equal(3.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitHigh, 1);
            });
            CleanupInstrumentation(tsmContext);
        }
    }
}
