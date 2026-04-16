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
            var tsmContext = CreateTSMContext("DifferentSMUDevices.pinmap", out _);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD" };

            ForceVoltageSequence.ForceVoltageRamp(tsmContext, pins);

            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_ForceSynchronizedVoltageRampSucceeds()
        {
            var tsmContext = CreateTSMContext("DifferentSMUDevices.pinmap", out _);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD" };

            ForceVoltageSequence.ForceSynchronizedVoltageRamp(tsmContext, pins);

            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_ConfigureVoltageRampSequenceInitiateAndFetchCurrentMeasurementsSucceeds()
        {
            var tsmContext = CreateTSMContext("DifferentSMUDevices.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD" };

            ForceVoltageSequence.ConfigureVoltageRampSequenceInitiateAndFetchCurrentMeasurements(tsmContext, pins);

            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_ConfigureUpfrontAndInitiateAdvancedSequenceLaterSucceeds()
        {
            var tsmContext = CreateTSMContext("DifferentSMUDevices.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD" };

            ConfigureSMUAdvancedSequencesAndInitiate.ConfigureSMUAdvancedSequenceAndInitiate(tsmContext, pins);
            CleanupInstrumentation(tsmContext);
        }
    }
}
