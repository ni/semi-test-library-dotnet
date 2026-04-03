using NationalInstruments.ModularInstruments.NIDCPower;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    public class DutPowerDownTests
    {
        [Fact]
        public void TestDutPowerDown()
        {
            var tsmContext = CreateTSMContext("Mixed Signal Tests.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            DutPowerDown(
                tsmContext,
                dutSupplyPinsOrPinGroups: new[] { "VDD" });
        }
    }
}
