using NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction;
using NationalInstruments.ModularInstruments.NIDCPower;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class InstrumentAbstractionTests
    {
        [Fact]
        public void Initialize_ForceVoltageRampSucceeds()
        {
            var tsmContext = CreateTSMContext("HLSTestPinMap.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD", "VCC" };

            ForceVoltageRamp.SameValueToAllSmuPins(tsmContext, pins);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_ForceSynchronizedVoltageRampAndFetchSucceeds()
        {
            var tsmContext = CreateTSMContext("HLSTestPinMap.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD", "VCC" };

            ForceSynchronizedVoltageRampAndFetch.SameValueToAllSmuPins(tsmContext, pins);
            CleanupInstrumentation(tsmContext);
        }
    }
}