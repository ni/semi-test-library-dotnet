using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    [Collection("NonParallelizable")]
    public class DutPowerDownTests
    {
        [Theory]
        [InlineData("PowerSupplyDevices.pinmap", "VEE")]
        [InlineData("PowerSupplyDevices.pinmap", "VDD1")]
        [InlineData("PowerSupplyDevices.pinmap", "VDD2")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC1")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC2")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC3")]
        [InlineData("DifferentSMUDevices.pinmap", "VDD")]
        public void Initialize_DutPowerDown_CorrectValuesSet(string pinmap, string pinName)
        {
            var tsmContext = CreateTSMContext(pinmap);
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            DutPowerDown(
                tsmContext,
                dutSupplyPinsOrPinGroups: new[] { pinName },
                forceLowestCurrentLimit: true);

            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPower = sessionManager.DCPower(pinName);
            dcPower.Do((sessionInfo, sitePinInfo) =>
            {
                var outPut = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                bool isOverRangeEnabled = outPut.Source.OverrangingEnabled;
                var voltageLevel = outPut.Source.Voltage.VoltageLevel;
                var currentLimit = outPut.Source.Voltage.CurrentLimit;
                bool isPowerSupply = PowerSupplySettings.TryGetValue(sitePinInfo.ModelString, out var powerSupplyInstrumentConfiguration);
                var expectedVoltageLevel = isPowerSupply ? powerSupplyInstrumentConfiguration.GetVoltageLevel(isOverRangeEnabled) : 0;
                Assert.Equal(voltageLevel, expectedVoltageLevel);
            });
            CleanupInstrumentation(tsmContext);
        }
    }
}
