using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Cascade
{
    public class MergeTypeCascadeTests
    {
        public static IEnumerable<object[]> GetPinGroupsOfSMU4163()
        {
            List<string> pinGroupNames = new List<string> { "G1_1mA", "G2_1mA", "G3_1mA", "G4_1mA", "G1_2mA", "G2_2mA", "G1_4mA" };

            foreach (var pinGroupName in pinGroupNames)
            {
                yield return new object[] { pinGroupName };
            }
        }

        [Theory]
        [InlineData("G1_2mA")]
        [InlineData("G2_2mA")]
        [InlineData("G1_4mA")]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        public void InitializeSMU4147_MergePinGroupForceCurrentMeasureVoltageAndUnMergeSucceeds(string pinGroup)
        {
            var tsmContext = CreateTSMContext("Merged_4147.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            MergePinGroupForceCurrentMeasureVoltageAndUnMerge(tsmContext, pinGroup);
            CleanupInstrumentation(tsmContext);
        }

        [Theory]
        [InlineData("G1_2mA")]
        [InlineData("G2_2mA")]
        [InlineData("G1_4mA")]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        public void InitializeSMU4162_MergePinGroupForceCurrentMeasureVoltageAndUnMergeSucceeds(string pinGroup)
        {
            var tsmContext = CreateTSMContext("Merged_4162.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            MergePinGroupForceCurrentMeasureVoltageAndUnMerge(tsmContext, pinGroup);
            CleanupInstrumentation(tsmContext);
        }

        [Theory]
        [MemberData(nameof(GetPinGroupsOfSMU4163))]
        [Trait(nameof(Platform), nameof(Platform.TesterOnly))]
        [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
        public void InitializeSMU4163_MergePinGroupForceCurrentMeasureVoltageAndUnMergeSucceeds(string pinGroup)
        {
            var tsmContext = CreateTSMContext("Merged_4163.pinmap");
            SetupNIDCPowerInstrumentation(tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            MergePinGroupForceCurrentMeasureVoltageAndUnMerge(tsmContext, pinGroup);
            CleanupInstrumentation(tsmContext);
        }

        private void MergePinGroupForceCurrentMeasureVoltageAndUnMerge(ISemiconductorModuleContext tsmContext, string pinGroup)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var dcPower = sessionManager.DCPower(new[] { "G1_4mA" });
            dcPower.MergePinGroup(pinGroup);
            ForceCurrentMeasureVoltage(
                tsmContext,
                pinsOrPinGroups: new[] { pinGroup },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);
            dcPower.UnmergePinGroup(pinGroup);
        }
    }
}
