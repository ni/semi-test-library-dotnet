using System.Linq;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    public class AcquireAnalogInputWaveformsTests
    {
        [Fact]
        public void InitializeDAQmxAIVoltageTask_RunDAQmxTestAcquireAnalogInputWaveforms_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("DAQmxMultiChannelTests.pinmap", out var publishedDataReader);
            SetupNIDAQmxAIVoltageTask(tsmContext);

            AcquireAnalogInputWaveforms(
                tsmContext,
                pinsOrPinGroups: new[] { "AIPin" });

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            string[] analogInputPins = new[] { "AIPin" };
            // Validate Maximum Value.
            var publishedDataMaximum = publishedData.Where(d => d.PublishedDataId == "Maximum").ToArray();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, analogInputPins, publishedDataMaximum);
            if (tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                // Limits are based on the expected value returned by the driver when in Offline Mode.
                AssertPublishedDataValueInRange(publishedDataMaximum, 9, 10);
            }
            else
            {
                // When run on tester, limits are set based on the maximum voltage limits provided.
                AssertPublishedDataValueInRange(publishedDataMaximum, -0.001, 0.001);
            }
            // Validate Minimum Value.
            var publishedDataMinimum = publishedData.Where(d => d.PublishedDataId == "Minimum").ToArray();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, analogInputPins, publishedDataMaximum);
            if (tsmContext.IsSemiconductorModuleInOfflineMode)
            {
                // Limits are based on the expected value returned by the driver when in Offline Mode.
                AssertPublishedDataValueInRange(publishedDataMinimum, -10, -9);
            }
            else
            {
                // When run on tester, limits are set based on the maximum voltage limits provided.
                AssertPublishedDataValueInRange(publishedDataMinimum, -0.001, 0.001);
            }
        }
    }
}
