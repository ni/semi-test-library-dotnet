using System.Reflection;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    internal static class SignatureCheckUtilities
    {
        internal static void AssertTsmcontextParameter(ParameterInfo parameter)
        {
            // validate tsmContext.
            Assert.Equal("tsmContext", parameter.Name);
            Assert.Equal(typeof(ISemiconductorModuleContext), parameter.ParameterType);
            Assert.False(parameter.IsOptional);
        }

        internal static void AssertResetDeviceParameter(ParameterInfo parameter)
        {
            // Validate resetDevice  (bool, default = false)
            Assert.Equal("resetDevice", parameter.Name);
            Assert.Equal(typeof(bool), parameter.ParameterType);
            Assert.True(parameter.IsOptional);
            Assert.Equal(false, parameter.DefaultValue);
        }
    }
}
