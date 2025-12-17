using System.Linq;
using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void CleanupInstrumentation_HasCorrectSignature()
        {
            var type = typeof(SetupAndCleanupSteps);

            // Act, get all overloads of the method
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "CleanupInstrumentation").ToArray();

            // Assert, ensure there is only one method without any overload.
            Assert.Single(methods);

            var method = methods[0];
            var parameters = method.GetParameters();
            Assert.Equal(3, parameters.Length);

            // Parameter 1: tsmContext
            SignatureCheckUtilities.AssertTsmcontextParameter(parameters[0]);

            // Parameter 2: resetDevice
            SignatureCheckUtilities.AssertResetDeviceParameter(parameters[1]);

            // Parameter 3: instrumentType  (NIInstrumentType, default = NIInstrumentType.All)
            Assert.Equal("instrumentType", parameters[2].Name);
            Assert.Equal(typeof(NIInstrumentType), parameters[2].ParameterType);
            Assert.True(parameters[2].IsOptional);
            Assert.Equal(NIInstrumentType.All, parameters[2].DefaultValue);

            // Return Type
            Assert.Equal(typeof(void), method.ReturnType);
        }
    }
}