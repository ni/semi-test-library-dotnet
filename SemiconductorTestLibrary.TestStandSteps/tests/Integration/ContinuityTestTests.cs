﻿using System;
using NationalInstruments.ModularInstruments.NIDCPower;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class ContinuityTestTests
    {
        [Fact]
        public void Initialize_RunContinuityTestWithNegativeCurrentLevel_CorrectDataPublished()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);
            string[] continuityPins = new string[] { "VCC1", "VCC2" };

            ContinuityTest(
                tsmContext,
                supplyPinsOrPinGroups: Array.Empty<string>(),
                currentLimitsPerSupplyPinOrPinGroup: Array.Empty<double>(),
                continuityPinsOrPinGroups: continuityPins,
                currentLevelPerContinuityPinOrPinGroup: new double[] { -0.02, -0.01 },
                voltageLimitHighPerContinuityPinOrPinGroup: new double[] { 1, 1 },
                voltageLimitLowPerContinuityPinOrPinGroup: new double[] { -1, -1 },
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            AssertPublishedDataCountPerPins(tsmContext.SiteNumbers.Count, continuityPins, publishedData);
            // limits are set based on the expected value returned by the driver when in Offline Mode.
            AssertPublishedDataValueInRange(publishedData, 0.075, 0.085);
            AssertPublishedDataId("Continuity", publishedData);
            CleanupInstrumentation(tsmContext);
        }
    }
}
