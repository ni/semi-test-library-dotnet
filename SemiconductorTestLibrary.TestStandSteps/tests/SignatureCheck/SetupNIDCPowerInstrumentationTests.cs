using System.Linq;
using System.Reflection;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void SetupNIDCPowerInstrumentation_GetExactOverload_HasCorrectSignature()
        {
            // Arrange
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

            // Act: get exact overload
            var method = type.GetMethod(
                "SetupNIDCPowerInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            // Assert
            Assert.NotNull(method);
            var parameters = method.GetParameters();
            SignatureCheckUtilities.AssertParameter(parameters[0], "tsmContext", false);
            SignatureCheckUtilities.AssertBoolParameter(parameters[1], "resetDevice", true, false);
            SignatureCheckUtilities.AssertDoubleParameter(parameters[2], "apertureTime", true, 1);
            SignatureCheckUtilities.AssertEnumParameter(parameters[3], "apertureTimeUnits", true, 1029); // Value of 'DCPowerMeasureApertureTimeUnits.PowerLineCycles' is 1029.
            SignatureCheckUtilities.AssertEnumParameter(parameters[4], "measureWhen", true, 1026); // Value of 'DCPowerMeasurementWhen.OnDemand' is 1026.
            SignatureCheckUtilities.AssertEnumParameter(parameters[5], "measurementSense", true, 1009); // Value of 'DCPowerMeasurementSense.Remote' is 1009.
            SignatureCheckUtilities.AssertDoubleParameter(parameters[6], "sourceDelay", true, -1);
            SignatureCheckUtilities.AssertDoubleParameter(parameters[7], "powerLineFrequency", true, -1);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void SetupNIDCPowerInstrumentation_GetAllOverloads_HasCorrectOverloadCount()
        {
            // Arrange
            var type = typeof(SetupAndCleanupSteps);
            int expectedOverloadCount = 1;

            // Act: get all overloads
            var overloads = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "SetupNIDCPowerInstrumentation").ToArray();

            // Assert
            Assert.NotNull(overloads);
            Assert.Equal(expectedOverloadCount, overloads.Length);
        }
    }
}
