﻿using System.Linq;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.ChiXiao))]
    public class BurstPatternTests
    {
        [Fact]
        public void InitializeNIDigital_RunBurstPatternTest_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDigitalPatternInstrumentation(tsmContext);
            string[] digitalPins = new string[] { "PA_EN", "C0", "C1" };

            BurstPattern(
                tsmContext,
                pinsOrPinGroups: digitalPins,
                patternName: "TX_RF");

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            // Validate burst operation.
            var publishedDataForBurst = publishedData.Where(d => d.PublishedDataId == "Pattern Pass/Fail Result").ToArray();
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedDataForBurst.Length);
            // Expected value returned by the driver is True when in Offline Mode
            AssertPublishedDataValue(true, publishedDataForBurst);
            // Validate published data on each pins.
            var publishedDataForPins = publishedData.Where(d => d.PublishedDataId == "Pattern Fail Count").ToArray();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, digitalPins, publishedDataForPins);
            // Expected value returned by the driver is '0' when in Offline Mode.
            AssertPublishedDataValue(0, publishedDataForPins);
            CleanupInstrumentation(tsmContext);
        }
    }
}
