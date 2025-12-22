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
        public void SetupNIDCPowerInstrumentation_ExactOverload_HasCorrectSignature()
        {
            // Arrange
            var type = typeof(SetupAndCleanupSteps);

            // Act: get exact overload
            var method = type.GetMethod(
                "SetupNIDCPowerInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[]
                {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(double),
                typeof(DCPowerMeasureApertureTimeUnits),
                typeof(DCPowerMeasurementWhen),
                typeof(DCPowerMeasurementSense),
                typeof(double),
                typeof(double)
                },
                modifiers: null);

            // Assert: method exists
            Assert.NotNull(method);

            var parameters = method.GetParameters();

            // Assert parameter count
            Assert.Equal(8, parameters.Length);

            // Parameter 1: tsmContext
            SignatureCheckUtilities.AssertTsmcontextParameter(parameters[0]);

            // Parameter 2: resetDevice
            SignatureCheckUtilities.AssertResetDeviceParameter(parameters[1]);

            // Parameter 3: apertureTime (double, default = 1)
            Assert.Equal("apertureTime", parameters[2].Name);
            Assert.True(parameters[2].IsOptional);
            Assert.Equal(1d, parameters[2].DefaultValue);

            // Parameter 4: apertureTimeUnits (enum, default = PowerLineCycles)
            Assert.Equal("apertureTimeUnits", parameters[3].Name);
            Assert.True(parameters[3].IsOptional);
            Assert.Equal(DCPowerMeasureApertureTimeUnits.PowerLineCycles, parameters[3].DefaultValue);

            // Parameter 5: measureWhen (enum, default = OnDemand)
            Assert.Equal("measureWhen", parameters[4].Name);
            Assert.True(parameters[4].IsOptional);
            Assert.Equal(DCPowerMeasurementWhen.OnDemand, parameters[4].DefaultValue);

            // Parameter 6: measurementSense (enum, default = Remote)
            Assert.Equal("measurementSense", parameters[5].Name);
            Assert.True(parameters[5].IsOptional);
            Assert.Equal(DCPowerMeasurementSense.Remote, parameters[5].DefaultValue);

            // Parameter 7: sourceDelay (double, default = -1)
            Assert.Equal("sourceDelay", parameters[6].Name);
            Assert.True(parameters[6].IsOptional);
            Assert.Equal(-1d, parameters[6].DefaultValue);

            // Return Type
            Assert.Equal(typeof(void), method.ReturnType);
        }
    }
}
