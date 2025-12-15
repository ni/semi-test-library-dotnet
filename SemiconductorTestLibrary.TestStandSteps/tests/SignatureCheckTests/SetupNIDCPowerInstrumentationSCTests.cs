using System.Reflection;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void SetupNIDCPowerInstrumentation_HasCorrectSignature()
        {
            // Arrange
            var type = typeof(SetupAndCleanupSteps);

            // Act
            var method = type.GetMethod("SetupNIDCPowerInstrumentation", BindingFlags.Public | BindingFlags.Static);

            // Assert
            Assert.NotNull(method);

            var parameters = method.GetParameters();
            Assert.Equal(8, parameters.Length);

            // Parameter 1: tsmContext
            Assert.Equal("tsmContext", parameters[0].Name);
            Assert.Equal(typeof(ISemiconductorModuleContext), parameters[0].ParameterType);
            Assert.False(parameters[0].IsOptional);

            // Parameter 2: resetDevice (bool, default = false)
            Assert.Equal("resetDevice", parameters[1].Name);
            Assert.Equal(typeof(bool), parameters[1].ParameterType);
            Assert.True(parameters[1].IsOptional);
            Assert.Equal(false, parameters[1].DefaultValue);

            // Parameter 3: apertureTime (double, default = 1)
            Assert.Equal("apertureTime", parameters[2].Name);
            Assert.Equal(typeof(double), parameters[2].ParameterType);
            Assert.True(parameters[2].IsOptional);
            Assert.Equal(1d, parameters[2].DefaultValue);

            // Parameter 4: apertureTimeUnits (enum, default = PowerLineCycles)
            Assert.Equal("apertureTimeUnits", parameters[3].Name);
            Assert.Equal(typeof(DCPowerMeasureApertureTimeUnits), parameters[3].ParameterType);
            Assert.True(parameters[3].IsOptional);
            Assert.Equal(DCPowerMeasureApertureTimeUnits.PowerLineCycles, parameters[3].DefaultValue);

            // Parameter 5: measureWhen (enum, default = OnDemand)
            Assert.Equal("measureWhen", parameters[4].Name);
            Assert.Equal(typeof(DCPowerMeasurementWhen), parameters[4].ParameterType);
            Assert.True(parameters[4].IsOptional);
            Assert.Equal(DCPowerMeasurementWhen.OnDemand, parameters[4].DefaultValue);

            // Parameter 6: measurementSense (enum, default = Remote)
            Assert.Equal("measurementSense", parameters[5].Name);
            Assert.Equal(typeof(DCPowerMeasurementSense), parameters[5].ParameterType);
            Assert.True(parameters[5].IsOptional);
            Assert.Equal(DCPowerMeasurementSense.Remote, parameters[5].DefaultValue);

            // Parameter 7: sourceDelay (double, default = -1)
            Assert.Equal("sourceDelay", parameters[6].Name);
            Assert.Equal(typeof(double), parameters[6].ParameterType);
            Assert.True(parameters[6].IsOptional);
            Assert.Equal(-1d, parameters[6].DefaultValue);

            // Parameter 8: powerLineFrequency (double, default = -1)
            Assert.Equal("powerLineFrequency", parameters[7].Name);
            Assert.Equal(typeof(double), parameters[7].ParameterType);
            Assert.True(parameters[7].IsOptional);
            Assert.Equal(-1d, parameters[7].DefaultValue);
        }
    }
}
