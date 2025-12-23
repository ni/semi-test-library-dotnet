using System.Reflection;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Utilities
{
    internal static class SignatureCheckUtilities
    {
        internal static void AssertEnumParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, int expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, (int)parameterInfo.DefaultValue);
        }

        internal static void AssertBoolParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional,  bool expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertDoubleParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional, double expectedDefaultValue)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
            Assert.Equal(expectedDefaultValue, parameterInfo.DefaultValue);
        }

        internal static void AssertParameter(ParameterInfo parameterInfo, string expectedName, bool expectedIsOptional)
        {
            Assert.Equal(expectedName, parameterInfo.Name);
            Assert.Equal(expectedIsOptional, parameterInfo.IsOptional);
        }
    }
}
