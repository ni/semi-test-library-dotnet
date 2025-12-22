using System.Linq;
using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void ResetInstrumentation_ExactOverload_HasCorrectSignature()
        {
            // Arrange
            var type = typeof(SetupAndCleanupSteps);

            // Act: get exact overload
            var method = type.GetMethod(
                "ResetInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[]
                {
                typeof(ISemiconductorModuleContext),
                typeof(bool),
                typeof(NIInstrumentType)
                },
                modifiers: null);

            // Assert: method exists
            Assert.NotNull(method);

            var parameters = method.GetParameters();

            // Assert parameter count
            Assert.Equal(3, parameters.Length);

            // Parameter 1: tsmContext
            SignatureCheckUtilities.AssertTsmcontextParameter(parameters[0]);

            // Parameter 2: resetDevice
            SignatureCheckUtilities.AssertResetDeviceParameter(parameters[1]);

            // Parameter 3: NIInstrumentType
            Assert.Equal(typeof(NIInstrumentType), parameters[2].ParameterType);
            Assert.True(parameters[2].IsOptional);
            Assert.Equal(NIInstrumentType.All, parameters[2].DefaultValue);

            // Return Type
            Assert.Equal(typeof(void), method.ReturnType);
        }
    }
}