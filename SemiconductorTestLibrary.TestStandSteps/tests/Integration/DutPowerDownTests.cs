using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Integration
{
    public class DutPowerDownTests
    {
        [Theory]
        [InlineData("PowerSupplyDevices.pinmap", "VCC_4051")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC_4150")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC_4151")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC_4110")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC_4112")]
        [InlineData("PowerSupplyDevices.pinmap", "VCC_4113")]
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
        }
    }
}
