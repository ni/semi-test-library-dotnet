using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class ForceDcCurrentTests
    {
        [Fact]
        public void InitializeDigital_RunForceDcCurrentWithGreaterThan2VoltageLimit_VoltageLimitLowSetToNegativeTwo()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceDcCurrent(
                tsmContext,
                pinsOrPinGroups: new[] { "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                settlingTime: 5e-5);

            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo => Assert.Equal(-2, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow));
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void InitializeDigital_RunForceDcCurrentWithLessThan2VoltageLimit_VoltageLimitLowSetToNegativeVoltageLimit()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);

            ForceDcCurrent(
                tsmContext,
                pinsOrPinGroups: new[] { "DigitalPins" },
                currentLevel: 0.005,
                voltageLimit: 1.3,
                settlingTime: 5e-5);

            var digital = new TSMSessionManager(tsmContext).Digital("DigitalPins");
            digital.Do(sessionInfo => Assert.Equal(-1.3, sessionInfo.PinSet.Ppmu.DCCurrent.VoltageLimitLow, 1));
            CleanupInstrumentation(tsmContext);
        }
    }
}
