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
        public void CleanupInstrumentation_GetExactOverload_HasCorrectSignature()
        {
            // Arrange
            var type = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[] { typeof(ISemiconductorModuleContext), typeof(bool), typeof(NIInstrumentType) };

            // Act: get exact overload
            var method = type.GetMethod(
                "CleanupInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            // Assert
            Assert.NotNull(method);
            var parameters = method.GetParameters();
            SignatureCheckUtilities.AssertParameter(parameters[0], "tsmContext", false);
            SignatureCheckUtilities.AssertBoolParameter(parameters[1], "resetDevice", true, false);
            SignatureCheckUtilities.AssertEnumParameter(parameters[2], "instrumentType", true, 0);
            Assert.Equal(typeof(void), method.ReturnType);
        }
    }
}