using NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class GenerateStandardWaveformFGenTests
    {
        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void SameWaveformAcrossAllPinsAndSites_Succeeds(string pinmap)
        {
            var tsmContext = CreateTSMContext(pinmap);
            SetupNIFGenInstrumentation(tsmContext);
            string[] pins = { "A", "B" };

            GenerateStandardWaveformFGen.SameWaveformAcrossAllPinsAndSites(tsmContext, pins);

            CleanupInstrumentation(tsmContext);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void DifferentWaveformsPerSite_Succeeds(string pinmap)
        {
            var tsmContext = CreateTSMContext(pinmap);
            SetupNIFGenInstrumentation(tsmContext);
            string[] pins = { "A", "B" };

            GenerateStandardWaveformFGen.DifferentWaveformsPerSite(tsmContext, pins);

            CleanupInstrumentation(tsmContext);
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void DifferentWaveformsPerPinPerSite_Succeeds(string pinmap)
        {
            var tsmContext = CreateTSMContext(pinmap);
            SetupNIFGenInstrumentation(tsmContext);
            string[] pins = { "A", "B" };

            GenerateStandardWaveformFGen.DifferentWaveformsPerPinPerSite(tsmContext, pins);

            CleanupInstrumentation(tsmContext);
        }
    }
}