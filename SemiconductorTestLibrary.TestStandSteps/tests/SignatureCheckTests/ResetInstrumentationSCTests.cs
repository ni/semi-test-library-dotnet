using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void ResetInstrumentation_HasCorrectSignature()
        {
            // Retrieve method
            var method = typeof(SetupAndCleanupSteps).GetMethod(
                "ResetInstrumentation",
                BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(method);

            // Get parameters
            var parameters = method.GetParameters();

            // ---- Parameter Count ----------------------------------------------------
            Assert.Equal(3, parameters.Length);

            // -------------------------------------------------------------------------
            // Parameter 1: tsmContext
            // -------------------------------------------------------------------------
            Assert.Equal("tsmContext", parameters[0].Name);
            Assert.Equal(typeof(ISemiconductorModuleContext), parameters[0].ParameterType);
            Assert.False(parameters[0].IsOptional);

            // -------------------------------------------------------------------------
            // Parameter 2: resetDevice  (bool, default = false)
            // -------------------------------------------------------------------------
            Assert.Equal("resetDevice", parameters[1].Name);
            Assert.Equal(typeof(bool), parameters[1].ParameterType);
            Assert.True(parameters[1].IsOptional);
            Assert.Equal(false, parameters[1].DefaultValue);

            // -------------------------------------------------------------------------
            // Parameter 3: instrumentType  (NIInstrumentType, default = NIInstrumentType.All)
            // -------------------------------------------------------------------------
            Assert.Equal("instrumentType", parameters[2].Name);
            Assert.Equal(typeof(NIInstrumentType), parameters[2].ParameterType);
            Assert.True(parameters[2].IsOptional);
            Assert.Equal(NIInstrumentType.All, parameters[2].DefaultValue);

            // ---- Return Type --------------------------------------------------------
            Assert.Equal(typeof(void), method.ReturnType);
        }
    }
}