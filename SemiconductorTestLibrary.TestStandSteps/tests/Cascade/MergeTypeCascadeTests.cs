using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.SetupAndCleanupSteps;
using static NationalInstruments.SemiconductorTestLibrary.TestStandSteps.CommonSteps;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Cascade
{
    public class MergeTypeCascadeTests
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        public MergeTypeCascadeTests()
        {
            _tsmContext = CreateTSMContext("pinmap"); // TODO : add the pinmap
        }

        [Theory]
        [InlineData("TwoPinPinGroup")]
        [InlineData("FourPinPinGroup")]
        public void InitializeSMU4147_MergePinGroupAndForceCurrentMeasureVoltage(string pinGroup)
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);
            SetupNIDCPowerInstrumentation(_tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            var sessionManager = new TSMSessionManager(_tsmContext);
            var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
            dcPower.MergePinGroup(pinGroup);
            ForceCurrentMeasureVoltage(
                _tsmContext,
                pinsOrPinGroups: new[] { "MergedPowerPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);
            dcPower.UnmergePinGroup(pinGroup);
        }

        [Theory]
        [InlineData("TwoPinPinGroup")]
        [InlineData("FourPinPinGroup")]
        [InlineData("EightPinPinGroup")]
        public void InitializeSMU4162_MergePinGroupAndForceCurrentMeasureVoltage(string pinGroup)
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);
            SetupNIDCPowerInstrumentation(_tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            var sessionManager = new TSMSessionManager(_tsmContext);
            var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
            dcPower.MergePinGroup(pinGroup);
            ForceCurrentMeasureVoltage(
                _tsmContext,
                pinsOrPinGroups: new[] { "MergedPowerPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);
            dcPower.UnmergePinGroup(pinGroup);
        }

        [Theory]
        [InlineData("TwoPinPinGroup")]
        [InlineData("FourPinPinGroup")]
        [InlineData("EightPinPinGroup")]
        public void InitializeSMU4167_MergePinGroupAndForceCurrentMeasureVoltage(string pinGroup)
        {
            SetupNIDigitalPatternInstrumentation(_tsmContext);
            SetupNIDCPowerInstrumentation(_tsmContext, measurementSense: DCPowerMeasurementSense.Local);

            var sessionManager = new TSMSessionManager(_tsmContext);
            var dcPower = sessionManager.DCPower(new[] { "PowerPins" });
            dcPower.MergePinGroup(pinGroup);
            ForceCurrentMeasureVoltage(
                _tsmContext,
                pinsOrPinGroups: new[] { "MergedPowerPins" },
                currentLevel: 0.005,
                voltageLimit: 3.3,
                apertureTime: 5e-5,
                settlingTime: 5e-5);
            dcPower.UnmergePinGroup(pinGroup);
        }
    }
}
