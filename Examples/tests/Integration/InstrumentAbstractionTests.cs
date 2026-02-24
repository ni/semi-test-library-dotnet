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
        public void Initialize_ForceVoltageRampMeasureCurrent_SucceedsAndPublishesData()
        {
            var tsmContext = CreateTSMContext("HLSTestPinMap.pinmap", out var publishedDataReader);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] pins = { "VDD", "VCC" };
            string publishedDataID = "ForceVoltageRampMeasureCurrent-TESTING";

            ForceVoltageRampMeasureCurrent.ForceVoltageRampMeasureCurrentExample(tsmContext, pins, publishedDataID);

            var publishedData = publishedDataReader.GetAndClearPublishedData();

            Assert.NotEmpty(publishedData);
            Assert.Equal(publishedDataID, publishedData[0].PublishedDataId);
        }
    }
}
