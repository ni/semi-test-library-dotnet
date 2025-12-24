using System.Linq;
using System.Reflection;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void SetupNIDCPowerInstrumentationWithParameters_HasCorrectSignature()
        {
            var type = typeof(SetupAndCleanupSteps);
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

            var method = type.GetMethod(
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
        public void GetAllSetupNIDCPowerInstrumentationOverloads_HasCorrectOverloadCount()
        {
            var type = typeof(SetupAndCleanupSteps);
            int expectedOverloadCount = 1;

            var overloads = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDCPowerInstrumentation");

            Assert.NotNull(overloads);
            Assert.Equal(expectedOverloadCount, overloads.Count());
        }
    }
}
