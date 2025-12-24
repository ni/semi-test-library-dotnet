using System.Linq;
using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void GetResetInstrumentationWithParameters_HasCorrectSignature()
        {
            var type = typeof(SetupAndCleanupSteps);
            var parameterTypes = new[] { typeof(ISemiconductorModuleContext), typeof(bool), typeof(NIInstrumentType) };

            var method = type.GetMethod(
                "ResetInstrumentation",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertBoolParameter(parameters[1], "resetDevice", true, false);
            AssertEnumParameter(parameters[2], "instrumentType", true, (int)NIInstrumentType.All);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllResetInstrumentationOverloads_HasCorrectOverloadCount()
        {
            var type = typeof(SetupAndCleanupSteps);
            int expectedOverloadCount = 1;

            var overloads = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "ResetInstrumentation");

            Assert.Equal(expectedOverloadCount, overloads.Count());
        }
    }
}