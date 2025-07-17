using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class ContinuityTestTests
    {
        [Fact]
        public void Initialize_RunContinuityTestWithNegativeCurrentLevel_Succeeds()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap", out var publishedDataReader, "Mixed Signal Tests.digiproj");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            ContinuityTest(
                tsmContext,
                supplyPinsOrPinGroups: Array.Empty<string>(),
                currentLimitsPerSupplyPinOrPinGroup: Array.Empty<double>(),
                continuityPinsOrPinGroups: new string[] { "VCC1", "VCC2" },
                currentLevelPerContinuityPinOrPinGroup: new double[] { -0.02, -0.01 },
                voltageLimitHighPerContinuityPinOrPinGroup: new double[] { 1, 1 },
                voltageLimitLowPerContinuityPinOrPinGroup: new double[] { -1, -1 },
                apertureTime: 5e-5,
                settlingTime: 5e-5);

            var publishedData = publishedDataReader.GetAndClearPublishedData();
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "VCC1").Count());
            Assert.Equal(tsmContext.SiteNumbers.Count, publishedData.Where(d => d.Pin == "VCC2").Count());
            foreach (var data in publishedData)
            {
                Assert.InRange(data.DoubleValue, .075, 0.85);
                Assert.Equal("Continuity", data.PublishedDataId);
            }

            CleanupInstrumentation(tsmContext);
        }
    }
}
