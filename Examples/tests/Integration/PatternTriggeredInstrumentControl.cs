using NationalInstruments.ModularInstruments.NIDCPower;
using Xunit;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.PatternTriggeredInstrumentControl;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class PatternTriggeredInstrumentControl
    {
        [Fact]
        public void Initialize_RunPatternTriggeredSmuMeasurement_SucceedsAndPublishesData()
        {
            var tsmContext = CreateTSMContext("PatternTriggeredInstrumentControl.pinmap", out var publishedDataReader, "PatternTriggeredInstrumentControl.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            var digitalPatternPinNames = new[] { "DigitalPins" };
            var smuPinNames = new[] { "VDD" };
            var patternName = "GenerateTrigger";
            var publishedDataID = "SmuMeasurementID-TESTING";

            PatternTriggeredSmuMeasurement(
                tsmContext,
                digitalPatternPinNames,
                smuPinNames,
                patternName,
                publishedDataID: publishedDataID);

            CleanupInstrumentation(tsmContext);

            var publishedData = publishedDataReader.GetAndClearPublishedData();

            Assert.NotEmpty(publishedData);
            Assert.Equal(publishedDataID, publishedData[0].PublishedDataId);
        }

        [Fact]
        public void Initialize_RunPatternTriggeredDmmMeasurement_SucceedsAndPublishesData()
        {
            var tsmContext = CreateTSMContext("PatternTriggeredInstrumentControl.pinmap", out var publishedDataReader, "PatternTriggeredInstrumentControl.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);
            SetupNIDMMInstrumentation(tsmContext);

            var digitalPatternPinNames = new[] { "DigitalPins" };
            var dmmPinNames = new[] { "DMM" };
            var patternName = "GenerateTrigger";
            var publishedDataID = "DmmMeasurementID-TESTING";

            PatternTriggeredDmmMeasurement(
                tsmContext,
                digitalPatternPinNames,
                dmmPinNames,
                patternName,
                publishedDataID: publishedDataID);

            CleanupInstrumentation(tsmContext);

            var publishedData = publishedDataReader.GetAndClearPublishedData();

            Assert.NotEmpty(publishedData);
            Assert.Equal(publishedDataID, publishedData[0].PublishedDataId);
        }
    }
}