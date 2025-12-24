using System.Linq;
using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.SignatureCheckUtilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void GetCleanupInstrumentationWithParameters_HasCorrectSignature()
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
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertEnumParameter(parameters[2], "instrumentType", true, (int)NIInstrumentType.All);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllCleanupInstrumentationOverloads_HasCorrectOverloadCount()
        {
            var type = typeof(SetupAndCleanupSteps);
            int expectedOverloadCount = 1;

            var overloads = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "CleanupInstrumentation");

            Assert.Equal(expectedOverloadCount, overloads.Count());
        }
    }
}