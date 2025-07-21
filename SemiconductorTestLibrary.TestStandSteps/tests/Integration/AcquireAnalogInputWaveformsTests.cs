using System.Linq;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class AcquireAnalogInputWaveformsTests
    {
        [Fact]
        public void InitializeDAQmxAIVoltageTask_RunDAQmxTestAcquireAnalogInputWaveforms_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("DAQmxTests.pinmap", out var publishedDataReader);
            SetupNIDAQmxAIVoltageTask(tsmContext);

            AcquireAnalogInputWaveforms(
                tsmContext,
                pinsOrPinGroups: new[] { "AllAIPins" });

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            string[] analogInputPins = new[] { "VCC1", "VCC2", "VDET" };
            // Validate Maximum Value, limits are based on the expected value returned by the driver when in Offline Mode.
            var publishedDataMaximum = publishedData.Where(d => d.PublishedDataId == "Maximum").ToArray();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, analogInputPins, publishedDataMaximum);
            AssertPublishedDataValueInRange(publishedDataMaximum, 9, 10);
            // Validate Minimum Value, limits are based on the expected value returned by the driver when in Offline Mode.
            var publishedDataMinimum = publishedData.Where(d => d.PublishedDataId == "Minimum").ToArray();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, analogInputPins, publishedDataMaximum);
            AssertPublishedDataValueInRange(publishedDataMinimum, -10, -9);
        }
    }
}
