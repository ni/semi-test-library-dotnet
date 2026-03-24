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
        public void Initialize_ForceSynchronizedVoltageRampAndFetchSucceeds()
        {
            var tsmContext = CreateTSMContext("DifferentSMUDevices.pinmap", out _);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD" };

            ForceVoltageSequence.ForceSynchronizedVoltageRampAndFetch(tsmContext, pins);
            CleanupInstrumentation(tsmContext);
        }

        [Fact]
        public void Initialize_ForceVoltageRampMeasureCurrent_SucceedsAndPublishesData()
        {
            var tsmContext = CreateTSMContext("HLSTestPinMap.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD", "VCC" };
            string publishedDataID = "ForceVoltageRampMeasureCurrent-TESTING";

            ForceVoltageSequence.ForceVoltageRampMeasureCurrent(tsmContext, pins, publishedDataID);

            CleanupInstrumentation(tsmContext);
            var publishedData = publishedDataReader.GetAndClearPublishedData();
            Assert.NotEmpty(publishedData);
            Assert.Equal(publishedDataID, publishedData[0].PublishedDataId);
        }
    }
}
