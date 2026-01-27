using System.Linq;
using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public class CommonStepsSignatureTests
    {
        [Fact]
        public void GetLeakageTestWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "LeakageTest",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "voltageLevel", false);
            AssertParameter(parameters[3], "currentLimit", false);
            AssertParameter(parameters[4], "apertureTime", false);
            AssertParameter(parameters[5], "settlingTime", false);
            AssertBoolParameter(parameters[6], "serialOperationEnabled", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllLeakageTestOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "LeakageTest");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetContinuityTestWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double[]),
                typeof(string[]),
                typeof(double[]),
                typeof(double[]),
                typeof(double[]),
                typeof(double),
                typeof(double)
            };
            var method = classType.GetMethod(
                "ContinuityTest",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "supplyPinsOrPinGroups", false);
            AssertParameter(parameters[2], "currentLimitsPerSupplyPinOrPinGroup", false);
            AssertParameter(parameters[3], "continuityPinsOrPinGroups", false);
            AssertParameter(parameters[4], "currentLevelPerContinuityPinOrPinGroup", false);
            AssertParameter(parameters[5], "voltageLimitHighPerContinuityPinOrPinGroup", false);
            AssertParameter(parameters[6], "voltageLimitLowPerContinuityPinOrPinGroup", false);
            AssertParameter(parameters[7], "apertureTime", false);
            AssertParameter(parameters[8], "settlingTime", false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllContinuityTestOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ContinuityTest");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetDutPowerDownWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(bool),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "DutPowerDown",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "dutSupplyPinsOrPinGroups", false);
            AssertDoubleParameter(parameters[2], "settlingTime", true, 0);
            AssertBoolParameter(parameters[3], "powerDownSuppliesSerially", true, false);
            AssertBoolParameter(parameters[4], "forceLowestCurrentLimit", true, true);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllDutPowerDownOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "DutPowerDown");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetDutPowerUpWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double[]),
                typeof(double[]),
                typeof(double),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "DutPowerUp",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "dutSupplyPinsOrPinGroups", false);
            AssertParameter(parameters[2], "perSupplyPinOrPinGroupVoltages", false);
            AssertParameter(parameters[3], "perSupplyPinOrPinGroupCurrentLimits", false);
            AssertDoubleParameter(parameters[4], "settlingTime", true, 0);
            AssertBoolParameter(parameters[5], "powerUpSuppliesSerially", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllDutPowerUpOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "DutPowerUp");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetAcquireAnalogInputWaveformsWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(int)
            };
            var method = classType.GetMethod(
                "AcquireAnalogInputWaveforms",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertIntParameter(parameters[2], "samplesPerChannel", true, 1000);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllAcquireAnalogInputWaveformsOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "AcquireAnalogInputWaveforms");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetBurstPatternWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(string)
            };
            var method = classType.GetMethod(
                "BurstPattern",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "patternName", false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllBurstPatternOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "BurstPattern");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetForceCurrentMeasureVoltageWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double)
            };
            var method = classType.GetMethod(
                "ForceCurrentMeasureVoltage",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "currentLevel", false);
            AssertParameter(parameters[3], "voltageLimit", false);
            AssertDoubleParameter(parameters[4], "settlingTime", true, 0);
            AssertDoubleParameter(parameters[5], "apertureTime", true, -1);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllForceCurrentMeasureVoltageOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ForceCurrentMeasureVoltage");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetForceVoltageMeasureCurrentWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double)
            };
            var method = classType.GetMethod(
                "ForceVoltageMeasureCurrent",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "voltageLevel", false);
            AssertParameter(parameters[3], "currentLimit", false);
            AssertDoubleParameter(parameters[4], "settlingTime", true, 0);
            AssertDoubleParameter(parameters[5], "apertureTime", true, -1);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllForceVoltageMeasureCurrentOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ForceVoltageMeasureCurrent");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetForceDcCurrentWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(double),
                typeof(double)
            };
            var method = classType.GetMethod(
                "ForceDcCurrent",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "currentLevel", false);
            AssertParameter(parameters[3], "voltageLimit", false);
            AssertDoubleParameter(parameters[4], "settlingTime", true, 0);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllForceDcCurrentOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ForceDcCurrent");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetForceDcVoltageWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(double),
                typeof(double)
            };
            var method = classType.GetMethod(
                "ForceDcVoltage",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "voltageLevel", false);
            AssertParameter(parameters[3], "currentLimit", false);
            AssertDoubleParameter(parameters[4], "settlingTime", true, 0);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllForceDcVoltageOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ForceDcVoltage");

            Assert.Single(overloads);
        }
    }
}
