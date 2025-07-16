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

            // Validate publish data.
            var publishedData = publishedDataReader.GetAndClearPublishedData();

            // Filter publish data by PublishID.
            var filteredPublishedData = publishedData.Where(pd => pd.PublishedDataId == "Continuity").ToArray();

            // generate dictionary of dictionary with site number as key and pin number as key for nested dictionbary.
            var groupedData = filteredPublishedData
           .GroupBy(cd => cd.SiteNumber)
           .ToDictionary(
               siteGroup => siteGroup.Key, // Site key
               siteGroup => siteGroup
                  .GroupBy(cd => cd.Pin)
                  .ToDictionary(
                  gradeGroup => gradeGroup.Key, // pin key
                     gradeGroup => gradeGroup.ToList()));

            // get published data for each site.
            var sites = tsmContext.SiteNumbers;
            foreach (var site in sites)
            {
                Assert.InRange(groupedData[site]["VCC1"][0].DoubleValue, 0.075, 0.085); // Expected value is around 0.08
                Assert.InRange(groupedData[site]["VCC2"][0].DoubleValue, 0.075, 0.085); // Expected value is around 0.08
            }

            CleanupInstrumentation(tsmContext);
        }
    }
}
