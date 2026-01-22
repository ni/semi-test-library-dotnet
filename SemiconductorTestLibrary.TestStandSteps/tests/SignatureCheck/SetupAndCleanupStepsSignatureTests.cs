using System;
using System.Linq;
using System.Reflection;
using NationalInstruments.DAQmx;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public class SetupAndCleanupStepsSignatureTests
    {
        [Fact]
        public void GetSetupNIDAQmxAIVoltageTaskWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string),
                typeof(double),
                typeof(double),
                typeof(DAQmxTerminalConfiguration)
            };
            var method = classType.GetMethod(
                "SetupNIDAQmxAIVoltageTask",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertStringParameter(parameters[1], "taskType", true, DefaultDAQmxTaskTypeStrings.AnalogInput);
            AssertDoubleParameter(parameters[2], "maxiumValue", true, 10);
            AssertDoubleParameter(parameters[3], "minimumValue", true, -10);
            AssertEnumParameter(parameters[4], "inputTerminalConfiguration", true, (int)DAQmxTerminalConfiguration.Default);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDAQmxAIVoltageTaskOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDAQmxAIVoltageTask");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDAQmxAOFGenVoltageTaskWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string),
                typeof(AOFunctionGenerationType),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(DAQmxTerminalConfiguration)
            };
            var method = classType.GetMethod(
                "SetupNIDAQmxAOFGenVoltageTask",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertStringParameter(parameters[1], "taskType", true, DefaultDAQmxTaskTypeStrings.AnalogOutputFunctionGeneration);
            AssertEnumParameter(parameters[2], "waveformType", true, (int)AOFunctionGenerationType.Sine);
            AssertDoubleParameter(parameters[3], "frequency", true, 1000.0);
            AssertDoubleParameter(parameters[4], "amplitude", true, 5.0);
            AssertDoubleParameter(parameters[5], "offset", true, 0.0);
            AssertEnumParameter(parameters[6], "outputTerminalConfiguration", true, (int)DAQmxTerminalConfiguration.Default);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDAQmxAOFGenVoltageTaskOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDAQmxAOFGenVoltageTask");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDAQmxAOVoltageTaskWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string),
                typeof(double),
                typeof(double),
                typeof(DAQmxTerminalConfiguration)
            };
            var method = classType.GetMethod(
                "SetupNIDAQmxAOVoltageTask",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertStringParameter(parameters[1], "taskType", true, DefaultDAQmxTaskTypeStrings.AnalogOutput);
            AssertDoubleParameter(parameters[2], "maxiumValue", true, 10);
            AssertDoubleParameter(parameters[3], "minimumValue", true, -10);
            AssertEnumParameter(parameters[4], "outputTerminalConfiguration", true, (int)DAQmxTerminalConfiguration.Default);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDAQmxAOVoltageTaskOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDAQmxAOVoltageTask");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDAQmxDITaskWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string),
                typeof(ChannelLineGrouping)
            };
            var method = classType.GetMethod(
                "SetupNIDAQmxDITask",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertStringParameter(parameters[1], "taskType", true, DefaultDAQmxTaskTypeStrings.DigitalInput);
            AssertEnumParameter(parameters[2], "channelLineGrouping", true, (int)ChannelLineGrouping.OneChannelForEachLine);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDAQmxDITaskOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDAQmxDITask");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDAQmxDOTaskWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string),
                typeof(ChannelLineGrouping)
            };
            var method = classType.GetMethod(
                "SetupNIDAQmxDOTask",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertStringParameter(parameters[1], "taskType", true, DefaultDAQmxTaskTypeStrings.DigitalOutput);
            AssertEnumParameter(parameters[2], "channelLineGrouping", true, (int)ChannelLineGrouping.OneChannelForEachLine);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDAQmxDOTaskOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDAQmxDOTask");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDCPowerInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(double),
                typeof(DCPowerMeasureApertureTimeUnits),
                typeof(DCPowerMeasurementWhen),
                typeof(DCPowerMeasurementSense),
                typeof(double),
                typeof(double)
            };
            var method = classType.GetMethod(
                "SetupNIDCPowerInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertDoubleParameter(parameters[2], "apertureTime", true, 1);
            AssertEnumParameter(parameters[3], "apertureTimeUnits", true, (int)DCPowerMeasureApertureTimeUnits.PowerLineCycles);
            AssertEnumParameter(parameters[4], "measureWhen", true, (int)DCPowerMeasurementWhen.OnDemand);
            AssertEnumParameter(parameters[5], "measurementSense", true, (int)DCPowerMeasurementSense.Remote);
            AssertDoubleParameter(parameters[6], "sourceDelay", true, -1);
            AssertDoubleParameter(parameters[7], "powerLineFrequency", true, -1);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDCPowerInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDCPowerInstrumentation");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDMMInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(DmmApertureTimeUnits),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(DMMMeasurementSettings?)
            };
            var method = classType.GetMethod(
                "SetupNIDMMInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertEnumParameter(parameters[2], "apertureTimeUnits", true, (int)DmmApertureTimeUnits.PowerLineCycles);
            AssertDoubleParameter(parameters[3], "apertureTime", true, 1);
            AssertDoubleParameter(parameters[4], "settleTime", true, 0.01);
            AssertDoubleParameter(parameters[5], "powerLineFrequency", true, -1);
            AssertStructParameter(parameters[6], "initialMeasurmentSettings", true, null);
            Assert.Equal("initialMeasurmentSettings", parameters[6].Name);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDMMInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDMMInstrumentation");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIDigitalPatternInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(string),
                typeof(string),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "SetupNIDigitalPatternInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertStringParameter(parameters[2], "levelsSheetToApply", true, string.Empty);
            AssertStringParameter(parameters[3], "timingSheetToApply", true, string.Empty);
            AssertBoolParameter(parameters[4], "applySourceWaveformData", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetSetupNIDigitalPatternInstrumentationDeprecatedOverloadWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(string),
                typeof(string)
            };
            var method = classType.GetMethod(
                "SetupNIDigitalPatternInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertStringParameter(parameters[2], "levelsSheetToApply", true, string.Empty);
            AssertStringParameter(parameters[3], "timingSheetToApply", true, string.Empty);
            Assert.Equal(typeof(void), method.ReturnType);
            Assert.NotNull(method.GetCustomAttribute<ObsoleteAttribute>());
        }

        [Fact]
        public void GetSetupNIDigitalPatternInstrumentationWithNoOptionalParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext)
            };
            var method = classType.GetMethod(
                "SetupNIDigitalPatternInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIDigitalPatternInstrumentationOverloads_HasExpectedOverloadCount()
        {
            int expectedOverloadCount = 3;
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDigitalPatternInstrumentation");

            Assert.Equal(expectedOverloadCount, overloads.Count());
        }

        [Fact]
        public void GetSetupNIRelayModulesWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(string)
            };
            var method = classType.GetMethod(
                "SetupNIRelayModules",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertStringParameter(parameters[2], "initialRelayConfigurationToApply", true, string.Empty);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIRelayModulesOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIRelayModules");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIScopeInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "SetupNIScopeInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIScopeInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIScopeInstrumentation");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNIFGenInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "SetupNIFGenInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNIFGenInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIFGenInstrumentation");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetSetupNISyncInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "SetupNISyncInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllSetupNISyncInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNISyncInstrumentation");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetCleanupInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[] { typeof(ISemiconductorModuleContext), typeof(bool), typeof(NIInstrumentType) };
            var method = classType.GetMethod(
                "CleanupInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertEnumParameter(parameters[2], "instrumentType", true, (int)NIInstrumentType.All);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllCleanupInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "CleanupInstrumentation");

            Assert.Single(overloads);
        }

        [Fact]
        public void GetResetInstrumentationWithParameters_HasCorrectSignature()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[] { typeof(ISemiconductorModuleContext), typeof(bool), typeof(NIInstrumentType) };
            var method = classType.GetMethod(
                "ResetInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertEnumParameter(parameters[2], "instrumentType", true, (int)NIInstrumentType.All);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllResetInstrumentationOverloads_HasSingleOverload()
        {
            var classType = typeof(SetupAndCleanupSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ResetInstrumentation");

            Assert.Single(overloads);
        }
    }
}
